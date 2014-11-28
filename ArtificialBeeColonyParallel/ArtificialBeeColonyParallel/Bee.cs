using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialBeeColonyParallel
{
    internal enum beeState
    {
        INACTIVE,
        ACTIVE,
        SCOUT
    }

    internal class Bee
    {
        public beeState status;
        public char[] memorizedSolution;
        public double qualityOfSolution;
        public int numberOfVisits;

        public Bee(beeState status, char[] memoryMatrix, double measureOfQuality, int numberOfVisits)
        {
            this.status = status;
            this.memorizedSolution = new char[memoryMatrix.Length];
            Array.Copy(memoryMatrix, this.memorizedSolution, memoryMatrix.Length);
            this.qualityOfSolution = measureOfQuality;
            this.numberOfVisits = numberOfVisits;
        }

        public override string ToString()
        {
            string s = "";
            s += "Status = " + this.status + "\n";
            s += " Memory = " + "\n";
            for (int i = 0; i < this.memorizedSolution.Length - 1; ++i)
                s += this.memorizedSolution[i] + "->";
            s += this.memorizedSolution[this.memorizedSolution.Length - 1] + "\n";
            s += " Quality = " + this.qualityOfSolution.ToString("F4");
            s += " Number visits = " + this.numberOfVisits;
            return s;

        } 
    }
}
