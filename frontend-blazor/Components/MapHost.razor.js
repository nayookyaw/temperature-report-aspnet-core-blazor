import { getSensorsInViewport } from '/js/sensor-api.js';

let map, sourceId = 'sensors';
let backendBase = '';
let lastKey = '';
let inflight = null;
let debounceTimer = null;
let selectedId = null;

function makeKey(b, z) {
  const q = v => Math.round(v * 1000) / 1000;
  return `${q(b.getWest())},${q(b.getSouth())},${q(b.getEast())},${q(b.getNorth())},z${Math.round(z)}`;
}

async function fetchViewport() {
  if (!map) return;
  const b = map.getBounds();
  const z = map.getZoom();
  const key = makeKey(b, z);
  if (key === lastKey) return;
  lastKey = key;

  if (inflight && typeof inflight.abort === 'function') inflight.abort();
  inflight = new AbortController();
  try {
    const items = await getSensorsInViewport(
      backendBase,
      {
        minLng: b.getWest().toFixed(6),
        minLat: b.getSouth().toFixed(6),
        maxLng: b.getEast().toFixed(6),
        maxLat: b.getNorth().toFixed(6),
        zoom: Math.round(z),
        limit: 10000
      },
      inflight.signal
    );

    const features = Array.isArray(items) ? items.map(item => {
      const id = item.id;
      const name = item.name ?? 'Sensor';
      const mac = item.macAddress ?? '';
      const status = item.status ?? 'Unknown';
      const serial = item.serialNumber ?? '';
      const temperature = item.temperature ?? null;
      const humidity = item.humidity ?? null;
      const lastSeen = item.lastSeenAt ?? null;

      return {
        type: 'Feature',
        id,
        properties: { id, name, mac, status, serial, temperature, humidity, lastSeen },
        geometry: { type: 'Point', coordinates: [item.Longitude ?? item.longitude, item.Latitude ?? item.latitude] }
      };
    }) : [];

    const geojson = { type: 'FeatureCollection', features };
    const src = map.getSource(sourceId);
    if (src) src.setData(geojson);
  } catch (err) {
    if (err.name !== 'AbortError') console.error('viewport fetch error', err);
  } finally {
    inflight = null;
  }
}

function debouncedFetchViewport() {
  if (debounceTimer) clearTimeout(debounceTimer);
  debounceTimer = setTimeout(fetchViewport, 150);
}

