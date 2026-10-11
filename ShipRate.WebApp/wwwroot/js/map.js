const map = L.map('map', {
    zoomControl: false,
    preferCanvas: true
});

L.control.zoom({ position: 'topright' }).addTo(map);

fetch('/maps/countries.geo.json')
    .then(r => r.json())
    .then(worldData => {
        const layer = L.geoJSON(worldData, {
            style: {
                color: '#334155',
                weight: 1,
                fillColor: '#f8fafc',
                fillOpacity: 1
            }
        }).addTo(map);

        map.fitBounds(layer.getBounds(), {
            padding: [30, 30]
        });
    });
