using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeuroMan.Models;
using NeuroMan.Services;
using Microsoft.AspNetCore.Http;

namespace NeuroMan.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        RoomService roomService;

        
        public HomeController(ILogger<HomeController> logger, RoomService roomService)
        {
            _logger = logger;
            this.roomService = roomService;
        }

        [Route("Home/Index")]
        [Route("")]
        public IActionResult Index(int? id)
        {
            var room = roomService.GetRoom((string)HttpContext.Items["room"]);
            room.Join(HttpContext.Connection.RemoteIpAddress.ToString());
            ViewBag.inputValues = room.GetInputValues();
            ViewBag.countOutputs = room.GetOutputs();
            ViewBag.users = room.GetParticipants().Values;
            ViewBag.user = room.GetParticipants()[HttpContext.Connection.RemoteIpAddress.ToString()];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
