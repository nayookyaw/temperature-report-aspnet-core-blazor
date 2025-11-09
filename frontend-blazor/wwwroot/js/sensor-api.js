export async function getSensorsInViewport(backendBase, queryParams, signal) {
    const limit = typeof queryParams.limit === "number" ? queryParams.limit : 10000;

    // ensure consistent decimals
    const formatDecimal = v => Number(v).toFixed(6);

    const params = new URLSearchParams({
        minLng: formatDecimal(queryParams.minLng),
        minLat: formatDecimal(queryParams.minLat),
        maxLng: formatDecimal(queryParams.maxLng),
        maxLat: formatDecimal(queryParams.maxLat),
        zoom: Math.round(queryParams.zoom),
        limit: String(limit)
    });

    const sensorViewPortEndpoint = `${backendBase}v1/sensor/viewport?${params.toString()}`;

    const res = await fetch(sensorViewPortEndpoint, { signal });
    if (!res.ok) {
        const txt = await res.text().catch(() => "");
        throw new Error(`Viewport fetch failed ${res.status}: ${txt}`);
    }
    return res.json();
}