using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("detail-column", ParentTag = "detail-section")]
    public class DetailColumnTagHelper : TagHelper
    {
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("header")]
        public string Header { get; set; }

        [HtmlAttributeName("type")]
        public DetailColumnType ColumnType { get; set; } = DetailColumnType.Text;

        [HtmlAttributeName("required")]
        public bool Required { get; set; } = false;

        [HtmlAttributeName("default-value")]
        public string DefaultValue { get; set; } = "";

        [HtmlAttributeName("placeholder")]
        public string Placeholder { get; set; } = "";

        [HtmlAttributeName("width")]
        public string Width { get; set; }

        [HtmlAttributeName("css-class")]
        public string CssClass { get; set; } = "";

        [HtmlAttributeName("min")]
        public string Min { get; set; } = "";

        [HtmlAttributeName("max")]
        public string Max { get; set; } = "";

        [HtmlAttributeName("step")]
        public string Step { get; set; } = "";

        [HtmlAttributeName("select-items")]
        public IEnumerable<SelectListData> SelectItems { get; set; }

        /// <summary>
        /// Show total in footer (for numeric columns)
        /// </summary>
        [HtmlAttributeName("show-total")]
        public bool ShowTotal { get; set; } = false;

        /// <summary>
        /// This column shows "Total:" label in footer
        /// </summary>
        [HtmlAttributeName("is-total-label")]
        public bool IsTotalLabel { get; set; } = false;

        [HtmlAttributeName("total-label-text")]
        public string TotalLabelText { get; set; } = "Total:";

        /// <summary>
        /// Auto-compute formula: e.g., "Quantity * UnitPrice" 
        /// or "Quantity * UnitPrice * (1 - DiscountPercent / 100)"
        /// </summary>
        [HtmlAttributeName("compute")]
        public string ComputeFormula { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Don't render any HTML - just register column definition
            output.SuppressOutput();

            if (context.Items.TryGetValue("DetailColumns", out var columnsObj) &&
                columnsObj is List<DetailColumnDefinition> columns)
            {
                columns.Add(new DetailColumnDefinition
                {
                    Name = Name,
                    Header = Header ?? Name,
                    ColumnType = ColumnType,
                    Required = Required,
                    DefaultValue = DefaultValue,
                    Placeholder = Placeholder,
                    Width = Width,
                    CssClass = CssClass,
                    Min = Min,
                    Max = Max,
                    Step = Step,
                    SelectItems = SelectItems != null ? new List<SelectListData>(SelectItems) : null,
                    ShowTotal = ShowTotal,
                    IsTotalLabel = IsTotalLabel,
                    TotalLabelText = TotalLabelText,
                    ComputeFormula = ComputeFormula
                });
            }
        }
    }

    [HtmlTargetElement("detail-section", ParentTag = "master-detail-form")]
    [RestrictChildren("detail-column")]
    public class DetailSectionTagHelper : TagHelper
    {
        [HtmlAttributeName("title")]
        public string Title { get; set; } = "Items";

        [HtmlAttributeName("property-name")]
        public string PropertyName { get; set; } = "Details";

        [HtmlAttributeName("section-class")]
        public string SectionClass { get; set; } = "";

        [HtmlAttributeName("add-button-text")]
        public string AddButtonText { get; set; } = "+ Add Row";

        [HtmlAttributeName("show-row-numbers")]
        public bool ShowRowNumbers { get; set; } = true;

        [HtmlAttributeName("allow-delete")]
        public bool AllowDelete { get; set; } = true;

        [HtmlAttributeName("min-rows")]
        public int MinRows { get; set; } = 0;

        [HtmlAttributeName("max-rows")]
        public int MaxRows { get; set; } = 0;

        [HtmlAttributeName("initial-rows")]
        public int InitialRows { get; set; } = 1;

        /// <summary>
        /// Existing detail data for Edit mode - pass as JSON string
        /// </summary>
        [HtmlAttributeName("existing-data")]
        public string ExistingDataJson { get; set; }

        [HtmlAttributeName("confirm-delete")]
        public bool ConfirmDelete { get; set; } = true;

        [HtmlAttributeName("confirm-delete-message")]
        public string ConfirmDeleteMessage { get; set; } = "Are you sure you want to remove this row?";

        [HtmlAttributeName("show-footer-totals")]
        public bool ShowFooterTotals { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Create a list to collect column definitions from children
            var columns = new List<DetailColumnDefinition>();
            context.Items["DetailColumns"] = columns;
            context.Items["DetailPropertyName"] = PropertyName;

            // Process children to collect column definitions
            await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"md-detail-section {SectionClass}".Trim());
            output.Attributes.SetAttribute("data-property-name", PropertyName);
            output.Attributes.SetAttribute("data-min-rows", MinRows.ToString());
            output.Attributes.SetAttribute("data-max-rows", MaxRows.ToString());
            output.Attributes.SetAttribute("data-initial-rows", InitialRows.ToString());
            output.Attributes.SetAttribute("data-confirm-delete", ConfirmDelete.ToString().ToLower());
            output.Attributes.SetAttribute("data-confirm-delete-message", ConfirmDeleteMessage);
            output.Attributes.SetAttribute("data-show-row-numbers", ShowRowNumbers.ToString().ToLower());
            output.Attributes.SetAttribute("data-allow-delete", AllowDelete.ToString().ToLower());

            var sb = new StringBuilder();

            // Section header
            sb.AppendLine("<div class=\"md-section-header md-detail-header\">");
            sb.AppendLine($"  <h4 class=\"md-section-title\">{Title}</h4>");
            sb.AppendLine($"  <button type=\"button\" class=\"md-btn md-btn-add md-add-row-btn\">{AddButtonText}</button>");
            sb.AppendLine("</div>");

            // Table
            sb.AppendLine("<div class=\"md-table-responsive\">");
            sb.AppendLine("<table class=\"md-detail-table\">");

            // Table head
            sb.AppendLine("<thead><tr>");
            if (ShowRowNumbers)
                sb.AppendLine("  <th class=\"md-th-row-num\">#</th>");

            foreach (var col in columns)
            {
                string requiredStar = col.Required ? " <span class=\"md-required\">*</span>" : "";
                string widthStyle = !string.IsNullOrEmpty(col.Width) ? $" style=\"width:{col.Width}\"" : "";
                sb.AppendLine($"  <th{widthStyle}>{col.Header}{requiredStar}</th>");
            }

            if (AllowDelete)
                sb.AppendLine("  <th class=\"md-th-actions\">Actions</th>");
            sb.AppendLine("</tr></thead>");

            // Table body
            sb.AppendLine("<tbody class=\"md-detail-body\">");
            sb.AppendLine("</tbody>");

            // Table footer (totals)
            if (ShowFooterTotals)
            {
                sb.AppendLine("<tfoot><tr class=\"md-detail-footer\">");
                if (ShowRowNumbers)
                    sb.AppendLine("  <td></td>");
                foreach (var col in columns)
                {
                    if (col.ShowTotal)
                        sb.AppendLine($"  <td class=\"md-total-cell\" data-total-for=\"{col.Name}\"><strong>0</strong></td>");
                    else if (col.IsTotalLabel)
                        sb.AppendLine($"  <td class=\"md-total-label\"><strong>{col.TotalLabelText}</strong></td>");
                    else
                        sb.AppendLine("  <td></td>");
                }
                if (AllowDelete)
                    sb.AppendLine("  <td></td>");
                sb.AppendLine("</tr></tfoot>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            // Hidden template for JS row creation
            sb.AppendLine("<script type=\"text/html\" class=\"md-row-template\">");
            sb.AppendLine(BuildRowTemplate(columns, ShowRowNumbers, AllowDelete, PropertyName));
            sb.AppendLine("</script>");

            // Columns definition as JSON for JS
            sb.AppendLine($"<script type=\"application/json\" class=\"md-columns-def\">");
            sb.AppendLine(System.Text.Json.JsonSerializer.Serialize(columns.Select(c => new
            {
                c.Name,
                c.ColumnType,
                c.Required,
                c.DefaultValue,
                c.ShowTotal,
                c.ComputeFormula,
                selectItems = c.SelectItems?.Select(si => new { si.Value, si.Text })
            })));
            sb.AppendLine("</script>");

            // Existing data for edit mode
            if (!string.IsNullOrEmpty(ExistingDataJson))
            {
                sb.AppendLine($"<script type=\"application/json\" class=\"md-existing-data\">");
                sb.AppendLine(ExistingDataJson);
                sb.AppendLine("</script>");
            }

            output.Content.SetHtmlContent(sb.ToString());
        }

        private string BuildRowTemplate(List<DetailColumnDefinition> columns,
            bool showRowNumbers, bool allowDelete, string propertyName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr class=\"md-detail-row\" data-row-index=\"__INDEX__\">");

            if (showRowNumbers)
                sb.AppendLine("  <td class=\"md-row-number\">__ROWNUM__</td>");

            foreach (var col in columns)
            {
                sb.AppendLine($"  <td class=\"md-cell md-cell-{col.Name.ToLower()}\">");
                sb.AppendLine($"    {BuildColumnInput(col, propertyName)}");
                sb.AppendLine("  </td>");
            }

            if (allowDelete)
            {
                sb.AppendLine("  <td class=\"md-cell-actions\">");
                sb.AppendLine("    <button type=\"button\" class=\"md-btn md-btn-danger md-remove-row-btn\" title=\"Remove\">✕</button>");
                sb.AppendLine("  </td>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string BuildColumnInput(DetailColumnDefinition col, string propertyName)
        {
            string namePrefix = $"{propertyName}[__INDEX__].{col.Name}";
            string idPrefix = $"{propertyName}___INDEX____{col.Name}";
            string requiredAttr = col.Required ? $"required=\"required\" data-val=\"true\" data-val-required=\"{col.Header} is required.\"" : "";
            string computeAttr = !string.IsNullOrEmpty(col.ComputeFormula) ? $"data-compute=\"{col.ComputeFormula}\"" : "";
            string cssClass = !string.IsNullOrEmpty(col.CssClass) ? col.CssClass : "";

            switch (col.ColumnType)
            {
                case DetailColumnType.Select:
                    var ssb = new StringBuilder();
                    ssb.AppendLine($"<select class=\"md-input md-detail-input {cssClass}\" name=\"{namePrefix}\" id=\"{idPrefix}\" " +
                        $"{requiredAttr} data-field=\"{col.Name}\">");
                    ssb.AppendLine($"  <option value=\"\">-- Select --</option>");
                    if (col.SelectItems != null)
                    {
                        foreach (var item in col.SelectItems)
                        {
                            ssb.AppendLine($"  <option value=\"{item.Value}\">{item.Text}</option>");
                        }
                    }
                    ssb.AppendLine("</select>");
                    return ssb.ToString();

                case DetailColumnType.Number:
                    return $"<input type=\"number\" class=\"md-input md-detail-input md-number {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"step=\"1\" min=\"{col.Min}\" max=\"{col.Max}\" " +
                           $"{requiredAttr} {computeAttr} data-field=\"{col.Name}\" />";

                case DetailColumnType.Decimal:
                    string step = !string.IsNullOrEmpty(col.Step) ? col.Step : "0.01";
                    return $"<input type=\"number\" class=\"md-input md-detail-input md-decimal {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"step=\"{step}\" min=\"{col.Min}\" max=\"{col.Max}\" " +
                           $"{requiredAttr} {computeAttr} data-field=\"{col.Name}\" />";

                case DetailColumnType.Date:
                    return $"<input type=\"date\" class=\"md-input md-detail-input md-date {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"{requiredAttr} data-field=\"{col.Name}\" />";

                case DetailColumnType.Checkbox:
                    string chk = col.DefaultValue?.ToLower() == "true" ? "checked" : "";
                    return $"<input type=\"checkbox\" class=\"md-checkbox md-detail-input {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"true\" {chk} " +
                           $"data-field=\"{col.Name}\" />" +
                           $"<input type=\"hidden\" name=\"{namePrefix}\" value=\"false\" />";

                case DetailColumnType.Hidden:
                    return $"<input type=\"hidden\" class=\"md-detail-input {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"data-field=\"{col.Name}\" />";

                case DetailColumnType.ReadOnly:
                    return $"<input type=\"text\" class=\"md-input md-detail-input md-readonly {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"readonly=\"readonly\" tabindex=\"-1\" {computeAttr} data-field=\"{col.Name}\" />";

                case DetailColumnType.TextArea:
                    return $"<textarea class=\"md-input md-detail-input md-textarea {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" rows=\"2\" " +
                           $"{requiredAttr} data-field=\"{col.Name}\">{col.DefaultValue}</textarea>";

                default: // Text, Email, Tel, Url
                    string inputType = col.ColumnType switch
                    {
                        DetailColumnType.Email => "email",
                        DetailColumnType.Tel => "tel",
                        DetailColumnType.Url => "url",
                        DetailColumnType.Color => "color",
                        _ => "text"
                    };
                    return $"<input type=\"{inputType}\" class=\"md-input md-detail-input {cssClass}\" " +
                           $"name=\"{namePrefix}\" id=\"{idPrefix}\" value=\"{col.DefaultValue}\" " +
                           $"placeholder=\"{col.Placeholder}\" " +
                           $"{requiredAttr} data-field=\"{col.Name}\" />";
            }
        }
    }

    /// <summary>
    /// Internal class to hold column definitions collected from child tag helpers
    /// </summary>
    public class DetailColumnDefinition
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public DetailColumnType ColumnType { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public string Placeholder { get; set; }
        public string Width { get; set; }
        public string CssClass { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Step { get; set; }
        public List<SelectListData> SelectItems { get; set; }
        public bool ShowTotal { get; set; }
        public bool IsTotalLabel { get; set; }
        public string TotalLabelText { get; set; }
        public string ComputeFormula { get; set; }
    }

    [HtmlTargetElement("master-field", ParentTag = "master-section")]
    public class MasterFieldTagHelper : TagHelper
    {
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("label")]
        public string Label { get; set; }

        [HtmlAttributeName("type")]
        public MasterFieldType FieldType { get; set; } = MasterFieldType.Text;

        [HtmlAttributeName("value")]
        public string Value { get; set; } = "";

        [HtmlAttributeName("placeholder")]
        public string Placeholder { get; set; } = "";

        [HtmlAttributeName("required")]
        public bool Required { get; set; } = false;

        [HtmlAttributeName("css-class")]
        public string CssClass { get; set; } = "";

        [HtmlAttributeName("col-span")]
        public int ColSpan { get; set; } = 1;

        [HtmlAttributeName("select-items")]
        public IEnumerable<SelectListData> SelectItems { get; set; }

        [HtmlAttributeName("min")]
        public string Min { get; set; }

        [HtmlAttributeName("max")]
        public string Max { get; set; }

        [HtmlAttributeName("step")]
        public string Step { get; set; }

        [HtmlAttributeName("maxlength")]
        public int? MaxLength { get; set; }

        [HtmlAttributeName("rows")]
        public int TextAreaRows { get; set; } = 3;

        [HtmlAttributeName("help-text")]
        public string HelpText { get; set; }

        [HtmlAttributeName("readonly")]
        public bool ReadOnly { get; set; } = false;

        [HtmlAttributeName("disabled")]
        public bool Disabled { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (FieldType == MasterFieldType.Hidden)
            {
                output.TagName = "input";
                output.Attributes.SetAttribute("type", "hidden");
                output.Attributes.SetAttribute("name", Name);
                output.Attributes.SetAttribute("id", Name);
                output.Attributes.SetAttribute("value", Value ?? "");
                output.TagMode = TagMode.SelfClosing;
                return;
            }

            output.TagName = "div";
            string spanClass = ColSpan > 1 ? $" md-col-span-{ColSpan}" : "";
            output.Attributes.SetAttribute("class", $"md-field-group{spanClass} {CssClass}".Trim());

            var sb = new StringBuilder();

            // Label
            sb.AppendLine($"<label class=\"md-label\" for=\"{Name}\">");
            sb.Append($"  {Label}");
            if (Required)
                sb.Append(" <span class=\"md-required\">*</span>");
            sb.AppendLine();
            sb.AppendLine("</label>");

            // Input
            sb.AppendLine(RenderInput());

            // Help text
            if (!string.IsNullOrEmpty(HelpText))
            {
                sb.AppendLine($"<small class=\"md-help-text\">{HelpText}</small>");
            }

            // Validation message
            sb.AppendLine($"<span class=\"md-field-validation\" data-valmsg-for=\"{Name}\"></span>");

            output.Content.SetHtmlContent(sb.ToString());
        }

        private string RenderInput()
        {
            var sb = new StringBuilder();
            string requiredAttr = Required ? "required=\"required\" data-val=\"true\" data-val-required=\"{Label} is required.\"" : "";
            string readonlyAttr = ReadOnly || FieldType == MasterFieldType.ReadOnly ? "readonly=\"readonly\"" : "";
            string disabledAttr = Disabled ? "disabled=\"disabled\"" : "";
            string maxLengthAttr = MaxLength.HasValue ? $"maxlength=\"{MaxLength}\" data-val-maxlength-max=\"{MaxLength}\" data-val-maxlength=\"Maximum {MaxLength} characters.\"" : "";

            switch (FieldType)
            {
                case MasterFieldType.TextArea:
                    sb.AppendLine($"<textarea class=\"md-input md-textarea\" id=\"{Name}\" name=\"{Name}\" " +
                        $"rows=\"{TextAreaRows}\" placeholder=\"{Placeholder}\" " +
                        $"{requiredAttr} {readonlyAttr} {disabledAttr} {maxLengthAttr}>{Value}</textarea>");
                    break;

                case MasterFieldType.Select:
                    sb.AppendLine($"<select class=\"md-input md-select\" id=\"{Name}\" name=\"{Name}\" " +
                        $"{requiredAttr} {disabledAttr}>");
                    sb.AppendLine($"  <option value=\"\">-- Select {Label} --</option>");
                    if (SelectItems != null)
                    {
                        foreach (var item in SelectItems)
                        {
                            string selected = item.Selected ? "selected=\"selected\"" : "";
                            sb.AppendLine($"  <option value=\"{item.Value}\" {selected}>{item.Text}</option>");
                        }
                    }
                    sb.AppendLine("</select>");
                    break;

                case MasterFieldType.Checkbox:
                    sb.AppendLine($"<div class=\"md-checkbox-wrapper\">");
                    string checkedAttr = Value?.ToLower() == "true" ? "checked=\"checked\"" : "";
                    sb.AppendLine($"  <input type=\"checkbox\" class=\"md-checkbox\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"true\" {checkedAttr} {disabledAttr} />");
                    sb.AppendLine($"  <input type=\"hidden\" name=\"{Name}\" value=\"false\" />");
                    sb.AppendLine($"</div>");
                    break;

                case MasterFieldType.Number:
                case MasterFieldType.Decimal:
                    string stepAttr = !string.IsNullOrEmpty(Step) ? $"step=\"{Step}\"" :
                        (FieldType == MasterFieldType.Decimal ? "step=\"0.01\"" : "step=\"1\"");
                    string minAttr = !string.IsNullOrEmpty(Min) ? $"min=\"{Min}\"" : "";
                    string maxAttr = !string.IsNullOrEmpty(Max) ? $"max=\"{Max}\"" : "";
                    sb.AppendLine($"<input type=\"number\" class=\"md-input md-number\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" placeholder=\"{Placeholder}\" " +
                        $"{stepAttr} {minAttr} {maxAttr} {requiredAttr} {readonlyAttr} {disabledAttr} />");
                    break;

                case MasterFieldType.Date:
                    sb.AppendLine($"<input type=\"date\" class=\"md-input md-date\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" {requiredAttr} {readonlyAttr} {disabledAttr} />");
                    break;

                case MasterFieldType.DateTime:
                    sb.AppendLine($"<input type=\"datetime-local\" class=\"md-input md-datetime\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" {requiredAttr} {readonlyAttr} {disabledAttr} />");
                    break;

                case MasterFieldType.Email:
                    sb.AppendLine($"<input type=\"email\" class=\"md-input\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" placeholder=\"{Placeholder}\" " +
                        $"{requiredAttr} {readonlyAttr} {disabledAttr} {maxLengthAttr} " +
                        $"data-val-email=\"Please enter a valid email.\" />");
                    break;

                case MasterFieldType.Password:
                    sb.AppendLine($"<input type=\"password\" class=\"md-input\" id=\"{Name}\" name=\"{Name}\" " +
                        $"placeholder=\"{Placeholder}\" " +
                        $"{requiredAttr} {disabledAttr} {maxLengthAttr} />");
                    break;

                case MasterFieldType.ReadOnly:
                    sb.AppendLine($"<input type=\"text\" class=\"md-input md-readonly\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" readonly=\"readonly\" tabindex=\"-1\" />");
                    break;

                default: // Text, Tel, Url, Color
                    string inputType = FieldType switch
                    {
                        MasterFieldType.Tel => "tel",
                        MasterFieldType.Url => "url",
                        MasterFieldType.Color => "color",
                        _ => "text"
                    };
                    sb.AppendLine($"<input type=\"{inputType}\" class=\"md-input\" id=\"{Name}\" name=\"{Name}\" " +
                        $"value=\"{Value}\" placeholder=\"{Placeholder}\" " +
                        $"{requiredAttr} {readonlyAttr} {disabledAttr} {maxLengthAttr} />");
                    break;
            }

            return sb.ToString();
        }
    }

    [HtmlTargetElement("master-section", ParentTag = "master-detail-form")]
    [RestrictChildren("master-field")]
    public class MasterSectionTagHelper : TagHelper
    {
        [HtmlAttributeName("title")]
        public string Title { get; set; } = "General Information";

        [HtmlAttributeName("columns")]
        public int Columns { get; set; } = 2;

        [HtmlAttributeName("section-class")]
        public string SectionClass { get; set; } = "";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Store columns info for child tag helpers
            context.Items["MasterColumns"] = Columns;

            var childContent = await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"md-master-section {SectionClass}".Trim());

            var sb = new StringBuilder();
            sb.AppendLine($"<div class=\"md-section-header\">");
            sb.AppendLine($"  <h4 class=\"md-section-title\">{Title}</h4>");
            sb.AppendLine($"</div>");
            sb.AppendLine($"<div class=\"md-master-fields md-grid md-grid-{Columns}\">");
            sb.AppendLine(childContent.GetContent());
            sb.AppendLine("</div>");

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
    [HtmlTargetElement("master-detail-form")]
    [RestrictChildren("master-section", "detail-section")]
    public class MasterDetailFormTagHelper : TagHelper
    {
        /// <summary>
        /// Form action URL
        /// </summary>
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        /// <summary>
        /// Form controller
        /// </summary>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>
        /// HTTP Method
        /// </summary>
        [HtmlAttributeName("method")]
        public string Method { get; set; } = "post";

        /// <summary>
        /// Form ID
        /// </summary>
        [HtmlAttributeName("form-id")]
        public string FormId { get; set; } = "masterDetailForm";

        /// <summary>
        /// Form Title
        /// </summary>
        [HtmlAttributeName("form-title")]
        public string FormTitle { get; set; }

        /// <summary>
        /// Submit button text
        /// </summary>
        [HtmlAttributeName("submit-text")]
        public string SubmitText { get; set; } = "Save";

        /// <summary>
        /// Cancel URL
        /// </summary>
        [HtmlAttributeName("cancel-url")]
        public string CancelUrl { get; set; }

        /// <summary>
        /// Cancel button text
        /// </summary>
        [HtmlAttributeName("cancel-text")]
        public string CancelText { get; set; } = "Cancel";

        /// <summary>
        /// Show cancel button
        /// </summary>
        [HtmlAttributeName("show-cancel")]
        public bool ShowCancel { get; set; } = true;

        /// <summary>
        /// CSS class for form
        /// </summary>
        [HtmlAttributeName("form-class")]
        public string FormClass { get; set; } = "";

        /// <summary>
        /// Enable client-side validation
        /// </summary>
        [HtmlAttributeName("enable-validation")]
        public bool EnableValidation { get; set; } = true;

        /// <summary>
        /// Anti-forgery token
        /// </summary>
        [HtmlAttributeName("antiforgery")]
        public bool Antiforgery { get; set; } = true;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Build form action URL
            var urlHelper = new Microsoft.AspNetCore.Mvc.Routing.UrlHelper(ViewContext);
            string actionUrl = "#";
            if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Controller))
            {
                actionUrl = $"/{Controller}/{Action}";
            }
            else if (!string.IsNullOrEmpty(Action))
            {
                actionUrl = $"{Action}";
            }

            output.TagName = "form";
            output.Attributes.SetAttribute("id", FormId);
            output.Attributes.SetAttribute("action", actionUrl);
            output.Attributes.SetAttribute("method", Method);
            output.Attributes.SetAttribute("class", $"md-form {FormClass}".Trim());

            if (EnableValidation)
            {
                output.Attributes.SetAttribute("novalidate", "novalidate");
            }

            var sb = new StringBuilder();

            // Anti-forgery token
            if (Antiforgery && Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                var antiforgery = ViewContext.HttpContext.RequestServices
                    .GetService(typeof(Microsoft.AspNetCore.Antiforgery.IAntiforgery))
                    as Microsoft.AspNetCore.Antiforgery.IAntiforgery;
                if (antiforgery != null)
                {
                    var tokenSet = antiforgery.GetAndStoreTokens(ViewContext.HttpContext);
                    sb.AppendLine($"<input type=\"hidden\" name=\"{tokenSet.FormFieldName}\" value=\"{tokenSet.RequestToken}\" />");
                }
            }

            // Form title
            if (!string.IsNullOrEmpty(FormTitle))
            {
                sb.AppendLine($"<div class=\"md-form-header\">");
                sb.AppendLine($"  <h2 class=\"md-form-title\">{FormTitle}</h2>");
                sb.AppendLine($"</div>");
            }

            // Validation summary
            if (EnableValidation)
            {
                sb.AppendLine("<div class=\"md-validation-summary\" style=\"display:none;\">");
                sb.AppendLine("  <ul></ul>");
                sb.AppendLine("</div>");
            }

            // Render child content (master-section and detail-section)
            var childContent = await output.GetChildContentAsync();
            sb.AppendLine(childContent.GetContent());

            // Action buttons
            sb.AppendLine("<div class=\"md-form-actions\">");
            sb.AppendLine($"  <button type=\"submit\" class=\"md-btn md-btn-primary\">");
            sb.AppendLine($"    <span class=\"md-btn-text\">{SubmitText}</span>");
            sb.AppendLine($"    <span class=\"md-btn-loading\" style=\"display:none;\">Saving...</span>");
            sb.AppendLine($"  </button>");

            if (ShowCancel)
            {
                string cancelHref = CancelUrl ?? "javascript:history.back()";
                sb.AppendLine($"  <a href=\"{cancelHref}\" class=\"md-btn md-btn-secondary\">{CancelText}</a>");
            }

            sb.AppendLine("</div>");

            output.Content.SetHtmlContent(sb.ToString());
        }
    }



    public enum DetailColumnType
    {
        Text,
        Number,
        Decimal,
        Select,
        Date,
        Hidden,
        Checkbox,
        TextArea,
        Email,
        Tel,
        Url,
        Color,
        ReadOnly
    }

    public enum MasterFieldType
    {
        Text,
        Number,
        Decimal,
        Select,
        Date,
        DateTime,
        Hidden,
        Checkbox,
        TextArea,
        Email,
        Tel,
        Url,
        Color,
        ReadOnly,
        Password
    }

    public class SelectListData
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }


}
