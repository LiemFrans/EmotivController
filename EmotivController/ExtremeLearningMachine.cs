using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
namespace EmotivController
{
    class ExtremeLearningMachine
    {
        private int hiddenLayer;
        private Random random = new Random();
        private double[,] inputs;
        private double[,] weight;
        private double[] betaHatt;

        public ExtremeLearningMachine(int hiddenLayer)
        {
            this.hiddenLayer = hiddenLayer;
        }
        public void teacher(double[][] inputs, double[] output)
        {
            this.inputs = convertToArray2DJagged(inputs);
            this.weight = generateWeight();
            var hInit = this.inputs.MultiplyByTranspose(weight);
            var h = new double[hInit.GetLength(0), hInit.GetLength(1)];
            for (int i = 0; i < h.GetLength(0); i++)
            {
                for (int j = 0; j < h.GetLength(1); j++)
                {
                    h[i, j] = 1 / (1 + Math.Exp(-hInit[i, j]));
                }
            }
            var htranspose = h.Transpose();
            var hPlus = ((h.TransposeAndDot(h)).PseudoInverse()).MultiplyByTranspose(h);
            betaHatt = hPlus.Dot(output);
            var yHatt = h.Dot(betaHatt);
            Console.WriteLine("MAPE EVAL : "+MAPEEval(yHatt, output.Select(x => (double)x).ToArray()));
        }
        public double[] Decide(double[][] inputs)
        {
            var f = convertToArray2DJagged(inputs);
            return f.Dot(betaHatt);
        }
        private double MAPEEval(double[] yHatt, double[] y)
        {
            var temp = 0.0;
            for (int i = 0; i < y.Length; i++)
            {
                temp += Math.Abs(((yHatt[i] - y[i]) / y[i]) * 100);
            }
            return (1.0 / Convert.ToDouble(y.Length)) * temp;
        }
        private double[,] convertToArray2DJagged(double[][]inputs)
        {
            var array2DJagged = new double[inputs.Length,inputs[0].Length];
            for(int i =0; i < inputs.Length; i++)
            {
                for(int j = 0; j < inputs[0].Length; j++)
                {
                    array2DJagged[i, j] = inputs[i][j];
                }
            }
            return array2DJagged;
        }
        private double [,] generateWeight()
        {
            var weight = new double[inputs.Length,hiddenLayer];
            for(int i = 0; i < inputs.GetLength(0);i++)
            {
                for(int j = 0; j < this.hiddenLayer; j++)
                {
                    weight[i, j] = GetRandomNumber(-0.5, 0.5);
                }
            }
            return weight;
        }

        private double GetRandomNumber(double minimum, double maximum)
        {
            var next = random.NextDouble();
            return minimum + (next * (maximum - minimum));
        }
    }
}
