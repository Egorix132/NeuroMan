using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroMan.Models
{
    public class Participant
    {
        public Participant(string name, string ip, List<double> inputWeights, List<double> outputWeights)
        {
            Name = name;
            Ip = ip;
            IsReady = false;
        }

        public string Name { get; set; }
        public string Ip { get; set; }
        public bool IsReady { get; set; }
    }
}
