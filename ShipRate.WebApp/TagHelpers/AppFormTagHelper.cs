using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("app-form")]
    public class AppFormTagHelper : TagHelper
    {
        public string Method { get; set; } = "post";
        public string Action { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "form";
            output.Attributes.SetAttribute("method", Method);

            if (!string.IsNullOrEmpty(Action))
                output.Attributes.SetAttribute("action", Action);

            output.Attributes.Add("class", "p-3 border rounded");
        }
    }


    [HtmlTargetElement("app-button")]
    public class AppButtonTagHelper : TagHelper
    {
        public string Text { get; set; } = "Submit";
        public string Class { get; set; } = "btn-primary";
        public string Type { get; set; } = "submit";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";
            output.Attributes.SetAttribute("type", Type);
            output.Attributes.SetAttribute("class", $"btn {Class}");
            output.Content.SetContent(Text);
        }
    }


    [HtmlTargetElement("app-input")]
    public class AppInputTagHelper : TagHelper
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Width { get; set; } = "100%";
        public string Type { get; set; } = "text";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "input";

            output.Attributes.SetAttribute("type", Type);
            output.Attributes.SetAttribute("name", Name);
            output.Attributes.SetAttribute("value", Value ?? "");
            output.Attributes.SetAttribute("class", "form-control");
            output.Attributes.SetAttribute("style", $"width:{Width};");
        }
    }


    [HtmlTargetElement("custom-input", Attributes = "asp-for")]
    public class CustomInputTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }
        public string display {  get; set; }


        public string Width { get; set; } = "200px";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var name = For.Name;
            var value = For.Model?.ToString() ?? "";
            var displayName = For.Metadata.DisplayName ?? For.Name;
            if (string.IsNullOrEmpty(display))
            {
                display = displayName;
            }
            output.TagName = "div";
            output.Content.SetHtmlContent($@"
            <label>{display}</label><br/>
            <input name='{name}' value='{value}' style='width:{Width}; padding:5px;' />
        ");
        }
    }
}