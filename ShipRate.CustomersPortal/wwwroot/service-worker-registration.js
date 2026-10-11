//if ("serviceWorker" in navigator) {
//    window.addEventListener("load", () => {
//        navigator.serviceWorker
//            .register("/service-worker.js")
//            .then(() => console.log("Service Worker Registered"))
//            .catch(err => console.log("SW registration failed: ", err));
//    });
//}




if ("serviceWorker" in navigator) {
    window.addEventListener("load", async () => {
        try {
            const registration = await navigator.serviceWorker.register("/service-worker.js", {
                scope: "/",
                updateViaCache: "none"
            });

            await registration.update();

            console.log("Service Worker registered successfully:", registration);
        } catch (error) {
            console.error("Service Worker registration failed:", error);
        }
    });
}

