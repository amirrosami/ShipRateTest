// wwwroot/js/master-detail.js

(function () {
    'use strict';

    // ============================================
    // MasterDetailManager - Core Engine
    // ============================================
    class MasterDetailManager {
        constructor(sectionEl) {
            this.section = sectionEl;
            this.propertyName = sectionEl.dataset.propertyName;
            this.minRows = parseInt(sectionEl.dataset.minRows) || 0;
            this.maxRows = parseInt(sectionEl.dataset.maxRows) || 0;
            this.initialRows = parseInt(sectionEl.dataset.initialRows) || 1;
            this.confirmDelete = sectionEl.dataset.confirmDelete === 'true';
            this.confirmDeleteMessage = sectionEl.dataset.confirmDeleteMessage || 'Are you sure?';
            this.showRowNumbers = sectionEl.dataset.showRowNumbers === 'true';
            this.allowDelete = sectionEl.dataset.allowDelete === 'true';

            this.tbody = sectionEl.querySelector('.md-detail-body');
            this.templateEl = sectionEl.querySelector('.md-row-template');
            this.columnsDefEl = sectionEl.querySelector('.md-columns-def');
            this.existingDataEl = sectionEl.querySelector('.md-existing-data');
            this.addBtn = sectionEl.querySelector('.md-add-row-btn');

            this.template = this.templateEl ? this.templateEl.innerHTML : '';
            this.columnsDef = this.columnsDefEl ? JSON.parse(this.columnsDefEl.textContent) : [];
            this.rowIndex = 0;

            this.init();
        }

        init() {
            // Bind add button
            if (this.addBtn) {
                this.addBtn.addEventListener('click', () => this.addRow());
            }

            // Bind delete buttons (event delegation)
            this.section.addEventListener('click', (e) => {
                if (e.target.closest('.md-remove-row-btn')) {
                    this.removeRow(e.target.closest('.md-detail-row'));
                }
            });

            // Bind input changes for compute and totals
            this.section.addEventListener('input', (e) => {
                if (e.target.closest('.md-detail-input')) {
                    const row = e.target.closest('.md-detail-row');
                    this.computeRow(row);
                    this.updateTotals();
                }
            });

            this.section.addEventListener('change', (e) => {
                if (e.target.closest('.md-detail-input')) {
                    const row = e.target.closest('.md-detail-row');
                    this.computeRow(row);
                    this.updateTotals();
                }
            });

            // Load existing data (Edit mode)
            if (this.existingDataEl) {
                try {
                    const existingData = JSON.parse(this.existingDataEl.textContent);
                    if (Array.isArray(existingData) && existingData.length > 0) {
                        existingData.forEach(data => this.addRow(data));
                        return;
                    }
                } catch (e) {
                    console.warn('MasterDetail: Could not parse existing data', e);
                }
            }

            // Add initial rows (Create mode)
            for (let i = 0; i < this.initialRows; i++) {
                this.addRow();
            }
        }

        addRow(data = null) {
            // Check max rows
            if (this.maxRows > 0) {
                const currentRows = this.tbody.querySelectorAll('.md-detail-row').length;
                if (currentRows >= this.maxRows) {
                    this.showNotification(`Maximum ${this.maxRows} rows allowed.`, 'warning');
                    return null;
                }
            }

            const index = this.rowIndex++;
            let rowHtml = this.template
                .replace(/__INDEX__/g, index)
                .replace(/__ROWNUM__/g, this.getNextRowNumber());

            // Create row element
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = rowHtml.trim();
            const row = tempDiv.querySelector('tr');

            if (!row) return null;

            // Set data if provided (Edit mode)
            if (data) {
                this.populateRow(row, data, index);
            }

            // Add row to table body with animation
            row.style.opacity = '0';
            row.style.transform = 'translateY(-10px)';
            this.tbody.appendChild(row);

            // Trigger animation
            requestAnimationFrame(() => {
                row.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
                row.style.opacity = '1';
                row.style.transform = 'translateY(0)';
            });

            // Compute values
            this.computeRow(row);
            this.updateTotals();
            this.updateRowNumbers();
            this.updateAddButtonState();

            // Focus first visible input in new row
            if (!data) {
                const firstInput = row.querySelector(
                    'input:not([type="hidden"]):not([readonly]), select, textarea'
                );
                if (firstInput) {
                    setTimeout(() => firstInput.focus(), 100);
                }
            }

            // Dispatch custom event
            this.section.dispatchEvent(new CustomEvent('md:rowAdded', {
                detail: { row, index, data },
                bubbles: true
            }));

            return row;
        }

        populateRow(row, data, index) {
            this.columnsDef.forEach(col => {
                const field = row.querySelector(`[data-field="${col.Name}"]`);
                if (!field) return;

                // Handle case-insensitive property matching
                const value = this.getPropertyValue(data, col.Name);

                if (value === null || value === undefined) return;

                if (field.type === 'checkbox') {
                    field.checked = value === true || value === 'true' || value === 'True';
                } else if (field.tagName === 'SELECT') {
                    field.value = value.toString();
                } else {
                    field.value = value;
                }
            });
        }

        getPropertyValue(obj, propName) {
            // Case-insensitive property lookup
            if (obj[propName] !== undefined) return obj[propName];

            const lowerProp = propName.toLowerCase();
            for (const key of Object.keys(obj)) {
                if (key.toLowerCase() === lowerProp) {
                    return obj[key];
                }
            }
            return null;
        }

        removeRow(row) {
            if (!row) return;

            // Check minimum rows
            const currentRows = this.tbody.querySelectorAll('.md-detail-row').length;
            if (this.minRows > 0 && currentRows <= this.minRows) {
                this.showNotification(`Minimum ${this.minRows} row(s) required.`, 'warning');
                return;
            }

            // Confirm delete
            if (this.confirmDelete) {
                if (!confirm(this.confirmDeleteMessage)) return;
            }

            // Animate removal
            row.style.transition = 'opacity 0.3s ease, transform 0.3s ease, max-height 0.3s ease';
            row.style.opacity = '0';
            row.style.transform = 'translateX(50px)';
            row.style.maxHeight = row.offsetHeight + 'px';

            setTimeout(() => {
                row.style.maxHeight = '0';
                row.style.padding = '0';
                row.style.margin = '0';
                row.style.overflow = 'hidden';
            }, 150);

            setTimeout(() => {
                row.remove();
                this.updateRowNumbers();
                this.updateTotals();
                this.updateAddButtonState();

                // Dispatch custom event
                this.section.dispatchEvent(new CustomEvent('md:rowRemoved', {
                    bubbles: true
                }));
            }, 400);
        }

        computeRow(row) {
            const computeFields = row.querySelectorAll('[data-compute]');
            computeFields.forEach(field => {
                const formula = field.dataset.compute;
                try {
                    const result = this.evaluateFormula(formula, row);
                    if (!isNaN(result) && isFinite(result)) {
                        field.value = this.formatNumber(result, field);
                    }
                } catch (e) {
                    // Silent fail for compute errors
                }
            });
        }

        evaluateFormula(formula, row) {
            // Replace field names with their values
            let expression = formula;

            this.columnsDef.forEach(col => {
                const field = row.querySelector(`[data-field="${col.Name}"]`);
                if (field) {
                    const value = parseFloat(field.value) || 0;
                    // Use word boundary regex to replace field names
                    const regex = new RegExp(`\\b${col.Name}\\b`, 'g');
                    expression = expression.replace(regex, value.toString());
                }
            });

            // Safely evaluate the mathematical expression
            return this.safeEval(expression);
        }

        safeEval(expression) {
            // Only allow numbers, operators, parentheses, and spaces
            const sanitized = expression.replace(/[^0-9+\-*/().%\s]/g, '');
            if (!sanitized) return 0;

            try {
                return new Function(`'use strict'; return (${sanitized})`)();
            } catch (e) {
                return 0;
            }
        }

        formatNumber(value, field) {
            if (field.step) {
                const decimals = (field.step.split('.')[1] || '').length;
                return value.toFixed(decimals);
            }
            if (field.classList.contains('md-decimal')) {
                return value.toFixed(2);
            }
            if (field.classList.contains('md-number')) {
                return Math.round(value).toString();
            }
            return value.toFixed(2);
        }

        updateTotals() {
            const footerCells = this.section.querySelectorAll('.md-total-cell');
            footerCells.forEach(cell => {
                const fieldName = cell.dataset.totalFor;
                let total = 0;

                this.tbody.querySelectorAll('.md-detail-row').forEach(row => {
                    const field = row.querySelector(`[data-field="${fieldName}"]`);
                    if (field) {
                        total += parseFloat(field.value) || 0;
                    }
                });

                cell.querySelector('strong').textContent = total.toFixed(2);
            });

            // Dispatch totals updated event
            this.section.dispatchEvent(new CustomEvent('md:totalsUpdated', {
                bubbles: true
            }));
        }

        updateRowNumbers() {
            if (!this.showRowNumbers) return;

            this.tbody.querySelectorAll('.md-detail-row').forEach((row, idx) => {
                const numCell = row.querySelector('.md-row-number');
                if (numCell) {
                    numCell.textContent = idx + 1;
                }
            });
        }

        getNextRowNumber() {
            return this.tbody.querySelectorAll('.md-detail-row').length + 1;
        }

        updateAddButtonState() {
            if (this.maxRows > 0 && this.addBtn) {
                const currentRows = this.tbody.querySelectorAll('.md-detail-row').length;
                this.addBtn.disabled = currentRows >= this.maxRows;
                if (currentRows >= this.maxRows) {
                    this.addBtn.classList.add('md-btn-disabled');
                } else {
                    this.addBtn.classList.remove('md-btn-disabled');
                }
            }
        }

        showNotification(message, type = 'info') {
            const notification = document.createElement('div');
            notification.className = `md-notification md-notification-${type}`;
            notification.textContent = message;
            notification.style.cssText = `
                position: fixed; top: 20px; right: 20px; padding: 12px 20px;
                border-radius: 6px; color: white; font-weight: 500; z-index: 10000;
                animation: mdSlideIn 0.3s ease;
                background-color: ${type === 'warning' ? '#f59e0b' : type === 'error' ? '#ef4444' : '#3b82f6'};
            `;
            document.body.appendChild(notification);
            setTimeout(() => {
                notification.style.opacity = '0';
                notification.style.transition = 'opacity 0.3s ease';
                setTimeout(() => notification.remove(), 300);
            }, 3000);
        }

        // Public API
        getRowCount() {
            return this.tbody.querySelectorAll('.md-detail-row').length;
        }

        getAllData() {
            const rows = [];
            this.tbody.querySelectorAll('.md-detail-row').forEach(row => {
                const rowData = {};
                this.columnsDef.forEach(col => {
                    const field = row.querySelector(`[data-field="${col.Name}"]`);
                    if (field) {
                        if (field.type === 'checkbox') {
                            rowData[col.Name] = field.checked;
                        } else if (field.type === 'number') {
                            rowData[col.Name] = parseFloat(field.value) || 0;
                        } else {
                            rowData[col.Name] = field.value;
                        }
                    }
                });
                rows.push(rowData);
            });
            return rows;
        }

        clearAllRows() {
            this.tbody.innerHTML = '';
            this.rowIndex = 0;
            this.updateTotals();
            this.updateAddButtonState();
        }
    }

    // ============================================
    // Form Validation
    // ============================================
    class MasterDetailValidator {
        constructor(formEl) {
            this.form = formEl;
            this.validationSummary = formEl.querySelector('.md-validation-summary');
            this.init();
        }

        init() {
            this.form.addEventListener('submit', (e) => {
                if (!this.validate()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        }

        validate() {
            this.clearErrors();
            const errors = [];

            // Validate required fields
            const requiredFields = this.form.querySelectorAll('[required]');
            requiredFields.forEach(field => {
                if (!field.value || field.value.trim() === '') {
                    const label = this.getFieldLabel(field);
                    errors.push(`${label} is required.`);
                    this.markFieldError(field);
                }
            });

            // Validate number fields
            const numberFields = this.form.querySelectorAll('input[type="number"]');
            numberFields.forEach(field => {
                if (field.value !== '') {
                    const val = parseFloat(field.value);
                    if (field.min !== '' && val < parseFloat(field.min)) {
                        const label = this.getFieldLabel(field);
                        errors.push(`${label} must be at least ${field.min}.`);
                        this.markFieldError(field);
                    }
                    if (field.max !== '' && val > parseFloat(field.max)) {
                        const label = this.getFieldLabel(field);
                        errors.push(`${label} must be at most ${field.max}.`);
                        this.markFieldError(field);
                    }
                }
            });

            // Validate email fields
            const emailFields = this.form.querySelectorAll('input[type="email"]');
            emailFields.forEach(field => {
                if (field.value && !this.isValidEmail(field.value)) {
                    const label = this.getFieldLabel(field);
                    errors.push(`${label} is not a valid email.`);
                    this.markFieldError(field);
                }
            });

            // Check minimum rows for detail sections
            const detailSections = this.form.querySelectorAll('.md-detail-section');
            detailSections.forEach(section => {
                const minRows = parseInt(section.dataset.minRows) || 0;
                if (minRows > 0) {
                    const currentRows = section.querySelectorAll('.md-detail-row').length;
                    if (currentRows < minRows) {
                        errors.push(`At least ${minRows} detail row(s) required.`);
                    }
                }
            });

            if (errors.length > 0) {
                this.showErrors(errors);
                return false;
            }

            return true;
        }

        getFieldLabel(field) {
            // Try to find associated label
            const id = field.id;
            if (id) {
                const label = this.form.querySelector(`label[for="${id}"]`);
                if (label) return label.textContent.replace('*', '').trim();
            }
            // Fall back to name
            const name = field.name || '';
            const parts = name.split('.');
            return parts[parts.length - 1] || 'Field';
        }

        markFieldError(field) {
            field.classList.add('md-input-error');
            field.addEventListener('input', () => {
                field.classList.remove('md-input-error');
            }, { once: true });
        }

        showErrors(errors) {
            if (this.validationSummary) {
                const ul = this.validationSummary.querySelector('ul');
                ul.innerHTML = errors.map(e => `<li>${e}</li>`).join('');
                this.validationSummary.style.display = 'block';

                // Scroll to validation summary
                this.validationSummary.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }

        clearErrors() {
            if (this.validationSummary) {
                this.validationSummary.style.display = 'none';
                this.validationSummary.querySelector('ul').innerHTML = '';
            }
            this.form.querySelectorAll('.md-input-error').forEach(el => {
                el.classList.remove('md-input-error');
            });
        }

        isValidEmail(email) {
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
        }
    }

    // ============================================
    // Auto-Initialize
    // ============================================
    function initMasterDetail() {
        // Initialize all detail sections
        document.querySelectorAll('.md-detail-section').forEach(section => {
            if (!section._mdManager) {
                section._mdManager = new MasterDetailManager(section);
            }
        });

        // Initialize form validation
        document.querySelectorAll('.md-form').forEach(form => {
            if (!form._mdValidator) {
                form._mdValidator = new MasterDetailValidator(form);
            }
        });
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initMasterDetail);
    } else {
        initMasterDetail();
    }

    // Expose globally
    window.MasterDetailManager = MasterDetailManager;
    window.MasterDetailValidator = MasterDetailValidator;
    window.MasterDetail = {
        getManager: (sectionEl) => sectionEl._mdManager,
        getValidator: (formEl) => formEl._mdValidator,
        reinit: initMasterDetail
    };

})();
