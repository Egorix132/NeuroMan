using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroMan.Models
{
    public class Participant
    {
        public Participant(string ip, List<double> inputWeights, List<double> outputWeights)
        {
            this.ip = ip;
            this.inputWeights = inputWeights;
            this.outputWeights = outputWeights;
        }

        public string ip { get; set; }

        public bool isReady { get; set; }


        public List<double> inputWeights = new List<double>();

        public List<double> outputWeights = new List<double>();
    }
}
