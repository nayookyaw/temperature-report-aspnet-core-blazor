export async function getSensorsInViewport(backendBase, q, signal) {
    const limit = typeof q.limit === "number" ? q.limit : 10000;

    // ensure consistent decimals
    const formatDecimal = v => Number(v).toFixed(6);

    const params = new URLSearchParams({
        minLng: formatDecimal(q.minLng),
        minLat: formatDecimal(q.minLat),
        maxLng: formatDecimal(q.maxLng),
        maxLat: formatDecimal(q.maxLat),
        zoom: Math.round(q.zoom),
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