// wwwroot/js/site.js

// ══════════════════════════════════════════════
//  MODAL MANAGEMENT (یکپارچه - فقط یک instance)
// ══════════════════════════════════════════════

let modalInstance = null;
let modalInstance2 = null;

function getModal() {
    const el = document.getElementById('appModal');
    if (!el) return null;
    if (!modalInstance) {
        modalInstance = new bootstrap.Modal(el, { keyboard: false, backdrop: 'static' });
    }
    return modalInstance;
}

function getModal2() {
    const el = document.getElementById('secondModal');
    if (!el) return null;
    if (!modalInstance2) {
        modalInstance2 = new bootstrap.Modal(el, { keyboard: false, backdrop: 'static' });
    }
    return modalInstance2;
}

function showModal() { const m = getModal(); if (m) m.show(); }
function showModal2() { const m = getModal2(); if (m) m.show(); }
function hideModal() { const m = getModal(); if (m) m.hide(); }
function hideModal2() { const m = getModal2(); if (m) m.hide(); }

function closeModal() {
    hideModal();
}

// ══════════════════════════════════════════════
//  HTMX EVENT HANDLERS
// ══════════════════════════════════════════════

document.body.addEventListener('closeModal', function () { hideModal(); });
document.body.addEventListener('closeModal2', function () { hideModal2(); });

// ★ refreshList عمومی
document.body.addEventListener('refreshList', function (evt) {
    const detail = evt.detail || {};
    const url = detail.url || '';
    if (url) {
        const searchInput = document.getElementById('searchInput');
        const searchVal = searchInput ? searchInput.value : '';
        htmx.ajax('GET', url + '?searchterm=' + encodeURIComponent(searchVal), {
            target: '#table-container',
            swap: 'innerHTML'
        });
    }
});

// ★ refresh های اختصاصی
const refreshEvents = {
    'refreshBanksList': '/Banks/List',
    'refreshPersonsList': '/Persons/List',
    'refreshLocationsList': '/LocationsV2/List',
    'refreshIndustryList': '/Industry/List',
    'refreshInventoryList': '/Inventory/List',
    'refreshAssetTypesList': '/AssetType/List',
    'refreshPortDistanceList': '/PortDistance/List',
    'refreshProductCategoryList': '/ProductCategory/List',
    'refreshBlackListReasonList': '/BlackListReason/List',
    'refreshVesselsList': '/Vessels/List',
    'refreshProductsList': '/Products/List',
    'refreshExchangeRatesList': '/ExchangeRates/List'
};

Object.entries(refreshEvents).forEach(([eventName, url]) => {
    document.body.addEventListener(eventName, function () {
        const searchInput = document.getElementById('searchInput');
        const searchVal = searchInput ? searchInput.value : '';
        htmx.ajax('GET', url + '?searchterm=' + encodeURIComponent(searchVal), {
            target: '#table-container',
            swap: 'innerHTML'
        });
    });
});

// ══════════════════════════════════════════════
//  CUSTOMERS - HTMX-First approach
// ══════════════════════════════════════════════

// ★ بستن Modal و رفرش لیست مشتریان — trigger از سرور
document.body.addEventListener('closeModalRefreshCustomers', function (evt) {
    hideModal();
    document.getElementById('modal-content').innerHTML = '';
    refreshCustomerList();
    const detail = evt.detail || {};
    if (detail.message) {
        setTimeout(() => createToast(detail.message, detail.type || 'success'), 500);
    }

});

document.body.addEventListener('closeSecondModalRefreshCustomers', function () {
    hideModal2();
    document.getElementById('modal-content2').innerHTML = '';
    refreshCustomerList();

});



//document.body.addEventListener('refreshVesselsList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Vessels/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});



function refreshCustomerList() {
    const form = document.getElementById('customerSearchForm');
    //alert(form);
    if (form) {
        htmx.trigger(form, 'submit')
    }
}

// ★ صفحه‌بندی مشتریان
function goToPage(page) {
    const pageInput = document.getElementById('PageNumber');
    if (pageInput) {
        pageInput.value = page;
        refreshCustomerList();
    }
}






// ══════════════════════════════════════════════
//  TOAST NOTIFICATIONS
// ══════════════════════════════════════════════

