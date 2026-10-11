// ========================================
// City Management - AJAX AutoComplete
// ========================================

let currentPage = 1;
let pageSize = 10;
let sortColumn = 'Id';
let sortDirection = 'asc';

// بارگذاری اولیه
$(document).ready(function () {
    initializeSelect2Ajax();
    loadCities();

    // جستجو با Enter
    $('#searchCityId, #searchCityName').on('keypress', function (e) {
        if (e.which === 13) {
            searchCities();
        }
    });
});

// ========================================
// سیستم نوتیفیکیشن
// ========================================
const Notify = {
    success: function (message) {
        this.show(message, 'success');
    },
    error: function (message) {
        this.show(message, 'error');
    },
    show: function (message, type) {
        const container = $('#notifyContainer');
        const icon = type === 'success' ? '✓' : '✕';
        const notifyClass = type === 'success' ? 'notify-success' : 'notify-error';

        const notification = $(`
            <div class="notify-message ${notifyClass}">
                <span class="notify-icon">${icon}</span>
                <span>${message}</span>
            </div>
        `);

        container.append(notification);

        setTimeout(() => {
            notification.addClass('fade-out');
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }
};

// ========================================
// فعال‌سازی Select2 با AJAX Search
// ========================================
function initializeSelect2Ajax() {
    // تنظیمات مشترک Select2
    const select2Options = {
        ajax: {
            url: '/Country/SearchCountries',
            dataType: 'json',
            delay: 300, // تاخیر 300ms برای جلوگیری از درخواست‌های زیاد
            data: function (params) {
                return {
                    term: params.term || '', // متن جستجو
                    page: params.page || 1
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: data.results,
                    pagination: {
                        more: data.pagination.more
                    }
                };
            },
            cache: true
        },
        placeholder: 'تایپ کنید...',
        allowClear: true,
        dir: 'rtl',
        language: {
            inputTooShort: function () {
                return 'حداقل 1 کاراکتر وارد کنید';
            },
            searching: function () {
                return 'در حال جستجو...';
            },
            noResults: function () {
                return 'نتیجه‌ای یافت نشد';
            },
            loadingMore: function () {
                return 'بارگذاری بیشتر...';
            }
        },
        minimumInputLength: 0 // بدون حداقل کاراکتر (برای نمایش همه)
    };

    // Select2 برای فیلتر جستجو
    $('#searchCountryId').select2({
        ...select2Options,
        placeholder: 'همه کشورها'
    });

    // Select2 برای Modal
    $('#countryId').select2({
        ...select2Options,
        placeholder: 'انتخاب کنید...',
        dropdownParent: $('#cityModal')
    });
}

// ========================================
// جستجوی شهرها
// ========================================
function searchCities() {
    currentPage = 1;
    loadCities();
}

function clearFilters() {
    $('#searchCityId').val('');
    $('#searchCityName').val('');
    $('#searchCountryId').val(null).trigger('change');
    currentPage = 1;
    sortColumn = 'Id';
    sortDirection = 'asc';
    loadCities();
}

// ========================================
// بارگذاری لیست شهرها
// ========================================
function loadCities() {
    const cityId = $('#searchCityId').val();
    const cityName = $('#searchCityName').val();
    const countryId = $('#searchCountryId').val();

    const params = {
        pageNumber: currentPage,
        pageSize: pageSize,
        sortColumn: sortColumn,
        sortDirection: sortDirection
    };

    if (cityId) params.cityId = cityId;
    if (cityName) params.cityName = cityName;
    if (countryId) params.countryId = countryId;

    $.ajax({
        url: '/City/GetPaged',
        type: 'GET',
        data: params,
        success: function (response) {
            if (response.success) {
                renderCityTable(response.data.items);
                renderPagination(response.data);
            } else {
                Notify.error(response.message || 'خطا در دریافت اطلاعات');
            }
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON?.message || 'خطا در بارگذاری شهرها';
            Notify.error(errorMsg);
            $('#cityTableBody').html(`<tr><td colspan="8" class="text-center text-danger">${errorMsg}</td></tr>`);
        }
    });
}

// ========================================
// رندر جدول شهرها
// ========================================
function renderCityTable(cities) {
    const tbody = $('#cityTableBody');
    tbody.empty();

    if (!cities || cities.length === 0) {
        tbody.html('<tr><td colspan="8" class="text-center">شهری یافت نشد</td></tr>');
        return;
    }

    cities.forEach(city => {
        const populationFormatted = city.population ? city.population.toLocaleString('fa-IR') : '-';
        const areaFormatted = city.area ? city.area.toLocaleString('fa-IR') : '-';
        const isCapitalBadge = city.isCapital
            ? '<span class="badge badge-warning">بله</span>'
            : '<span class="badge badge-secondary">خیر</span>';
        const isActiveBadge = city.isActive
            ? '<span class="badge badge-success">فعال</span>'
            : '<span class="badge badge-danger">غیرفعال</span>';

        const row = `
            <tr>
                <td>${city.id}</td>
                <td>${city.name}</td>
                <td>${city.countryName || '-'}</td>
                <td>${populationFormatted}</td>
                <td>${areaFormatted}</td>
                <td>${isCapitalBadge}</td>
                <td>${isActiveBadge}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="showEditModal(${city.id})" title="ویرایش">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="confirmDelete(${city.id}, '${city.name}')" title="حذف">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
        tbody.append(row);
    });
}

// ========================================
// صفحه‌بندی
// ========================================
function renderPagination(data) {
    const pagination = $('#pagination');
    pagination.empty();

    if (data.totalPages <= 1) return;

    if (data.hasPreviousPage) {
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="changePage(${currentPage - 1}); return false;">قبلی</a></li>`);
    } else {
        pagination.append(`<li class="page-item disabled"><span class="page-link">قبلی</span></li>`);
    }

    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(data.totalPages, currentPage + 2);

    for (let i = startPage; i <= endPage; i++) {
        const activeClass = i === currentPage ? 'active' : '';
        pagination.append(`<li class="page-item ${activeClass}"><a class="page-link" href="#" onclick="changePage(${i}); return false;">${i}</a></li>`);
    }

    if (data.hasNextPage) {
        pagination.append(`<li class="page-item"><a class="page-link" href="#" onclick="changePage(${currentPage + 1}); return false;">بعدی</a></li>`);
    } else {
        pagination.append(`<li class="page-item disabled"><span class="page-link">بعدی</span></li>`);
    }
}

function changePage(page) {
    currentPage = page;
    loadCities();
}

// ========================================
// مرتب‌سازی
// ========================================
function sortTable(column) {
    if (sortColumn === column) {
        sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
        sortColumn = column;
        sortDirection = 'asc';
    }
    loadCities();
}

// ========================================
// نمایش مودال ایجاد
// ========================================
function showCreateModal() {
    $('#cityModalTitle').text('افزودن شهر جدید');
    $('#cityId').val(0);
    $('#cityForm')[0].reset();
    $('#isActive').prop('checked', true);
    $('#countryId').val(null).trigger('change');
    $('#cityModal').modal('show');
}

// ========================================
// نمایش مودال ویرایش
// ========================================
function showEditModal(id) {
    $.ajax({
        url: `/City/GetById/${id}`,
        type: 'GET',
        success: function (response) {
            if (response.success) {
                const city = response.data;
                $('#cityModalTitle').text('ویرایش شهر');
                $('#cityId').val(city.id);
                $('#cityName').val(city.name);

                // Set Select2 با Option جدید
                const newOption = new Option(city.countryName, city.countryId, true, true);
                $('#countryId').append(newOption).trigger('change');

                $('#population').val(city.population || '');
                $('#area').val(city.area || '');
                $('#isCapital').prop('checked', city.isCapital);
                $('#isActive').prop('checked', city.isActive);
                $('#cityModal').modal('show');
            } else {
                Notify.error(response.message || 'خطا در بارگذاری اطلاعات شهر');
            }
        },
        error: function () {
            Notify.error('خطا در بارگذاری اطلاعات شهر');
        }
    });
}

// ========================================
// ذخیره شهر
// ========================================
function saveCity() {
    const id = parseInt($('#cityId').val());
    const cityDto = {
        id: id,
        name: $('#cityName').val().trim(),
        countryId: parseInt($('#countryId').val()),
        population: $('#population').val() ? parseInt($('#population').val()) : null,
        area: $('#area').val() ? parseFloat($('#area').val()) : null,
        isCapital: $('#isCapital').is(':checked'),
        isActive: $('#isActive').is(':checked')
    };

    if (!cityDto.name) {
        Notify.error('نام شهر الزامی است');
        return;
    }
    if (!cityDto.countryId || cityDto.countryId === 0) {
        Notify.error('انتخاب کشور الزامی است');
        return;
    }

    const url = id === 0 ? '/City/Create' : '/City/Update';
    const method = id === 0 ? 'POST' : 'PUT';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        data: JSON.stringify(cityDto),
        success: function (response) {
            $('#cityModal').modal('hide');
            loadCities();

            if (response && response.success === false) {
                Notify.error(response.message || 'خطا در ذخیره');
            } else {
                const msg = (response && response.message)
                    ? response.message
                    : (id === 0 ? 'شهر با موفقیت ایجاد شد' : 'شهر با موفقیت ویرایش شد');
                Notify.success(msg);
            }
        },
        error: function (xhr) {
            let errorMsg = 'خطا در ذخیره شهر';
            if (xhr.responseJSON?.message) {
                errorMsg = xhr.responseJSON.message;
            } else if (xhr.responseJSON?.errors) {
                errorMsg = xhr.responseJSON.errors.join(', ');
            }
            Notify.error(errorMsg);
        }
    });
}

// ========================================
// حذف شهر
// ========================================
function confirmDelete(id, name) {
    if (confirm(`آیا از حذف شهر "${name}" اطمینان دارید؟`)) {
        $.ajax({
            url: `/City/Delete/${id}`,
            type: 'DELETE',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    Notify.success(response.message || 'شهر با موفقیت حذف شد');
                    loadCities();
                } else {
                    Notify.error(response.message || 'خطا در حذف شهر');
                }
            },
            error: function (xhr) {
                const errorMsg = xhr.responseJSON?.message || 'خطا در حذف شهر';
                Notify.error(errorMsg);
            }
        });
    }
}
