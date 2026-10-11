using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ShipRate.WebAppCustomers.Controllers
{
    public class ApplicationController :Controller
    {
        protected string ErrorPartialViewName = "";
        protected void AddModelStateErrors(List<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected void HxTrigger(object triggers)
        {
            Response.Headers.Append("HX-Trigger", JsonSerializer.Serialize(triggers));
        }

        protected PartialViewResult ErrorPartialView(List<string> errors)
        {
            AddModelStateErrors(errors);
            return PartialView(ErrorPartialViewName);
        }


    }
}
