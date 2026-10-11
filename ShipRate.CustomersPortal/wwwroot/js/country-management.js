// ===============================================
// 🌍 Country Management - SPA Style
// بدون PostBack - تجربه کاربری عالی
// ===============================================

// ✅ متغیرهای سراسری
let currentPage = 1;
let pageSize = 10;
let searchTerm = '';
let sortColumn = 'name';
let sortDirection = 'asc';

// ===============================================
// 🔧 تنظیمات AJAX و CSRF Token
// ===============================================

$.ajaxSetup({
    beforeSend: function (xhr) {
        const token = $('meta[name="csrf-token"]').attr('content');
        if (token) {
            xhr.setRequestHeader('RequestVerificationToken', token);
        }
    }
});

// ===============================================
// 🚀 بارگذاری اولیه
// ===============================================

$(document).ready(function () {
    initializeApp();
});

function initializeApp() {
    // ✅ بارگذاری لیست کشورها
    loadCountries();

    // ✅ جستجو با تاخیر (Debounce)
    let searchTimeout;
    $('#searchBox').on('keyup', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => {
            searchTerm = $(this).val();
            currentPage = 1;
            loadCountries();
        }, 500);
    });

    // ✅ مرتب‌سازی
    $('#countriesTable th[data-sort]').on('click', function () {
        const column = $(this).data('sort');
        if (sortColumn === column) {
            sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            sortColumn = column;
            sortDirection = 'asc';
        }
        updateSortIcons();
        loadCountries();
    });

    // ✅ اعتبارسنجی Real-time کد کشور
    $('#code').on('blur', function () {
        const code = $(this).val().trim();
        const id = parseInt($('#countryId').val()) || 0;

        if (code && code.length >= 2) {
            checkCodeUniqueness(code, id === 0 ? null : id);
        }
    });

    // ✅ تبدیل خودکار کد به حروف بزرگ
    $('#code').on('input', function () {
        $(this).val($(this).val().toUpperCase());
    });
}

// ===============================================
// 📊 بارگذاری لیست کشورها
// ===============================================

function loadCountries() {
    $.ajax({
        url: '/Country/GetList',
        type: 'GET',
        data: {
            pageNumber: currentPage,
            pageSize: pageSize,
            searchTerm: searchTerm,
            sortColumn: sortColumn,
            sortDirection: sortDirection
        },
        success: function (response) {
            renderTable(response);
            renderPagination(response);
            updateStats(response);
        },
        error: function (xhr) {
            if (xhr.status === 403) {
                showError('شما دسترسی لازم برای مشاهده کشورها را ندارید');
            } else {
                showError('خطا در بارگذاری اطلاعات');
            }
        }
    });
}

// ===============================================
// 🎨 رندر جدول
// ===============================================

