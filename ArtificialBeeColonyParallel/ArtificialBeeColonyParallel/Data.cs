using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialBeeColonyParallel
{
    internal class Data
    {
        public char[] points;

        public Data(int pointsCount)
        {
            this.points = new char[pointsCount];
            this.points[0] = 'A';
            for (int i = 1; i < this.points.Length; ++i)
                this.points[i] = (char) (this.points[i - 1] + 1);
        }

        public double Distance(char firstPoint, char secondPoint)
        {
            if (firstPoint < secondPoint)
                return 1.0*((int) secondPoint - (int) firstPoint);
            else
                return 1.5*((int) firstPoint - (int) secondPoint);
        }

        public double ShortestPathLength()
        {
            return 1.0*(this.points.Length - 1);
        }

        public long NumberOfPossiblePaths()
        {
            long n = this.points.Length;
            long answer = 1;
            for (int i = 1; i <= n; ++i)
                checked
                {
                    answer *= i;
                }
            return answer;
        }

        public override string ToString()
        {
            string s = "";
            s += "Points: ";
            for (int i = 0; i < this.points.Length; ++i)
                s += this.points[i] + " ";
            return s;
        }
    }
}
