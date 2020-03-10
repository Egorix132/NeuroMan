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

        private Dictionary<string, Participant> participants = new Dictionary<string, Participant>();
        private Dictionary<string, Participant> archive = new Dictionary<string, Participant>();

        private List<double> inputValues = new List<double>();
        private List<double> outputValues = new List<double>();

        public int answer;

        public Room(string name)
        {
            this.Name = name;
            GenerateInput(9);
            outputValues = new List<double>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0};
        }

        public int CountOfUsers()
        {
            return participants.Count;
        }

        public bool IsFull()
        {
            return CountOfUsers() >= maxUsers;
        }

        public void Join(string ip)
        {
            if (archive.Any(p => p.Value.ip == ip) && !participants.Any(p => p.Value.ip == ip))
                participants.Add(ip, archive[ip]);
            else if (!participants.Any(p => p.Value.ip == ip))
            {
                archive.Add(ip, new Participant(ip, new List<double>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new List<double>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
                participants.Add(ip, archive[ip]);
            }
            
        }

        public void Unjoin(string ip)
        {
            if (participants.Any(p => p.Value.ip == ip))
                participants.Remove(ip);
        }

        public bool SetReady(string ip)
        {
            if (participants.Any(p => p.Value.ip == ip))
            {
                participants[ip].isReady = !participants[ip].isReady;
            }

            if (participants.All(p => p.Value.isReady))
            {
                CalculateOutput();
                return true;
            }
            else return false;
        }

        public List<double> GetInputValues()
        {
            return inputValues;
        }

        public int GetOutputs()
        {
            return outputValues.Count;
        }
        public Dictionary<string, Participant> GetParticipants()
        {
            return participants;
        }

        public void GenerateInput(int number)
        {
            inputValues.Clear();
            Random rand = new Random();
            for(int i = 0; i < number; i++)
            {
                inputValues.Add(rand.Next(0,2));
            }
        }

        private void CalculateOutput()
        {
            foreach(KeyValuePair<string, Participant> pair in participants)
            {
                double MainValue = 0;
                for(int j = 0; j < inputValues.Count; j++)
                {
                    MainValue += inputValues[j] * pair.Value.inputWeights[j];
                }

                for(int j = 0; j < outputValues.Count; j++)
                {
                    outputValues[j] = MainValue * pair.Value.outputWeights[j];
                }
            }

            answer = outputValues.IndexOf(outputValues.Max());
        }
    }
}
