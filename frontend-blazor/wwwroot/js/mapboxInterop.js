// mapboxInterop.js
// A simple Mapbox helper for clustering, popups, and smooth map interactions.

window.mapboxInterop = (function () {
  let map;
  const sourceId = "sensors";

  /**
   * Initialize the Mapbox map
   * @param {string} containerId - HTML element ID to mount the map
   * @param {[number, number]} [center=[174.7633, -36.8485]] - Initial map center [lng, lat]
   * @param {number} [zoom=5] - Initial zoom level
   */
  function initMap(containerId, center, zoom) {
    mapboxgl.accessToken = window.__MAPBOX_TOKEN__;

    // Create the Mapbox instance
    map = new mapboxgl.Map({
      container: containerId,
      style: "mapbox://styles/mapbox/streets-v12",
      center: center || [174.7633, -36.8485], // Default: Auckland
      zoom: zoom || 5,
    });

    map.on("load", () => {
      // --- Data source with clustering enabled ---
      map.addSource(sourceId, {
        type: "geojson",
        data: {
          type: "FeatureCollection",
          features: [],
        },
        cluster: true,
        clusterMaxZoom: 14, // Max zoom for clustering
        clusterRadius: 50,  // Cluster radius in pixels
      });

      // --- Cluster circles ---
      map.addLayer({
        id: "clusters",
        type: "circle",
        source: sourceId,
        filter: ["has", "point_count"],
        paint: {
          "circle-radius": [
            "step",
            ["get", "point_count"],
            15, 100, 20, 750, 30,
          ],
          "circle-color": [
            "step",
            ["get", "point_count"],
            "#51bbd6", // few points
            100, "#f1f075",
            750, "#f28cb1" // many points
          ],
        },
      });

      // --- Cluster count labels ---
      map.addLayer({
        id: "cluster-count",
        type: "symbol",
        source: sourceId,
        filter: ["has", "point_count"],
        layout: {
          "text-field": ["get", "point_count_abbreviated"],
          "text-size": 12,
        },
      });

      // --- Individual (unclustered) points ---
      map.addLayer({
        id: "unclustered-point",
        type: "circle",
        source: sourceId,
        filter: ["!", ["has", "point_count"]],
        paint: {
          "circle-radius": 6,
          "circle-color": "#11b4da",
          "circle-stroke-width": 1,
          "circle-stroke-color": "#fff",
        },
      });

      // --- Interactivity: clicking a cluster zooms in ---
      map.on("click", "clusters", (e) => {
        const features = map.queryRenderedFeatures(e.point, {
          layers: ["clusters"],
        });
        const clusterId = features[0].properties.cluster_id;

        map.getSource(sourceId).getClusterExpansionZoom(clusterId, (err, zoom) => {
          if (err) return;
          map.easeTo({
            center: features[0].geometry.coordinates,
            zoom: zoom,
          });
        });
      });

      // --- Clicking an individual point shows popup ---
      map.on("click", "unclustered-point", (e) => {
        const p = e.features[0].properties;
        const coords = e.features[0].geometry.coordinates.slice();

        const html = `
          <strong>${p.Name}</strong><br/>
          MAC: ${p.MacAddress}<br/>
          SN: ${p.SerialNumber}<br/>
          Temp: ${p.Temperature}&deg;C<br/>
          Humidity: ${p.Humidity}%
        `;

        new mapboxgl.Popup()
          .setLngLat(coords)
          .setHTML(html)
          .addTo(map);
      });

      // --- Cursor feedback ---
      map.on("mouseenter", "clusters", () => {
        map.getCanvas().style.cursor = "pointer";
      });
      map.on("mouseleave", "clusters", () => {
        map.getCanvas().style.cursor = "";
      });
    });
  }

  /**
   * Update the GeoJSON data displayed on the map.
   * @param {object} geoJson - GeoJSON FeatureCollection
   */
  function setData(geoJson) {
    const src = map.getSource(sourceId);
    if (src) src.setData(geoJson);
  }

  /**
   * Smoothly fly to a location on the map.
   * @param {number} lng - Longitude
   * @param {number} lat - Latitude
   */
  function flyTo(lng, lat) {
    if (!map) return;
    map.flyTo({ center: [lng, lat], zoom: 14 });
  }

  // --- Expose public methods ---
  return {
    initMap,
    setData,
    flyTo,
  };
})();