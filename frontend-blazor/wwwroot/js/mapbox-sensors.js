// Minimal Mapbox + cluster + viewport fetcher for Blazor interop (Blazor-safe: no @onclick in HTML)
window.SensorMap = (function () {
  let map, sourceId = 'sensors';
  let backendBase = '';
  let lastKey = '';
  let inflight = null;
  let debounceTimer = null;

  function makeKey(b, z) {
    // Quantize to reduce fetch churn
    const q = v => Math.round(v * 1000) / 1000;
    return `${q(b.getWest())},${q(b.getSouth())},${q(b.getEast())},${q(b.getNorth())},z${Math.round(z)}`;
  }

  async function fetchViewport() {
    if (!map) return;
    const b = map.getBounds();
    const z = map.getZoom();
    const key = makeKey(b, z);
    if (key === lastKey) return; // fast path: no change
    lastKey = key;

    // cancel any in-flight fetch
    if (inflight && typeof inflight.abort === 'function') {
      inflight.abort();
    }
    inflight = new AbortController();

    const params = new URLSearchParams({
      minLng: b.getWest().toFixed(6),
      minLat: b.getSouth().toFixed(6),
      maxLng: b.getEast().toFixed(6),
      maxLat: b.getNorth().toFixed(6),
      zoom: Math.round(z),
      limit: 10000
    });

    const url = `${backendBase}/v1/sensors/viewport?${params.toString()}`;
    // console.log (url)

    try {
      const res = await fetch(url, { signal: inflight.signal });
      if (!res.ok) {
        console.error('viewport fetch failed', res.status, await res.text());
        return;
      }
      const items = await res.json();

      const features = Array.isArray(items) ? items.map(it => ({
        type: 'Feature',
        properties: {
          id: it.Id,
          title: it.Name,
          mac: it.MacAddress,
          status: it.Status || ''
        },
        geometry: {
          type: 'Point',
          coordinates: [it.longitude, it.latitude]
        }
      })) : [];

      const src = map.getSource(sourceId);
      const geojson = { type: 'FeatureCollection', features };
      if (src) src.setData(geojson);
    } catch (err) {
      if (err.name !== 'AbortError') {
        console.error('viewport fetch error', err);
      }
    } finally {
      inflight = null;
    }
  }

  function debouncedFetchViewport() {
    if (debounceTimer) clearTimeout(debounceTimer);
    debounceTimer = setTimeout(fetchViewport, 150);
  }

  function init(containerId, options) {
    backendBase = (options.backendBase || '')?.replace(/\/+$/, ''); // trim trailing /
    mapboxgl.accessToken = window.__MAPBOX_TOKEN__;
    map = new mapboxgl.Map({
      container: containerId,
      style: 'mapbox://styles/mapbox/streets-v12',
      center: options.center || [174.7633, -36.8485], // Auckland
      zoom: options.zoom || 5
    });

    map.on('load', () => {
      // Source + layers
      map.addSource(sourceId, {
        type: 'geojson',
        data: { type: 'FeatureCollection', features: [] },
        cluster: true,
        clusterMaxZoom: 14,
        clusterRadius: 50
      });

      map.addLayer({
        id: 'clusters',
        type: 'circle',
        source: sourceId,
        filter: ['has', 'point_count'],
        paint: {
        // radius grows with cluster size
        'circle-radius': [
          'step',
          ['get', 'point_count'],
          16,   // < 100
          100,  22,   // 100–749
          750,  28    // 750+
        ],
        // colors per size bucket
        'circle-color': [
          'step',
          ['get', 'point_count'],
          '#3b82f6',  // <100   (blue-500)
          100, '#f59e0b', // 100–749 (amber-500)
          750, '#ef4444'  // 750+   (red-500)
        ],
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
        paint: {
          'text-color': '#ffffff'  // make sure it contrasts the bubble
        }
      });

      map.addLayer({
        id: 'unclustered-point',
        type: 'circle',
        source: sourceId,
        filter: ['!', ['has', 'point_count']],
        paint: {
          'circle-radius': 6,
          'circle-color': '#11b4da',
          'circle-stroke-width': 1,
          'circle-stroke-color': '#ffffff'
        }
      });

      // Safe interactions (NO Blazor directives in HTML)
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
        const [lng, lat] = f.geometry.coordinates;
        const title = f.properties?.title || '';
        const mac = f.properties?.mac || '';
        const status = f.properties?.status || '';

        // Plain HTML (no @onclick). We'll attach a JS listener.
        const id = `sensor-btn-${f.properties.id}`;
        const html = `
          <div style="min-width:200px">
            <div style="font-weight:600">${title}</div>
            <div><small>MAC: ${mac}</small></div>
            <div><small>Status: ${status}</small></div>
            <button id="${id}" type="button" style="margin-top:6px">Fly here</button>
          </div>
        `;

        new mapboxgl.Popup()
          .setLngLat([lng, lat])
          .setHTML(html)
          .addTo(map);

        // Attach JS click handler safely
        // Delay a tick to ensure DOM exists
        setTimeout(() => {
          const btn = document.getElementById(id);
          if (btn) {
            btn.addEventListener('click', () => {
              flyTo(lng, lat, 15);
            });
          }
        }, 0);
      });

      map.on('mouseenter', 'clusters', () => map.getCanvas().style.cursor = 'pointer');
      map.on('mouseleave', 'clusters', () => map.getCanvas().style.cursor = '');
      map.on('mouseenter', 'unclustered-point', () => map.getCanvas().style.cursor = 'pointer');
      map.on('mouseleave', 'unclustered-point', () => map.getCanvas().style.cursor = '');

      fetchViewport();
    });

    // Debounce to keep things snappy
    map.on('moveend', debouncedFetchViewport);
    map.on('load', () => console.log('Mapbox: load fired'));
    map.on('error', (e) => console.error('Mapbox error:', e && e.error));
  }

  function flyTo(lng, lat, zoom) {
    if (!map) return;
    map.flyTo({ center: [lng, lat], zoom: zoom || 14 });
  }

  return { init, flyTo };
})();