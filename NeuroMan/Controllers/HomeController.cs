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
        private readonly RoomService roomService;

        
        public HomeController(ILogger<HomeController> logger, RoomService roomService)
        {
            _logger = logger;
            this.roomService = roomService;
        }

        [Route("Home/Index")]
        [Route("")]
        public IActionResult Index()
        {  
            return View(CheckAuth());
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

        private bool CheckAuth()
        {
            bool auth = HttpContext.Request.Cookies.ContainsKey("name") && HttpContext.Request.Cookies.ContainsKey("room");
            if (auth)
            {
                string userName = HttpContext.Request.Cookies["name"];
                string roomName = HttpContext.Request.Cookies["room"];

                if (roomService.ContainsRoom(roomName))
                {
                    Room room = roomService.GetRoom(roomName);

                    if (room.IsFull() || (room.ContainsParticipant(userName) && room.GetParticipants()[userName].Ip != HttpContext.Connection.RemoteIpAddress.ToString()))
                        auth = false;
                }
                else
                    roomService.AddRoom(roomName);
            }

            return auth;
        }
    }
}
