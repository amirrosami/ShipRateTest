// ══════════════════════════════════════════════════════════════
// Global Variables
// ══════════════════════════════════════════════════════════════
let currentVoyageId = null;
let currentVoyageLabel = '';
let currentVoyagePortId = null;
let currentVoyagePortLabel = '';
const modal = new bootstrap.Modal(document.getElementById('formModal'));

// ══════════════════════════════════════════════════════════════
// Modal Handling
// ══════════════════════════════════════════════════════════════
function showModal(title) {
    document.getElementById('modalTitle').innerText = title;
    modal.show();
}

document.getElementById('formModal').addEventListener('hidden.bs.modal', () => {
    document.getElementById('modalTitle').innerText = 'Form';
    document.getElementById('modalBody').innerHTML =
        '<div class="text-center py-5"><i class="bi bi-hourglass-split fa-spin"></i> Loading...</div>';
});

// ══════════════════════════════════════════════════════════════
// Voyage Selection (Level 1 → Level 2)
// ══════════════════════════════════════════════════════════════
function selectVoyage(id, label) {
    currentVoyageId = id;
    currentVoyageLabel = label;
    currentVoyagePortId = null;
    currentVoyagePortLabel = '';

    // Update header labels
    document.getElementById('voyagePortLabel').innerText = `(${label})`;
    document.getElementById('voyagePortEventLabel').innerText = '';

    // Enable/disable "New" buttons
    document.getElementById('btnNewVoyagePort').disabled = false;
    document.getElementById('btnNewVoyagePort').setAttribute(
        'hx-get', `/Voyage/CreateVoyagePort?voyageId=${id}`
    );
    htmx.process(document.getElementById('btnNewVoyagePort'));

    document.getElementById('btnNewVoyagePortEvent').disabled = true;

    // Clear event grid
    document.getElementById('voyagePortEventGridContainer').innerHTML =
        '<p class="text-muted text-center py-3 mb-0"><i class="bi bi-arrow-up-circle me-1"></i>Select a Port to view its Events</p>';

    // Load Voyage Ports via HTMX
    htmx.ajax('GET', `/Voyage/VoyagePortList?voyageId=${id}`, {
        target: '#voyagePortGridContainer',
        swap: 'innerHTML'
    });

    // Highlight selected row
    document.querySelectorAll('#voyageGridContainer tbody tr').forEach(r => r.classList.remove('table-active'));
    event?.target?.closest('tr')?.classList.add('table-active');
}

// ══════════════════════════════════════════════════════════════
// Voyage Port Selection (Level 2 → Level 3)
// ══════════════════════════════════════════════════════════════
function selectVoyagePort(id, label) {
    currentVoyagePortId = id;
    currentVoyagePortLabel = label;

    // Enable "New Event" button and set its hx-get
    document.getElementById('btnNewVoyagePortEvent').disabled = false;
    document.getElementById('btnNewVoyagePortEvent').setAttribute(
        'hx-get', `/Voyage/CreateVoyagePortEvent?voyagePortId=${id}`
    );
    htmx.process(document.getElementById('btnNewVoyagePortEvent'));

    // Update label
    document.getElementById('voyagePortEventLabel').innerText = `(${label})`;

    // Load Voyage Port Events via HTMX
    htmx.ajax('GET', `/Voyage/VoyagePortEventList?voyagePortId=${id}`, {
        target: '#voyagePortEventGridContainer',
        swap: 'innerHTML'
    });

    // Highlight selected row
    document.querySelectorAll('#voyagePortGridContainer tbody tr').forEach(r => r.classList.remove('table-active'));
    event?.target?.closest('tr')?.classList.add('table-active');
}

// ══════════════════════════════════════════════════════════════
// HTMX Event Listeners — Auto-refresh grids after CUD
// ══════════════════════════════════════════════════════════════

// After Voyage CRUD → refresh voyage grid
document.body.addEventListener('voyageChanged', function () {
    modal.hide();
    htmx.ajax('GET', `/Voyage/Index?page=1`, {
        target: '#voyageGridContainer',
        swap: 'innerHTML'
    });
});

// After VoyagePort CRUD → refresh port grid
document.body.addEventListener('voyagePortChanged', function () {
    modal.hide();
    if (currentVoyageId) {
        htmx.ajax('GET', `/Voyage/VoyagePortList?voyageId=${currentVoyageId}`, {
            target: '#voyagePortGridContainer',
            swap: 'innerHTML'
        });
    }
    // Clear events grid (selected port may have been deleted)
    document.getElementById('voyagePortEventGridContainer').innerHTML =
        '<p class="text-muted text-center py-3 mb-0"><i class="bi bi-arrow-up-circle me-1"></i>Select a Port to view its Events</p>';
    document.getElementById('btnNewVoyagePortEvent').disabled = true;
    document.getElementById('voyagePortEventLabel').innerText = '';
    currentVoyagePortId = null;
});

// After VoyagePortEvent CRUD → refresh event grid
document.body.addEventListener('voyagePortEventChanged', function () {
    modal.hide();
    if (currentVoyagePortId) {
        htmx.ajax('GET', `/Voyage/VoyagePortEventList?voyagePortId=${currentVoyagePortId}`, {
            target: '#voyagePortEventGridContainer',
            swap: 'innerHTML'
        });
    }
});

// ══════════════════════════════════════════════════════════════
// HTMX After-Swap: Close modal on success response
// ══════════════════════════════════════════════════════════════
document.body.addEventListener('htmx:afterSwap', function (evt) {
    // If the response contains a success indicator, the custom events
    // above will handle closing. This handles form re-render on validation error.
    if (evt.detail.target.id === 'modalBody') {
        // Re-process HTMX attributes in new modal content
        htmx.process(evt.detail.target);
    }
});

// ══════════════════════════════════════════════════════════════
// Delete Confirmation Enhancement
// ══════════════════════════════════════════════════════════════
document.body.addEventListener('htmx:confirm', function (evt) {
    // Only intercept elements with hx-confirm
    if (!evt.detail.elt.hasAttribute('hx-confirm')) return;

    evt.preventDefault();

    const message = evt.detail.elt.getAttribute('hx-confirm');
    if (confirm(message)) {
        evt.detail.issueRequest(true);
    }
});
