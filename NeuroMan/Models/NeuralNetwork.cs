using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroMan.Models
{
    public class NeuralNetwork
    {
        private static readonly List<List<double>> numbers = new List<List<double>>(10) {
            new List<double>(15) {1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1}, // 0
            new List<double>(15) {0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1}, // 1
            new List<double>(15) {1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1}, // 2
            new List<double>(15) {1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1}, // 3
            new List<double>(15) {1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1}, // 4
            new List<double>(15) {1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1}, // 5
            new List<double>(15) {1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1}, // 6
            new List<double>(15) {1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0}, // 7
            new List<double>(15) {1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1}, // 8
            new List<double>(15) {1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1}  // 9
        };

        private List<double> inputValues = new List<double>();
        private List<double> outputValues = new List<double>();

        private readonly Dictionary<string, List<double>> inputWeights = new Dictionary<string, List<double>>();
        private readonly Dictionary<string, List<double>> outputWeights = new Dictionary<string, List<double>>();

        public int Answer { get; set; }

        public NeuralNetwork()
        {
            GenerateInput();
            outputValues = new List<double>(10) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public void GenerateInput()
        {
            inputValues.Clear();
            Random rand = new Random();
            int x = rand.Next(0, 11);
            if(x < 10)
            {
                inputValues = new List<double>(numbers[x]);
            }
            else
            {
                for (int i = 0; i < 15; i++)
                {
                    inputValues.Add(double.Parse(rand.NextDouble().ToString().Substring(0, 5)));
                }
            }
        }

        public List<double> GetInputValues()
        {
            return inputValues;
        }

        public int GetOutputCount()
        {
            return outputValues.Count;
        }

        public void SetWeights(string name, List<double> inputWeights, List<double> outputWeights)
        {
            this.inputWeights[name] = inputWeights;
            this.outputWeights[name] = outputWeights;
        }

        public void RemoveWeights(string name)
        {
            inputWeights.Remove(name);
            outputWeights.Remove(name);
        }

        public void CalculateOutput()
        {
            double outputSum = 0;

            for (int i = 0; i < inputWeights.Count; i++)
            {
                double MainValue = 0;
                for (int j = 0; j < inputValues.Count; j++)
                {
                    MainValue += inputValues[j] * inputWeights.Values.ToList()[i][j];
                }

                MainValue = Sigmoid(MainValue - 1);

                outputValues = outputValues.Select(v => 0d).ToList();

                for (int j = 0; j < outputValues.Count; j++)
                {
                    outputValues[j] += MainValue * outputWeights.Values.ToList()[i][j];
                    outputSum += outputValues[j];
                }    
            }

            outputValues.ForEach(w => w = Sigmoid(w - 1));

            outputSum = outputValues.Sum();

            for (int j = 0; j < outputValues.Count; j++)
            {
                outputValues[j] = outputValues[j] / outputSum;
            }
            
            Answer = outputValues.IndexOf(outputValues.Max());
        }

        private static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0f + k);
        } 
    }
}
