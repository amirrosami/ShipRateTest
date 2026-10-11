//const CACHE_VERSION = "v1";
//const CACHE_NAME = `customerportal-cache-${CACHE_VERSION}`;

//const APP_SHELL = [
//    "/",
//    "/css/site.css",
//    "/js/site.js",
//    "/offline.html"
//];

//// نصب (install)
//self.addEventListener("install", event => {
//    event.waitUntil(
//        caches.open(CACHE_NAME).then(cache => {
//            return cache.addAll(APP_SHELL);
//        })
//    );
//    self.skipWaiting();
//});

//// فعال‌سازی (activate)
//self.addEventListener("activate", event => {
//    event.waitUntil(
//        caches.keys().then(keys => {
//            return Promise.all(
//                keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))
//            );
//        })
//    );
//    self.clients.claim();
//});

//// دریافت درخواست‌ها (fetch)
//self.addEventListener("fetch", event => {
//    event.respondWith(
//        caches.match(event.request)
//            .then(response => {
//                return response || fetch(event.request)
//                    .catch(() => caches.match("/offline.html"));
//            })
//    );
//});




const CACHE_VERSION = "v2";
const STATIC_CACHE = `customerportal-static-${CACHE_VERSION}`;

const PRECACHE_ASSETS = [
    "/offline.html",
    "/css/site.css",
    "/js/site.js"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(STATIC_CACHE).then(cache => cache.addAll(PRECACHE_ASSETS))
    );
    self.skipWaiting();
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(names =>
            Promise.all(
                names
                    .filter(name => name !== STATIC_CACHE)
                    .map(name => caches.delete(name))
            )
        )
    );
    self.clients.claim();
});

self.addEventListener("fetch", event => {
    const request = event.request;
    const url = new URL(request.url);

    if (request.method !== "GET") {
        return;
    }

    if (url.origin !== self.location.origin) {
        return;
    }

    // Never cache auth, identity, api, or admin routes.
    if (
        url.pathname.startsWith("/Account") ||
        url.pathname.startsWith("/Identity") ||
        url.pathname.startsWith("/api") ||
        url.pathname.startsWith("/Admin")
    ) {
        return;
    }

    // Always go to network first for page navigations so auth cookies
    // and server-side authorization are evaluated on every request.
    if (request.mode === "navigate") {
        event.respondWith(
            fetch(request).catch(() => caches.match("/offline.html"))
        );
        return;
    }

    // Cache only static assets.
    if (isStaticAsset(url.pathname)) {
        event.respondWith(handleStaticAsset(event));
    }
});

function isStaticAsset(pathname) {
    return (
        pathname.startsWith("/css/") ||
        pathname.startsWith("/js/") ||
        pathname.startsWith("/images/") ||
        pathname.startsWith("/lib/") ||
        pathname === "/favicon.ico"
    );
}

async function handleStaticAsset(event) {
    const request = event.request;
    const cache = await caches.open(STATIC_CACHE);
    const cached = await cache.match(request);

    const networkFetch = fetch(request)
        .then(response => {
            if (response.ok && response.type === "basic") {
                cache.put(request, response.clone());
            }
            return response;
        });

    if (cached) {
        event.waitUntil(networkFetch.catch(() => { }));
        return cached;
    }

    try {
        return await networkFetch;
    } catch {
        return new Response("Resource unavailable.", {
            status: 503,
            headers: { "Content-Type": "text/plain" }
        });
    }
}
