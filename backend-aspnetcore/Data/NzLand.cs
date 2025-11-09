namespace BackendAspNetCore.Geo;

public static class NzLand
{
    // Each polygon is a list of (lat, lng) vertices in WGS84.
    // Simplified outlines good enough for seeding; includes main coastlines (not tiny islands).
    private static readonly (double lat, double lng)[] NorthIsland =
    {
        (-41.35, 174.85), // Wellington
        (-39.70, 174.00), // Taranaki W
        (-38.45, 174.60), // Waikato W
        (-37.30, 174.90), // Auckland W
        (-36.25, 174.90), // Northland E
        (-35.00, 173.50), // Northland NW
        (-34.40, 172.70), // Cape Reinga
        (-35.40, 174.50), // Bay of Islands
        (-36.50, 175.30), // Hauraki Gulf
        (-37.40, 175.95), // Coromandel/BOP W
        (-37.70, 178.60), // East Cape
        (-39.10, 177.40), // Hawke Bay
        (-40.10, 176.20), // Manawatū/East
        (-41.00, 176.80), // Wairarapa
        (-41.35, 174.85), // back to Wellington
    };

    private static readonly (double lat, double lng)[] SouthIsland =
    {
        (-41.55, 173.50), // NE Marlborough
        (-41.80, 172.60), // NW (near Karamea)
        (-42.40, 171.60), // Westport
        (-43.30, 170.40), // Westland
        (-44.40, 168.30), // Fiordland/Te Anau W
        (-46.40, 167.80), // Southland SW
        (-46.60, 168.35), // Bluff
        (-45.90, 170.50), // Dunedin
        (-44.00, 171.00), // Canterbury mid
        (-43.50, 172.70), // Christchurch
        (-42.40, 173.60), // Kaikōura
        (-41.55, 173.50), // back to Marlborough
    };

    private static readonly (double lat, double lng)[] StewartIsland =
    {
        (-46.70, 167.60),
        (-46.70, 168.50),
        (-47.40, 168.50),
        (-47.40, 167.60),
        (-46.70, 167.60),
    };

    private static readonly (double lat, double lng)[][] Islands = { NorthIsland, SouthIsland, StewartIsland };

    public static bool IsOnLand(double lat, double lng)
    {
        foreach (var poly in Islands)
            if (PointInPolygon(lat, lng, poly)) return true;
        return false;
    }

    // Ray-casting (lat, lng) vs polygon (lat, lng)
    private static bool PointInPolygon(double lat, double lng, (double lat, double lng)[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            var (latI, lngI) = polygon[i];
            var (latJ, lngJ) = polygon[j];

            // Check if edge crosses the horizontal ray at 'lat'
            bool intersect = ((lngI > lng) != (lngJ > lng)) &&
                             (lat < (latJ - latI) * (lng - lngI) / (lngJ - lngI + double.Epsilon) + latI);
            if (intersect) inside = !inside;
        }
        return inside;
    }
}