document.body.addEventListener('htmx:afterRequest', function (event) {
  if (!event.detail.successful) return;
  const trigger = event.detail.elt;
  if (trigger.classList.contains('tab')) {
    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    trigger.classList.add('active');
  }
});

document.body.addEventListener('htmx:afterSwap', function (event) {
  if (event.detail.target.id === 'shipment-content') {
    const firstTab = document.querySelector('#shipment-tabs .tab');
    if (firstTab) firstTab.classList.add('active');
  }
});