//document.body.addEventListener('showToast', function (evt) {
//    const detail = evt.detail || {};
//    createToast(detail.message || 'Done.', detail.type || 'info');
//});


document.body.addEventListener('showToast', function (evt) {
    const detail = evt.detail || {};
    const message = detail.message || 'Done.';
    const type = detail.type || 'info';
    createToast(message, type);
});




function createToast(message, type) {
    const toastEl = document.getElementById('appToast');
    if (!toastEl) {
        console.error("Toast element #appToast not found!");
        return;
    }

    // 1. بازنشانی کلاس‌ها و محتوا از نمایش قبلی
    toastEl.className = 'toast align-items-center border-0';
    const toastBody = toastEl.querySelector('.toast-body');
    if (toastBody) {
        toastBody.innerHTML = '';
    }



    // 2. تنظیم کلاس‌ها و محتوای جدید
    const icons = {
        success: 'bi-check-circle',
        danger: 'bi-x-circle',
        warning: 'bi-exclamation-triangle',
        info: 'bi-info-circle'
    };
    const bgColors = {
        success: 'text-bg-success',
        danger: 'text-bg-danger',
        warning: 'text-bg-warning',
        info: 'text-bg-info'
    };

    const iconClass = icons[type] || icons.info;
    const bgClass = bgColors[type] || bgColors.info;

    if (toastBody) {
        toastBody.innerHTML = `<i class="bi ${iconClass} me-2"></i>${message}`;
    }
    toastEl.classList.add(bgClass);




    // 3. نمایش مجدد
    const toast = new bootstrap.Toast(toastEl, { delay: 5000 });
    toast.show();
}






// ══════════════════════════════════════════════
//  HTMX GLOBAL CONFIG
// ══════════════════════════════════════════════

document.body.addEventListener('htmx:afterSwap', function (evt) {
    const targetId = evt.detail.target?.id;

    // ★ Modal content لود شد → re-process HTMX + باز کردن Modal مربوطه
    if (targetId === 'modal-content') {
        htmx.process(evt.detail.target);
        showModal();
    }
    if (targetId === 'modal-content2') {
        htmx.process(evt.detail.target);
        showModal2();
    }

    // لیست رفرش شد → Modal رو ببند (برای CustomerConfirmation)
    if (targetId === 'listContainer') {
        closeModal();
    }

    // Picture upload section
    if (targetId === 'modal-content' || targetId === 'app-modal-content' || targetId === 'pictureSection') {
        setTimeout(function () { initPictureUpload(); }, 150);
    }
});

// Handle 422 validation errors
document.body.addEventListener('htmx:beforeSwap', function (evt) {
    if (evt.detail.xhr.status === 422) {
        evt.detail.shouldSwap = true;
        evt.detail.isError = false;
    }
});

// ══════════════════════════════════════════════
//  CUSTOMER CONFIRMATION - Modal Openers
// ══════════════════════════════════════════════

function openCreateForm() {
    fetch('/CustomerConfirmation/Create')
        .then(r => r.text())
        .then(html => {
            document.getElementById('modal-content').innerHTML = html;
            htmx.process(document.getElementById('modal-content'));
            showModal();
            initPersonSearch();
        });
}

function openEditForm(id) {
    fetch('/CustomerConfirmation/Edit?id=' + id)
        .then(r => r.text())
        .then(html => {
            document.getElementById('modal-content').innerHTML = html;
            htmx.process(document.getElementById('modal-content'));
            showModal();
            initPersonSearch();
        });
}

function openConfirmReject(id) {
    fetch('/CustomerConfirmation/ConfirmReject?id=' + id)
        .then(r => r.text())
        .then(html => {
            document.getElementById('modal-content').innerHTML = html;
            htmx.process(document.getElementById('modal-content'));
            showModal();
        });
}

function openDeleteForm(id) {
    fetch('/CustomerConfirmation/Delete?id=' + id)
        .then(r => r.text())
        .then(html => {
            document.getElementById('modal-content').innerHTML = html;
            htmx.process(document.getElementById('modal-content'));
            showModal();
        });
}

// ══════════════════════════════════════════════
//  SEARCH (CustomerConfirmation)
// ══════════════════════════════════════════════

