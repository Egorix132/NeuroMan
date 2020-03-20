using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroMan.Components
{
    public class SetNameViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var tuple = (Request.Query.ContainsKey("name")
                ? Request.Query["name"].ToString()
                : Request.Cookies.ContainsKey("name") ? Request.Cookies["name"] : "",
                Request.Query.ContainsKey("room")
                ? Request.Query["room"].ToString()
                : Request.Cookies.ContainsKey("room") ? Request.Cookies["room"] : "");
            return View(tuple);
        }
    }
}
