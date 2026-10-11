using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("autocomplete-search")]
    public class AutocompleteTagHelper : TagHelper
    {
        public string Label { get; set; } = "Search"; // Customizable Label
        public string Name { get; set; } = "Name";    // Input Name (for Model Binding)
        public string HiddenName { get; set; } = "Id"; // Hidden Input Name
        public string Value { get; set; }
        public string HiddenValue { get; set; }
        public string Placeholder { get; set; } = "Type to search...";
        public string ApiUrl { get; set; } // The API endpoint for this specific field

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "mb-3 col-md-6");

            var html = $@"
            <label class='form-label'>{Label} <span class='text-danger'>*</span></label>
            <div class='autocomplete-container position-relative'>
                <input type='text' 
                       name='{Name}' 
                       class='form-control' 
                       placeholder='{Placeholder}' 
                       value='{Value}' 
                       autocomplete='off' 
                       data-autocomplete='true' 
                       data-url='{ApiUrl}' />
                
                <input type='hidden' name='{HiddenName}' value='{HiddenValue}' class='autocomplete-hidden' />
                
                <div class='list-group position-absolute w-100 shadow autocomplete-results' 
                     style='z-index:9999; max-height:200px; overflow-y:auto; display:none;'>
                </div>
            </div>";

            output.Content.SetHtmlContent(html);
        }
    }

}
