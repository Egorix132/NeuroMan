using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMan.Models;

namespace NeuroMan.Services
{
    public class RoomService
    {
        Dictionary<string, Room> rooms = new Dictionary<string, Room>();
        private Dictionary<string, Participant> archive = new Dictionary<string, Participant>();

        public string FindRoom()
        {
            foreach (KeyValuePair<string, Room> room in rooms)
            {
                if (!room.Value.IsFull())
                {
                    return room.Key;
                }
            }
            return null;
        }

        public Room GetRoom(string name)
        {
            if(rooms.ContainsKey(name)) return rooms[name];
            
            return null;
        }

        public string AddRoom(string name = null)
        {
            string _name = name;
            if (_name == null) _name = GenerateRoomName(9);
            rooms.Add(_name, new Room(_name));
            return _name;
        }

        private string GenerateRoomName(int lenght)
        {
            Random r = new Random();
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < lenght; i++)
            {
                s.Append(r.Next(0, 255));
            }
            return s.ToString();
        }
    }
}
