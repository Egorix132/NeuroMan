using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroMan.Models;

namespace NeuroMan.Services
{
    public class RoomService
    {
        readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public string FindFreeRoom(string userName)
        {
            foreach (KeyValuePair<string, Room> room in rooms)
                if (!room.Value.IsFull() && !room.Value.ContainsParticipant(userName))
                    return room.Key;

            return AddRoom();
        }

        public Room GetRoom(string name)
        {
            if(rooms.ContainsKey(name)) return rooms[name];
            
            return null;
        }

        public Room GetRoomByParticipant(string name) => rooms.Values.ToList().Find(r => r.ContainsParticipant(name));

        public string AddRoom(string name = null)
        {
            string _name = name;
            if (_name == null) _name = GenerateRoomName(9);
            rooms.Add(_name, new Room(_name));
            return _name;
        }

        private string GenerateRoomName(int length)
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, length);  // Return 8 character string
        }

        public bool ContainsRoom(string name) => rooms.ContainsKey(name);

        public void DeleteFreeRooms()
        {
            foreach(KeyValuePair<string, Room> room in rooms)
                if(room.Value.GetParticipants().Count <= 0)
                    rooms.Remove(room.Key);
        }
    }
}
