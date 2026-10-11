const map = L.map('map', {
    zoomControl: false,
    preferCanvas: true,
    worldCopyJump: true,
    minZoom: 1,
    maxZoom: 8
});

L.control.zoom({ position: 'topright' }).addTo(map);

const countryLayerGroup = L.layerGroup().addTo(map);
const pointLayerGroup = L.layerGroup().addTo(map);
const routeLayerGroup = L.layerGroup().addTo(map);

function getMarkerIcon(type) {
    let cls = 'office-marker';

    if (type === 'Port') cls = 'port-marker';
    else if (type === 'Airport') cls = 'airport-marker';
    else if (type === 'Warehouse') cls = 'warehouse-marker';

    return L.divIcon({
        className: '',
        html: `<div class="map-marker ${cls}"></div>`,
        iconSize: [16, 16],
        iconAnchor: [8, 8]
    });
}

function routeColor(mode) {
    if (mode === 'Air') return '#2563eb';
    if (mode === 'Sea') return '#16a34a';
    if (mode === 'Road') return '#f59e0b';
    return '#64748b';
}

function addLegend() {
    const legend = L.control({ position: 'bottomleft' });

    legend.onAdd = function () {
        const div = L.DomUtil.create('div', 'map-legend');
        div.style.background = 'rgba(255,255,255,0.92)';
        div.style.padding = '10px 12px';
        div.style.borderRadius = '8px';
        div.style.border = '1px solid #dbe3ee';
        div.style.boxShadow = '0 1px 6px rgba(0,0,0,.08)';
        div.style.fontSize = '12px';
        div.innerHTML = `
            <div style="font-weight:700; margin-bottom:6px;">Legend</div>
            <div><span style="display:inline-block;width:10px;height:10px;background:#16a34a;border-radius:50%;margin-right:6px;"></span>Port</div>
            <div><span style="display:inline-block;width:10px;height:10px;background:#2563eb;border-radius:50%;margin-right:6px;"></span>Airport</div>
            <div><span style="display:inline-block;width:10px;height:10px;background:#f97316;border-radius:50%;margin-right:6px;"></span>Office</div>
            <div><span style="display:inline-block;width:10px;height:10px;background:#7c3aed;border-radius:50%;margin-right:6px;"></span>Warehouse</div>
        `;
        return div;
    };

    legend.addTo(map);
}

async function loadWorldMap() {
    const res = await fetch('/maps/countries.geo.json');
    const data = await res.json();

    const worldLayer = L.geoJSON(data, {
        style: {
            color: '#334155',
            weight: 1,
            fillColor: '#f8fafc',
            fillOpacity: 1
        },
        onEachFeature: function (feature, layer) {
            if (feature.properties && feature.properties.name) {
                layer.bindTooltip(feature.properties.name, {
                    sticky: true,
                    direction: 'auto'
                });
            }

            layer.on({
                mouseover: function (e) {
                    e.target.setStyle({
                        weight: 2,
                        color: '#0f172a',
                        fillColor: '#e2e8f0'
                    });
                },
                mouseout: function (e) {
                    worldLayer.resetStyle(e.target);
                }
            });
        }
    });

    countryLayerGroup.addLayer(worldLayer);

    const bounds = worldLayer.getBounds();
    if (bounds.isValid()) {
        map.fitBounds(bounds, { padding: [25, 25] });
    } else {
        map.setView([20, 0], 2);
    }
}

async function loadPoints() {
    const res = await fetch('/api/map/points');
    const points = await res.json();

    points.forEach(p => {
        const marker = L.marker([p.lat, p.lng], {
            icon: getMarkerIcon(p.type)
        }).addTo(pointLayerGroup);

        marker.bindPopup(`
            <b>${p.name}</b><br/>
            Type: ${p.type}<br/>
            Lat: ${p.lat}<br/>
            Lng: ${p.lng}
        `);

        marker.bindTooltip(p.name, {
            direction: 'top',
            offset: [0, -8],
            className: 'map-label'
        });
    });
}

async function loadRoutes() {
    const res = await fetch('/api/map/routes');
    const routes = await res.json();

    routes.forEach(r => {
        L.polyline(
            [[r.fromLat, r.fromLng], [r.toLat, r.toLng]],
            {
                color: routeColor(r.mode),
                weight: 4,
                opacity: 0.9,
                dashArray: r.mode === 'Sea' ? '8, 8' : null
            }
        ).addTo(routeLayerGroup).bindPopup(`
            <b>${r.shipmentNo}</b><br/>
            Mode: ${r.mode}
        `);
    });
}

(async function init() {
    addLegend();
    await loadWorldMap();
    await loadPoints();
    await loadRoutes();
})();