export function init(containerId, options) {
  backendBase = (options.backendBase || '');
  mapboxgl.accessToken = window.__MAPBOX_TOKEN__;
  map = new mapboxgl.Map({
    container: containerId,
    style: 'mapbox://styles/mapbox/streets-v12',
    center: options.center || [174.7633, -36.8485],
    zoom: options.zoom || 5
  });

  map.on('load', () => {
    map.addSource(sourceId, {
      type: 'geojson',
      data: { type: 'FeatureCollection', features: [] },
      cluster: true,
      clusterMaxZoom: 14,
      clusterRadius: 50,
      promoteId: 'id',
    });

    map.addLayer({
      id: 'clusters',
      type: 'circle',
      source: sourceId,
      filter: ['has', 'point_count'],
      paint: {
        'circle-radius': ['step', ['get', 'point_count'], 16, 100, 22, 750, 28],
        'circle-color': ['step', ['get', 'point_count'], 'rgba(246, 59, 246, 1)', 100, '#f59e0b', 750, '#ef4444'],
        'circle-stroke-color': '#ffffff',
        'circle-stroke-width': 1
      }
    });

    map.addLayer({
      id: 'cluster-count',
      type: 'symbol',
      source: sourceId,
      filter: ['has', 'point_count'],
      layout: {
        'text-field': ['get', 'point_count_abbreviated'],
        'text-size': 12,
        'text-allow-overlap': true
      },
      paint: { 'text-color': '#ffffff' }
    });

    map.addLayer({
      id: 'unclustered-point',
      type: 'circle',
      source: sourceId,
      filter: ['!', ['has', 'point_count']],
      paint: {
        'circle-radius': ['case', ['boolean', ['feature-state', 'selected'], false], 8, 6],
        'circle-color': ['case', ['boolean', ['feature-state', 'selected'], false], '#ef4444', 'rgba(218, 171, 17, 1)'],
        'circle-stroke-width': ['case', ['boolean', ['feature-state', 'selected'], false], 2, 1],
        'circle-stroke-color': '#ffffff'
      }
    });

    map.on('click', 'clusters', (e) => {
      const features = map.queryRenderedFeatures(e.point, { layers: ['clusters'] });
      const clusterId = features[0]?.properties?.cluster_id;
      if (clusterId == null) return;
      map.getSource(sourceId).getClusterExpansionZoom(clusterId, (err, zoom) => {
        if (err) return;
        map.easeTo({ center: features[0].geometry.coordinates, zoom });
      });
    });

    map.on('click', 'unclustered-point', (e) => {
      const f = e.features && e.features[0];
      if (!f) return;

      const fid = f.id ?? f.properties?.id;
      if (selectedId != null) map.setFeatureState({ source: sourceId, id: selectedId }, { selected: false });
      selectedId = fid;
      map.setFeatureState({ source: sourceId, id: selectedId }, { selected: true });

      const [lng, lat] = f.geometry.coordinates;
      const p = f.properties || {};
      const title = p.name || 'Sensor';
      const mac = p.mac || '';
      const status = (p.status || 'Unknown').toString();
      const serial = p.serial || '';
      const temp = p.temperature ?? null;
      const hum = p.humidity ?? null;
      const lastSeen = p.lastSeen || '';

      const id = `sensor-${p.id || Math.random().toString(36).slice(2)}`;

      const html = `
        <div class="sensor-popup">
          <div class="sp-header">
            <div class="sp-title">${escapeHtml(title)}</div>
            <div class="sp-right">
              <span class="sp-badge warn">${escapeHtml(status)}</span>
              <button id="${id}-close" class="sp-x" aria-label="Close">×</button>
            </div>
          </div>
          <div class="sp-grid">
            <div class="lbl">MAC</div><div>${escapeHtml(mac) || '-'}</div>
            <div class="lbl">Serial</div><div>${escapeHtml(serial) || '-'}</div>
            <div class="lbl">Temp</div><div>${fmtVal(temp, '°C')}</div>
            <div class="lbl">Humidity</div><div>${fmtVal(hum, '%')}</div>
            <div class="lbl">Last seen</div><div>${fmtTime(lastSeen) || '-'}</div>
            <div class="lbl">Coords</div><div>${lng.toFixed(5)}, ${lat.toFixed(5)}</div>
          </div>
          <div class="sp-actions">
            <button id="${id}-details" class="sp-btn primary">Details</button>
          </div>
        </div>
      `;

      new mapboxgl.Popup({ closeButton: false, offset: 10, anchor: 'bottom' })
        .setLngLat([lng, lat]).setHTML(html).addTo(map);

      setTimeout(() => {
        const det = document.getElementById(`${id}-details`);
        if (det) det.addEventListener('click', () => {
          window.dispatchEvent(new CustomEvent('sensor:details', { detail: { id: p.id } }));
        });
        const closeBtn = document.getElementById(`${id}-close`);
        if (closeBtn) closeBtn.addEventListener('click', () => {
          const el = closeBtn.closest('.mapboxgl-popup');
          if (el && el.parentNode) el.parentNode.removeChild(el);
        });
      }, 0);
    });

    map.on('click', (e) => {
      const hit = map.queryRenderedFeatures(e.point, { layers: ['unclustered-point'] });
      if (hit.length === 0 && selectedId != null) {
        map.setFeatureState({ source: sourceId, id: selectedId }, { selected: false });
        selectedId = null;
      }
    });

    map.on('moveend', debouncedFetchViewport);
    fetchViewport();
  });
}

function escapeHtml(s) {
  if (s == null) return '';
  return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;')
    .replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#039;');
}
function fmtVal(v, unit) {
  if (v == null || v === '') return '-';
  const n = Number(v);
  return Number.isFinite(n) ? `${n}${unit}` : escapeHtml(String(v));
}
function fmtTime(iso) {
  if (!iso) return '';
  try {
    const d = new Date(iso);
    if (isNaN(d.getTime())) return '';
    return d.toLocaleString();
  } catch { return ''; }
}

export function flyTo(lng, lat, zoom) {
  if (!map) return;
  map.flyTo({ center: [lng, lat], zoom: zoom || 14 });
}