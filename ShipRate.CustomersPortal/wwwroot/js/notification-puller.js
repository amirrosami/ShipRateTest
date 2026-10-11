(function () {
    const POLL_INTERVAL_MS = 10000;

    function tick() {
        htmx.trigger(document.body, 'notificationChanged');
        htmx.trigger(document.body, 'messagingChanged');
        
    }

    const timerId = setInterval(tick, POLL_INTERVAL_MS);

    document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
            clearInterval(timerId);
        } else {
            tick();
        }
    });
})();