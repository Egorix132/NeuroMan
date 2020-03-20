using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeuroMan.Models;
using NeuroMan.Services;

namespace NeuroMan.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly RoomService roomService;

        public AuthController(ILogger<AuthController> logger, RoomService roomService)
        {
            _logger = logger;
            this.roomService = roomService;
        }


        [Route("/Auth")]
        public string Auth(User user)
        {
            if (user.Room.Length == 0)
            {
                user.Name = user.Name.Length == 0 ? GenerateRandomName(9) : user.Name;
                user.Room = roomService.FindFreeRoom(user.Name);
            }  
            else
            {
                if (roomService.ContainsRoom(user.Room))
                {
                    Room room = roomService.GetRoom(user.Room);

                    if (room.IsFull())
                    {
                        return "Room is full";
                    }
                    if (user.Name.Length != 0)
                    {
                        if (room.ContainsParticipant(user.Name))
                            return "This name is already taken";
                    }
                    else
                        do
                            user.Name = GenerateRandomName(9);
                        while (room.ContainsParticipant(user.Name));    
                        
                }
                else
                    roomService.AddRoom(user.Room);
            }

            Response.Cookies.Append("name", user.Name);
            Response.Cookies.Append("room", user.Room);

            return "ok";
        }

        /*private string GenerateRandomName(int lenght)
        {
            Random r = new Random();
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < lenght; i++)
            {
                s.Append(r.Next(0, 255));
            }
            return s.ToString();
        }*/

        private string GenerateRandomName(int length)
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, length);  // Return 8 character string
        }

    }

    public class User
    {
        public string Name { get; set; }
        public string Room { get; set; }
    }

    
}