function renderTable(data) {
    const tbody = $('#countriesTableBody');
    tbody.empty();

    if (data.items.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="8" class="text-center py-5">
                    <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                    <p class="text-muted">موردی یافت نشد</p>
                </td>
            </tr>
        `);
        return;
    }

    data.items.forEach(country => {
        const row = `
            <tr data-country-id="${country.id}">
                <td>
                    <strong>${escapeHtml(country.name)}</strong>
                </td>
                <td>
                    <span class="badge bg-secondary">${escapeHtml(country.code)}</span>
                </td>
                <td>${country.capital ? escapeHtml(country.capital) : '<span class="text-muted">-</span>'}</td>
                <td>${country.population ? formatNumber(country.population) : '<span class="text-muted">-</span>'}</td>
                <td>${country.area ? formatNumber(country.area) + ' km²' : '<span class="text-muted">-</span>'}</td>
                <td>
                    <span class="badge bg-info">${country.citiesCount}</span>
                </td>
                <td>
                    <span class="badge ${country.isActive ? 'bg-success' : 'bg-secondary'}">
                        ${country.isActive ? 'فعال' : 'غیرفعال'}
                    </span>
                </td>
                <td class="text-center">
                    <button class="btn btn-sm btn-info" onclick="editCountry(${country.id})" 
                            title="ویرایش">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" 
                            onclick="deleteCountry(${country.id}, '${escapeHtml(country.name)}')"
                            ${country.citiesCount > 0 ? 'disabled' : ''}
                            title="${country.citiesCount > 0 ? 'کشور دارای شهر است' : 'حذف'}">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

// ===============================================
// 📄 رندر صفحه‌بندی
// ===============================================

function renderPagination(data) {
    const totalPages = Math.ceil(data.totalCount / data.pageSize);
    const pagination = $('#pagination');
    pagination.empty();

    if (totalPages <= 1) return;

    // ⏮️ اولین صفحه
    if (currentPage > 2) {
        pagination.append(createPageButton(1, '«'));
    }

    // ⏪ قبلی
    pagination.append(createPageButton(
        currentPage - 1,
        'قبلی',
        currentPage === 1
    ));

    // 🔢 شماره صفحات
    for (let i = 1; i <= totalPages; i++) {
        if (i === 1 || i === totalPages || Math.abs(i - currentPage) <= 2) {
            pagination.append(createPageButton(i, i.toString(), false, i === currentPage));
        } else if (Math.abs(i - currentPage) === 3) {
            pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
    }

    // ⏩ بعدی
    pagination.append(createPageButton(
        currentPage + 1,
        'بعدی',
        currentPage === totalPages
    ));

    // ⏭️ آخرین صفحه
    if (currentPage < totalPages - 1) {
        pagination.append(createPageButton(totalPages, '»'));
    }
}

function createPageButton(page, text, disabled = false, active = false) {
    return `
        <li class="page-item ${disabled ? 'disabled' : ''} ${active ? 'active' : ''}">
            <a class="page-link" href="#" onclick="changePage(${page}); return false;">
                ${text}
            </a>
        </li>
    `;
}

function changePage(page) {
    currentPage = page;
    loadCountries();
    // Scroll به بالای جدول
    $('html, body').animate({
        scrollTop: $('#countriesTable').offset().top - 100
    }, 300);
}

// ===============================================
// 📊 به‌روزرسانی آمار
// ===============================================

function updateStats(data) {
    $('#totalCountries').text(formatNumber(data.totalCount));

    const activeCount = data.items.filter(c => c.isActive).length;
    $('#activeCountries').text(formatNumber(activeCount));

    const totalCities = data.items.reduce((sum, c) => sum + c.citiesCount, 0);
    $('#totalCities').text(formatNumber(totalCities));

    const totalPopulation = data.items.reduce((sum, c) => sum + (c.population || 0), 0);
    $('#totalPopulation').text(formatNumber(totalPopulation));
}

// ===============================================
// ➕ افزودن کشور جدید
// ===============================================

function openCreateModal() {
    $('#modalTitle').html('<i class="fas fa-plus-circle"></i> افزودن کشور');
    $('#countryForm')[0].reset();
    $('#countryId').val('0');
    $('#isActive').prop('checked', true);
    clearValidationErrors();
    $('#countryModal').modal('show');
}

// ===============================================
// ✏️ ویرایش کشور
// ===============================================

function editCountry(id) {
    $.ajax({
        url: `/Country/GetById?id=${id}`,
        type: 'GET',
        success: function (country) {
            $('#modalTitle').html('<i class="fas fa-edit"></i> ویرایش کشور');
            $('#countryId').val(country.id);
            $('#name').val(country.name);
            $('#code').val(country.code);
            $('#capital').val(country.capital || '');
            $('#population').val(country.population || '');
            $('#area').val(country.area || '');
            $('#isActive').prop('checked', country.isActive);
            clearValidationErrors();
            $('#countryModal').modal('show');
        },
        error: function () {
            showError('خطا در بارگذاری اطلاعات');
        }
    });
}

// ===============================================
// 💾 ذخیره کشور
// ===============================================

function saveCountry() {
    clearValidationErrors();

    const id = parseInt($('#countryId').val()) || 0;
    const data = {
        id: id,
        name: $('#name').val().trim(),
        code: $('#code').val().trim().toUpperCase(),
        capital: $('#capital').val().trim() || null,
        population: $('#population').val() ? parseInt($('#population').val()) : null,
        area: $('#area').val() ? parseFloat($('#area').val()) : null,
        isActive: $('#isActive').is(':checked')
    };

    // ✅ Validation سمت کلاینت
    if (!data.name) {
        displayFieldError('name', 'نام کشور الزامی است');
        return;
    }
    if (!data.code) {
        displayFieldError('code', 'کد کشور الزامی است');
        return;
    }

    const url = id === 0 ? '/Country/Create' : '/Country/Update';
    const method = id === 0 ? 'POST' : 'PUT';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                $('#countryModal').modal('hide');
                loadCountries();
                showSuccess(response.message || (id === 0 ? 'کشور با موفقیت ایجاد شد' : 'کشور با موفقیت به‌روزرسانی شد'));
            }
        },
        error: function (xhr) {
            if (xhr.status === 400 && xhr.responseJSON && xhr.responseJSON.errors) {
                displayValidationErrors(xhr.responseJSON.errors);
            } else if (xhr.status === 403) {
                showError('شما دسترسی لازم برای انجام این عملیات را ندارید');
            } else {
                showError('خطا در ذخیره اطلاعات');
            }
        }
    });
}