function clearSearch() {
    const nameInput = document.querySelector('#searchForm [name="searchName"]');
    const statusInput = document.querySelector('#searchForm [name="searchStatus"]');
    if (nameInput) nameInput.value = '';
    if (statusInput) statusInput.value = 'All';
    htmx.ajax('GET', '/CustomerConfirmation/List', { target: '#listContainer' });
}

// ══════════════════════════════════════════════
//  PERSON AUTOCOMPLETE
// ══════════════════════════════════════════════

function initPersonSearch() {
    const input = document.getElementById('personSearchInput');
    const dropdown = document.getElementById('personDropdown');
    const hiddenInput = document.getElementById('personIdHidden');
    const selectedBadge = document.getElementById('selectedPersonBadge');
    const selectedName = document.getElementById('selectedPersonName');

    if (!input) return;

    let debounceTimer;

    input.addEventListener('input', function () {
        const term = this.value.trim();
        clearTimeout(debounceTimer);

        if (term.length < 2) {
            dropdown.classList.remove('show');
            dropdown.innerHTML = '';
            return;
        }

        debounceTimer = setTimeout(() => {
            fetch('/CustomerConfirmation/SearchPerson?term=' + encodeURIComponent(term))
                .then(r => r.json())
                .then(data => {
                    dropdown.innerHTML = '';
                    if (data.length === 0) {
                        dropdown.innerHTML = '<li><span class="dropdown-item text-muted">No results</span></li>';
                    } else {
                        data.forEach(p => {
                            const li = document.createElement('li');
                            li.innerHTML = `<button type="button" class="dropdown-item" 
                                data-id="${p.id}" data-name="${p.fullName}">${p.fullName}</button>`;
                            li.querySelector('button').addEventListener('click', function () {
                                selectPerson(this.dataset.id, this.dataset.name);
                            });
                            dropdown.appendChild(li);
                        });
                    }
                    dropdown.classList.add('show');
                });
        }, 300);
    });

    document.addEventListener('click', function (e) {
        if (!input.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

    function selectPerson(id, name) {
        hiddenInput.value = id;
        input.value = '';
        dropdown.classList.remove('show');
        selectedBadge.classList.remove('d-none');
        selectedName.textContent = name;
    }

    const clearBtn = document.getElementById('clearPersonBtn');
    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            hiddenInput.value = '';
            selectedBadge.classList.add('d-none');
            selectedName.textContent = '';
        });
    }
}

// ══════════════════════════════════════════════
//  PICTURE UPLOAD
// ══════════════════════════════════════════════

function initPictureUpload() {
    const container = document.getElementById('pictureContainer');
    const fileInput = document.getElementById('pictureFileInput');
    if (!container || !fileInput) return;

    const newContainer = container.cloneNode(true);
    container.parentNode.replaceChild(newContainer, container);
    const newFileInput = fileInput.cloneNode(true);
    fileInput.parentNode.replaceChild(newFileInput, fileInput);

    newContainer.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        newFileInput.click();
    });

    newFileInput.addEventListener('change', async function (e) {
        const file = e.target.files[0];
        if (!file) return;
        const personId = newContainer.dataset.personId;
        if (!personId) return;

        newContainer.innerHTML = '<div class="spinner-border"></div><p>Uploading...</p>';
        const formData = new FormData();
        formData.append('file', file);
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        try {
            const response = await fetch(`/Persons/UploadPicture?personId=${personId}`, {
                method: 'POST',
                body: formData,
                headers: token ? { 'RequestVerificationToken': token } : {}
            });
            if (response.ok) {
                htmx.ajax('GET', `/Persons/GetPicturePartial?personId=${personId}`, {
                    target: '#pictureSection', swap: 'innerHTML'
                });
            }
        } catch (err) {
            console.error(err);
            alert('Upload error');
        }
        newFileInput.value = '';
    });
}


