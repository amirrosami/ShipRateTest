using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace ShipRate.WebApp.Configs
{
    public class CustomMetadataProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.ModelType == typeof(string))
            {
                context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}