// ===============================================
// 🗑️ حذف کشور
// ===============================================

function deleteCountry(id, name) {
    if (!confirm(`⚠️ آیا از حذف "${name}" اطمینان دارید؟\n\nاین عملیات غیرقابل بازگشت است.`)) {
        return;
    }

    $.ajax({
        url: `/Country/Delete?id=${id}`,
        type: 'DELETE',
        success: function (response) {
            if (response.success) {
                loadCountries();
                showSuccess(response.message || 'کشور با موفقیت حذف شد');
            }
        },
        error: function (xhr) {
            if (xhr.responseJSON && xhr.responseJSON.message) {
                showError(xhr.responseJSON.message);
            } else if (xhr.status === 403) {
                showError('شما دسترسی لازم برای حذف کشورها را ندارید');
            } else {
                showError('خطا در حذف کشور');
            }
        }
    });
}

// ===============================================
// ✅ بررسی یکتایی کد
// ===============================================

function checkCodeUniqueness(code, excludeId) {
    $.ajax({
        url: '/Country/CheckCodeUniqueness',
        type: 'GET',
        data: { code: code, excludeId: excludeId },
        success: function (response) {
            if (!response.isUnique) {
                displayFieldError('code', 'این کد قبلاً ثبت شده است');
            } else {
                clearFieldError('code');
            }
        }
    });
}

// ===============================================
// 🎨 مدیریت خطاهای Validation
// ===============================================

function displayValidationErrors(errors) {
    for (const [field, messages] of Object.entries(errors)) {
        displayFieldError(field, messages[0]);
    }
}

function displayFieldError(field, message) {
    const input = $(`#${field.toLowerCase()}`);
    input.addClass('is-invalid');
    $(`#${field.toLowerCase()}Error`).text(message);
}

function clearFieldError(field) {
    const input = $(`#${field.toLowerCase()}`);
    input.removeClass('is-invalid');
    $(`#${field.toLowerCase()}Error`).text('');
}

function clearValidationErrors() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').text('');
}

// ===============================================
// 🔧 توابع کمکی
// ===============================================

function updateSortIcons() {
    $('#countriesTable th[data-sort] i').attr('class', 'fas fa-sort');
    const currentHeader = $(`#countriesTable th[data-sort="${sortColumn}"] i`);
    currentHeader.attr('class', sortDirection === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down');
}

function formatNumber(num) {
    return num ? num.toLocaleString('fa-IR') : '0';
}

function escapeHtml(text) {
    if (!text) return '';
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.toString().replace(/[&<>"']/g, m => map[m]);
}

// ===============================================
// 🎨 پیام‌های Toastr
// ===============================================

function showSuccess(message) {
    toastr.success(message, 'موفق', {
        closeButton: true,
        progressBar: true,
        timeOut: 3000,
        positionClass: 'toast-top-left'
    });
}

function showError(message) {
    toastr.error(message, 'خطا', {
        closeButton: true,
        progressBar: true,
        timeOut: 5000,
        positionClass: 'toast-top-left'
    });
}

function showWarning(message) {
    toastr.warning(message, 'هشدار', {
        closeButton: true,
        progressBar: true,
        timeOut: 4000,
        positionClass: 'toast-top-left'
    });
}

function showInfo(message) {
    toastr.info(message, 'اطلاعات', {
        closeButton: true,
        progressBar: true,
        timeOut: 3000,
        positionClass: 'toast-top-left'
    });
}