// Refresh the table list
document.body.addEventListener('refreshList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';

    const pageSize = document.getElementById('pageSize');
    console.log(pageSize);
    const pageSizeVal = pageSize ? pageSize.value : 10;


    htmx.ajax('GET', '/PackageTypes/List?searchterm=' + encodeURIComponent(searchVal) + '&&pageSize=' + encodeURIComponent(pageSizeVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});




// Refresh the table list
document.body.addEventListener('refreshLocationsList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/LocationsV2/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});


document.body.addEventListener('refreshBanksList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Banks/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshPersonsList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Persons/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshIndustryList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Industry/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});
document.body.addEventListener('refreshInventoryList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Inventory/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});


document.body.addEventListener('refreshAssetTypesList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/AssetType/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});


document.body.addEventListener('refreshPortDistanceList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/PortDistance/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshProductCategoryList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/ProductCategory/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshBlackListReasonList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/BlackListReason/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshVesselsList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Vessels/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});

document.body.addEventListener('refreshProductsList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/Products/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});


document.body.addEventListener('refreshExchangeRatesList', function () {
    const searchInput = document.getElementById('searchInput');
    const searchVal = searchInput ? searchInput.value : '';


    htmx.ajax('GET', '/ExchangeRates/List?searchterm=' + encodeURIComponent(searchVal), {
        target: '#table-container',
        swap: 'innerHTML'
    });
});



//function formatMoneyInput(input) {
//    let value = input.value.replace(/,/g, '').replace(/[^\d.]/g, '');

//    const parts = value.split('.');
//    let integerPart = parts[0];
//    let decimalPart = parts.length > 1 ? '.' + parts.slice(1).join('') : '';

//    integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

//    input.value = integerPart + decimalPart;
//}

//document.addEventListener('input', function (e) {
//    if (e.target.classList.contains('money-input')) {
//        formatMoneyInput(e.target);
//    }
//});

//document.addEventListener('submit', function (e) {
//    e.target.querySelectorAll('.money-input').forEach(function (input) {
//        input.value = input.value.replace(/,/g, '');
//    });
//});

//document.body.addEventListener('htmx:configRequest', function (e) {
//    document.querySelectorAll('.money-input').forEach(function (input) {
//        input.value = input.value.replace(/,/g, '');
//    });
//});

// ۱. تابع فرمت کردن یک اینپوت با حفظ موقعیت مکان‌نما (Caret)
function formatMoneyInput(input) {
    if (!input || !input.value) return;

    const cursorPosition = input.selectionStart;
    const oldLength = input.value.length;

    // حذف کاماها و کاراکترهای غیرعددی (به جز نقطه اعشار)
    let value = input.value.replace(/,/g, '').replace(/[^\d.]/g, '');

    const parts = value.split('.');
    let integerPart = parts[0] || '';
    let decimalPart = parts.length > 1 ? '.' + parts.slice(1).join('') : '';

    // جداکننده ۳ رقم
    integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    const formattedValue = integerPart + decimalPart;

    if (input.value !== formattedValue) {
        input.value = formattedValue;

        // تنظیم مجدد جایگاه مکان‌نما
        if (document.activeElement === input) {
            const newLength = formattedValue.length;
            const newCursorPosition = Math.max(0, cursorPosition + (newLength - oldLength));
            input.setSelectionRange(newCursorPosition, newCursorPosition);
        }
    }
}

// ۲. فرمت کردن تمام اینپوت‌های پولی در یک کانتینر
function formatAllMoneyInputs(container = document) {
    container.querySelectorAll('.money-input').forEach(function (input) {
        formatMoneyInput(input);
    });
}

// ۳. گوش دادن به رویداد تایپ کاربر
document.addEventListener('input', function (e) {
    if (e.target && e.target.classList && e.target.classList.contains('money-input')) {
        formatMoneyInput(e.target);
    }
});

// ۴. فرمت کردن خودکار هنگام لود اولیه صفحه
document.addEventListener('DOMContentLoaded', function () {
    formatAllMoneyInputs(document);
});

// ۵. فرمت کردن مجدد پس از هر بار تعویض یا لود بخش‌هایی با HTMX (مثل باز شدن مودال یا رفرش پارشیال)
document.body.addEventListener('htmx:afterSwap', function (e) {
    formatAllMoneyInputs(e.detail.target);
});

document.body.addEventListener('htmx:afterSettle', function (e) {
    formatAllMoneyInputs(e.detail.target);
});



//Show toast notification
//document.body.addEventListener('showToast', function (evt) {
//    const detail = evt.detail || {};
//    const message = detail.message || 'Done.';
//    const type = detail.type || 'info';
//    createToast(message, type);
//});



















//// wwwroot/js/site.js

//// ══════════════════════════════════════════════
////  MODAL MANAGEMENT
//// ══════════════════════════════════════════════

//let modalInstance = null;
//let modalInstance2 = null;
//let formModal;


//function getModal() {
//    const el = document.getElementById('appModal');
//    if (!el) return null;
//    if (!modalInstance) {
//        modalInstance = new bootstrap.Modal(el, { keyboard: false, backdrop: 'static' });
//    }
//    return modalInstance;
//}


//function getModal2() {
//    const el = document.getElementById('secondModal');
//    if (!el) return null;
//    if (!modalInstance2) {
//        modalInstance2 = new bootstrap.Modal(el, { keyboard: false, backdrop: 'static' });
//    }
//    return modalInstance2;
//}



//// Called by onclick on buttons that load modal content
//function showModal() {
//    const m = getModal();
//    if (m) m.show();
//}
//function showModal2() {
//    const m = getModal2();
//    if (m) m.show();
//}

//function hideModal() {
//    const m = getModal();
//    if (m) m.hide();
//}
//function hideModal2() {
//    const m = getModal2();
//    if (m) m.hide();
//}

//// Cleanup modal content after it's fully hidden
////document.addEventListener('DOMContentLoaded', function () {
////    const modalEl = document.getElementById('appModal');
////    if (modalEl) {
////        modalEl.addEventListener('hidden.bs.modal', function () {
////            const content = document.getElementById('modal-content');
////            if (content) {
////                content.innerHTML = '';
////            //        '<div class="modal-body text-center py-5">' +
////            //        '<div class="spinner-border text-primary" role="status"></div>' +
////            //        '</div>';
////            }
////        });
////    }
////});


//// ══════════════════════════════════════════════
////  HTMX EVENT HANDLERS
//// ══════════════════════════════════════════════

//// Close modal when server sends HX-Trigger: closeModal
//document.body.addEventListener('closeModal', function () {
//    hideModal();
//});

//document.body.addEventListener('closeModal2', function () {
//    hideModal2();
//});

//// Refresh the table list
//document.body.addEventListener('refreshList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/PackageTypes/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});


//document.body.addEventListener('refreshBanksList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Banks/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});
//document.body.addEventListener('refreshIndustryList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Industry/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});
//document.body.addEventListener('refreshInventoryList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Inventory/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});


//document.body.addEventListener('refreshAssetTypesList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/AssetType/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});


//document.body.addEventListener('refreshPortDistanceList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/PortDistance/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});

//document.body.addEventListener('refreshProductCategoryList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/ProductCategory/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});

//document.body.addEventListener('refreshBlackListReasonList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/BlackListReason/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});



//document.body.addEventListener('refreshVesselsList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Vessels/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});

//document.body.addEventListener('refreshProductsList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/Products/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});

//document.body.addEventListener('refreshExchangeRatesList', function () {
//    const searchInput = document.getElementById('searchInput');
//    const searchVal = searchInput ? searchInput.value : '';


//    htmx.ajax('GET', '/ExchangeRates/List?searchterm=' + encodeURIComponent(searchVal), {
//        target: '#table-container',
//        swap: 'innerHTML'
//    });
//});


////Show toast notification
//document.body.addEventListener('showToast', function (evt) {
//    const detail = evt.detail || {};
//    const message = detail.message || 'Done.';
//    const type = detail.type || 'info';
//    createToast(message, type);
//});

////document.body.addEventListener('htmx:trigger', function (evt) {
////    if (evt.detail.name === 'showToast') {
////        const detail = evt.detail.parameters;
////        const message = detail.message || 'Done.';
////        const type = detail.type || 'info';
////        createToast(message, type);
////    }
////});



//// ══════════════════════════════════════════════
////  TOAST NOTIFICATIONS
//// ══════════════════════════════════════════════

//function createToast(message, type) {
//    const toastEl = document.getElementById('appToast');
//    if (!toastEl) {
//        console.error("Toast element #appToast not found!");
//        return;
//    }

//    // 1. بازنشانی کلاس‌ها و محتوا از نمایش قبلی
//    toastEl.className = 'toast align-items-center border-0';
//    const toastBody = toastEl.querySelector('.toast-body');
//    if (toastBody) {
//        toastBody.innerHTML = '';
//    }

//    // 2. تنظیم کلاس‌ها و محتوای جدید
//    const icons = {
//        success: 'bi-check-circle',
//        danger: 'bi-x-circle',
//        warning: 'bi-exclamation-triangle',
//        info: 'bi-info-circle'
//    };
//    const bgColors = {
//        success: 'text-bg-success',
//        danger: 'text-bg-danger',
//        warning: 'text-bg-warning',
//        info: 'text-bg-info'
//    };

//    const iconClass = icons[type] || icons.info;
//    const bgClass = bgColors[type] || bgColors.info;

//    if (toastBody) {
//        toastBody.innerHTML = `<i class="bi ${iconClass} me-2"></i>${message}`;
//    }
//    toastEl.classList.add(bgClass);

//    // 3. نمایش مجدد
//    const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
//    toast.show();
//}

//// ══════════════════════════════════════════════
////  HTMX GLOBAL CONFIG
//// ══════════════════════════════════════════════

//// Process new HTMX content inside modals (re-wire Bootstrap components)
//document.body.addEventListener('htmx:afterSwap', function (evt) {
//    // If we swapped into the modal content, re-init any needed Bootstrap components
//    if (evt.detail.target && evt.detail.target.id === 'modal-content') {
//        htmx.process(evt.detail.target);
//    }
//    if (evt.detail.target && evt.detail.target.id === 'modal-content') {
//        htmx.process(evt.detail.target);
//    }

//    else if (evt.detail.target && evt.detail.target.id === 'listContainer') {
//        closeModal();
//    }


//});

//// Handle non-200 responses — HTMX by default ignores non-2xx for swaps
//// We need 422 (validation errors) to still swap content
//document.body.addEventListener('htmx:beforeSwap', function (evt) {
//    if (evt.detail.xhr.status === 422) {
//        evt.detail.shouldSwap = true;
//        evt.detail.isError = false;
//    }
//});
//// ============================================
//// Setup بعد از HTMX swap
//// ============================================
//document.addEventListener('DOMContentLoaded', function () {
//    document.body.addEventListener('htmx:afterSwap', function (evt) {
//        const target = evt.detail.target;

//        if (target.id === 'modal-content' ||
//            target.id === 'pictureSection' ||
//            target.closest('#appModal')) {
//            setTimeout(function() {
//                console.log('1');
//                const container = document.getElementById('pictureContainer');
//                const fileInput = document.getElementById('pictureFileInput');

//                if (!container || !fileInput) return;



//                // حذف listener قبلی
//                const newContainer = container.cloneNode(true);
//                container.parentNode.replaceChild(newContainer, container);

//                const newFileInput = fileInput.cloneNode(true);
//                fileInput.parentNode.replaceChild(newFileInput, fileInput);


//                // کلیک روی باکس
//                newContainer.addEventListener('click', function (e) {
//                    e.preventDefault();
//                    e.stopPropagation();
//                    newFileInput.click();


//                });

//                // آپلود فایل
//                newFileInput.addEventListener('change', async function (e) {
//                    const file = e.target.files[0];
//                    if (!file) return;

//                    const personId = newContainer.dataset.personId;
//                    if (!personId) return;

//                    newContainer.innerHTML = '<div class="spinner-border"></div><p>در حال آپلود...</p>';

//                    const formData = new FormData();
//                    formData.append('file', file);

//                    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

//                    try {
//                        const response = await fetch(`/Persons/UploadPicture?personId=${personId}`, {
//                            method: 'POST',
//                            body: formData,
//                            headers: token ? { 'RequestVerificationToken': token } : {}
//                        });

//                        if (response.ok) {
//                            htmx.ajax('GET', `/Persons/GetPicturePartial?personId=${personId}`, {
//                                target: '#pictureSection',
//                                swap: 'innerHTML'
//                            });
//                        }
//                    } catch (err) {
//                        console.error(err);
//                        alert('خطا در آپلود');
//                    }

//                    newFileInput.value = '';
//                });
//            }, 150);
//        }
//    });
//});


//document.addEventListener('DOMContentLoaded', function () {
//    formModal = new bootstrap.Modal(document.getElementById('appModal'));
//});

//// ===== Open Create =====
//function openCreateForm() {
//    fetch('/CustomerConfirmation/Create')
//        .then(r => r.text())
//        .then(html => {
//            document.getElementById('modal-content').innerHTML = html;
//            formModal.show();
//            initPersonSearch();
//        });
//}

//// ===== Open Edit =====
//function openEditForm(id) {
//    fetch('/CustomerConfirmation/Edit?id=' + id)
//        .then(r => r.text())
//        .then(html => {
//            document.getElementById('modal-content').innerHTML = html;
//            formModal.show();
//            initPersonSearch();
//        });
//}

//// ===== Open Confirm/Reject =====
//function openConfirmReject(id) {
//    fetch('/CustomerConfirmation/ConfirmReject?id=' + id)
//        .then(r => r.text())
//        .then(html => {
//            document.getElementById('modal-content').innerHTML = html;
//            formModal.show();
//        });
//}

//// ===== Open Delete =====
//function openDeleteForm(id) {
//    fetch('/CustomerConfirmation/Delete?id=' + id)
//        .then(r => r.text())
//        .then(html => {
//            document.getElementById('modal-content').innerHTML = html;
//            formModal.show();
//        });
//}

//// ===== Close Modal =====
//function closeModal() {
//    formModal.hide();
//}

//// ===== Clear Search =====
//function clearSearch() {
//    document.querySelector('#searchForm [name="searchName"]').value = '';
//    document.querySelector('#searchForm [name="searchStatus"]').value = 'All';
//    htmx.ajax('GET', '/CustomerConfirmation/List', { target: '#listContainer' });
//}

//// ===== Handle HTMX after swap (close modal & refresh) =====
//// document.body.addEventListener('htmx:afterSwap', function (evt) {
////     if (evt.detail.target.id === 'listContainer') {
////         closeModal();
////     }
//// });

//// ============================================================
////  Person Autocomplete Search
//// ============================================================
//function initPersonSearch() {
//    const input = document.getElementById('personSearchInput');
//    const dropdown = document.getElementById('personDropdown');
//    const hiddenInput = document.getElementById('personIdHidden');
//    const selectedBadge = document.getElementById('selectedPersonBadge');
//    const selectedName = document.getElementById('selectedPersonName');

//    if (!input) return;

//    let debounceTimer;

//    input.addEventListener('input', function () {
//        const term = this.value.trim();
//        clearTimeout(debounceTimer);

//        if (term.length < 2) {
//            dropdown.classList.remove('show');
//            dropdown.innerHTML = '';
//            return;
//        }

//        debounceTimer = setTimeout(() => {
//            fetch('/CustomerConfirmation/SearchPerson?term=' + encodeURIComponent(term))
//                .then(r => r.json())
//                .then(data => {
//                    dropdown.innerHTML = '';
//                    if (data.length === 0) {
//                        dropdown.innerHTML = '<li><span class="dropdown-item text-muted">No results found</span></li>';
//                    } else {
//                        data.forEach(p => {
//                            const li = document.createElement('li');
//                            li.innerHTML = '<button type="button" class="dropdown-item" data-id="' +
//                                p.id + '" data-name="' + p.fullName + '">' + p.fullName + '</button>';
//                            li.querySelector('button').addEventListener('click', function () {
//                                selectPerson(this.dataset.id, this.dataset.name);
//                            });
//                            dropdown.appendChild(li);
//                        });
//                    }
//                    dropdown.classList.add('show');
//                });
//        }, 300);
//    });

//    // Close dropdown on outside click
//    document.addEventListener('click', function (e) {
//        if (!input.contains(e.target) && !dropdown.contains(e.target)) {
//            dropdown.classList.remove('show');
//        }
//    });

//    function selectPerson(id, name) {
//        hiddenInput.value = id;
//        input.value = '';
//        dropdown.classList.remove('show');
//        selectedBadge.classList.remove('d-none');
//        selectedName.textContent = name;
//    }

//    // Allow removing selection
//    const clearBtn = document.getElementById('clearPersonBtn');
//    if (clearBtn) {
//        clearBtn.addEventListener('click', function () {
//            hiddenInput.value = '';
//            selectedBadge.classList.add('d-none');
//            selectedName.textContent = '';
//        });
//    }
//}





