// ═══════════════════════════════════════════════════════════
// GLOBAL STATE
// ═══════════════════════════════════════════════════════════
let selectedFlightId = null;
let selectedFlightNumber = null;

// ═══════════════════════════════════════════════════════════
// FLIGHT SELECTION (Parent Grid)
// ═══════════════════════════════════════════════════════════
function selectFlight(flightId, flightNumber) {
    selectedFlightId = flightId;
    selectedFlightNumber = flightNumber;

    // Show flight dates section
    document.getElementById('flight-dates-section').style.display = 'block';

    // Update button
    const btnNewFlightDate = document.getElementById('btn-new-flight-date');
    btnNewFlightDate.style.display = 'inline-block';
    btnNewFlightDate.setAttribute('hx-get', `/Flight/CreateFlightDate?flightId=${flightId}`);
    btnNewFlightDate.setAttribute('hx-target', '#modal-container');
    btnNewFlightDate.setAttribute('onclick', 'showModal()');
    htmx.process(btnNewFlightDate);

    // Load flight dates grid
    htmx.ajax('GET', `/Flight/LoadFlightDateGrid?flightId=${flightId}`, {
        target: '#flight-date-grid-container',
        swap: 'innerHTML'
    });

    // Scroll to flight dates section
    document.getElementById('flight-dates-section').scrollIntoView({ behavior: 'smooth', block: 'start' });
}

// ═══════════════════════════════════════════════════════════
// DELETE CONFIRMATIONS
// ═══════════════════════════════════════════════════════════
function deleteFlight(flightId) {
    if (confirm('Are you sure you want to delete this flight?')) {
        const formData = new FormData();
        formData.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

        htmx.ajax('POST', `/Flight/DeleteFlight?id=${flightId}`, {
            target: '#modal-container',
            swap: 'innerHTML',
            values: Object.fromEntries(formData)
        });
    }
}

function deleteFlightDate(flightDateId) {
    if (confirm('Are you sure you want to delete this flight date?')) {
        const formData = new FormData();
        formData.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

        htmx.ajax('POST', `/Flight/DeleteFlightDate?id=${flightDateId}`, {
            target: '#modal-container',
            swap: 'innerHTML',
            values: Object.fromEntries(formData)
        });
    }
}

// ═══════════════════════════════════════════════════════════
// MODAL MANAGEMENT
// ═══════════════════════════════════════════════════════════
function showModal() {
    setTimeout(() => {
        const modal = document.querySelector('.modal');
        if (modal) {
            modal.classList.add('show');
            modal.style.display = 'block';
            document.body.classList.add('modal-open');

            // Create backdrop if not exists
            if (!document.querySelector('.modal-backdrop')) {
                const backdrop = document.createElement('div');
                backdrop.className = 'modal-backdrop fade show';
                document.body.appendChild(backdrop);
            }
        }
    }, 100);
}

function closeModal() {
    const modal = document.querySelector('.modal');
    if (modal) {
        modal.classList.remove('show');
        modal.style.display = 'none';
    }

    const backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) {
        backdrop.remove();
    }

    document.body.classList.remove('modal-open');
    document.getElementById('modal-container').innerHTML = '';
}

// ═══════════════════════════════════════════════════════════
// HTMX EVENT LISTENERS
// ═══════════════════════════════════════════════════════════
document.addEventListener('DOMContentLoaded', function () {

    // Listen for flight changes (create/edit/delete)
    document.body.addEventListener('flightChanged', function () {
        closeModal();
        htmx.trigger('#flight-grid-container', 'flightChanged');

        // Clear selection
        selectedFlightId = null;
        selectedFlightNumber = null;
        document.getElementById('flight-dates-section').style.display = 'none';
        document.getElementById('flight-date-grid-container').innerHTML = '<p class="text-muted text-center py-4">Select a flight to view dates</p>';
    });

    // Listen for flight date changes
    document.body.addEventListener('flightDateChanged', function () {
        closeModal();

        // Reload flight dates grid if a flight is selected
        if (selectedFlightId) {
            htmx.ajax('GET', `/Flight/LoadFlightDateGrid?flightId=${selectedFlightId}`, {
                target: '#flight-date-grid-container',
                swap: 'innerHTML'
            });
        }
    });

    // Close modal on ESC key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            closeModal();
        }
    });
});
