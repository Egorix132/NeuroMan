using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroMan.Models
{
    public class Room
    {
        public bool isPrivate = false;
        public string Name { get; set; }

        private int maxUsers = 5;

        private readonly Dictionary<string, Participant> participants = new Dictionary<string, Participant>();

        public NeuralNetwork neuralNetwork;

        public Room(string name)
        {
            Name = name;
            neuralNetwork = new NeuralNetwork();
        }

        public int CountOfUsers()
        {
            return participants.Count;
        }

        public bool IsFull()
        {
            return CountOfUsers() >= maxUsers;
        }

        public void Join(string name, string ip)
        {
            if (!participants.Any(p => p.Value.Name == name))
                participants.Add(name, new Participant(name, ip, new List<double>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new List<double>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        public void Unjoin(string name)
        {
            if (participants.Any(p => p.Value.Name == name))
            {
                participants[name].IsReady = false;
                participants.Remove(name);
                neuralNetwork.RemoveWeights(name);
            }      
        }

        public bool ChangeReadiness(string name)
        {
            if (participants.Any(p => p.Value.Name == name))
            {
                participants[name].IsReady = !participants[name].IsReady;
            }

            if (participants.All(p => p.Value.IsReady))
            {
                neuralNetwork.CalculateOutput();
                return true;
            }

            else return false;
        }

        public Dictionary<string, Participant> GetParticipants()
        {
            return participants;
        }

        public bool ContainsParticipant(string name)
        {
            foreach(Participant p in participants.Values)
                if (p.Name == name) return true;

            return false;
        }
    }
}
