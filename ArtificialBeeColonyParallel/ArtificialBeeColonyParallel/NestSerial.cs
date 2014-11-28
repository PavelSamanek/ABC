using System;
using System.Threading;

namespace ArtificialBeeColonyParallel
{
    // Serial implementation of ABC algorithm
    internal class NestSerial
    {
        private Random random; // multipurpose
        private Timer timer; 

        // probability an active bee will reject a better neighbor food source OR accept worse neighbor food source

        public Bee[] bees;
        public double bestMeasureOfQuality;
        public char[] bestMemoryMatrix; // problem-specific
        public int[] indexesOfInactiveBees; // contains indexes into the bees array
        public int maxNumberCycles; // one cycle represents an action by all bees in the hive
        //public int maxCyclesWithNoImprovement; // deprecated

        public int maxNumberVisits;
        public int numberActive;
        public int numberInactive;
        public int numberScout;
        public Data pointsData; // this is the problem-specific data we want to optimize
        public double probMistake = 0.05;
        public double probPersuasion = 0.90; // probability inactive bee is persuaded by better waggle solution
        public int totalNumberBees; // mostly for readability in the object constructor call

        public NestSerial(int totalNumberBees, int numberInactive, int numberActive, int numberScout,
            int maxNumberVisits,
            int maxNumberCycles, Data pointsData)
        {
            random = new Random(0);

            this.totalNumberBees = totalNumberBees;
            this.numberInactive = numberInactive;
            this.numberActive = numberActive;
            this.numberScout = numberScout;
            this.maxNumberVisits = maxNumberVisits;
            this.maxNumberCycles = maxNumberCycles;
            //this.maxCyclesWithNoImprovement = maxCyclesWithNoImprovement;

            //this.citiesData = new CitiesData(citiesData.points.Length); // hive's copy of problem-specific data
            this.pointsData = pointsData; // reference to CityData

            // this.probPersuasion & this.probMistake are hard-coded in class definition

            bees = new Bee[totalNumberBees];
            bestMemoryMatrix = GenerateRandomSolution(); // alternative initializations are possible
            bestMeasureOfQuality = MeasureOfQuality(bestMemoryMatrix);

            indexesOfInactiveBees = new int[numberInactive]; // indexes of bees which are currently inactive

            for (int i = 0; i < totalNumberBees; ++i) // initialize each bee, and best solution
            {
                beeState currStatus; // depends on i. need status before we can initialize Bee
                if (i < numberInactive)
                {
                    currStatus = beeState.INACTIVE; // inactive
                    indexesOfInactiveBees[i] = i; // curr bee is inactive
                }
                else if (i < numberInactive + numberScout)
                {
                    currStatus = beeState.SCOUT; // scout
                }
                else
                {
                    currStatus = beeState.ACTIVE; // active
                }

                char[] randomMemoryMatrix = GenerateRandomSolution();
                double mq = MeasureOfQuality(randomMemoryMatrix);
                const int numberOfVisits = 0;

                bees[i] = new Bee(currStatus, randomMemoryMatrix, mq, numberOfVisits); // instantiate current bee

                // does this bee have best solution?
                if (bees[i].qualityOfSolution < bestMeasureOfQuality)
                    // curr bee is better (< because smaller is better)
                {
                    Array.Copy(bees[i].memorizedSolution, bestMemoryMatrix, bees[i].memorizedSolution.Length);
                    bestMeasureOfQuality = bees[i].qualityOfSolution;
                }
            } // each bee
        }

        public override string ToString()
        {
            string s = "";
            s += "Best path found: ";
            for (int i = 0; i < bestMemoryMatrix.Length - 1; ++i)
                s += bestMemoryMatrix[i] + "->";
            s += bestMemoryMatrix[bestMemoryMatrix.Length - 1] + "\n";

            s += "Path quality:    ";
            if (bestMeasureOfQuality < 10000.0)
                s += bestMeasureOfQuality.ToString("F4") + "\n";
            else
                s += bestMeasureOfQuality.ToString("#.####e+00");
            s += "\n";
            return s;
        }

        // TravelingSalesmanHive ctor


        public char[] GenerateRandomSolution()
        {
            var result = new char[pointsData.points.Length]; // // problem-specific
            Array.Copy(pointsData.points, result, pointsData.points.Length);

            for (int i = 0; i < result.Length; i++) // Fisher-Yates (Knuth) shuffle
            {
                int r = random.Next(i, result.Length);
                char temp = result[r];
                result[r] = result[i];
                result[i] = temp;
            }
            return result;
        } // GenerateRandomSolution()

        public char[] GenerateNeighbourSolution(char[] memoryMatrix)
        {
            var result = new char[memoryMatrix.Length];
            Array.Copy(memoryMatrix, result, memoryMatrix.Length);

            int ranIndex = random.Next(0, result.Length); // [0, Length-1] inclusive
            int adjIndex;
            if (ranIndex == result.Length - 1)
                adjIndex = 0;
            else
                adjIndex = ranIndex + 1;

            char tmp = result[ranIndex];
            result[ranIndex] = result[adjIndex];
            result[adjIndex] = tmp;

            return result;
        } // GenerateNeighbourSolution()

        public double MeasureOfQuality(char[] memoryMatrix)
        {
            double answer = 0.0;
            for (int i = 0; i < memoryMatrix.Length - 1; ++i)
            {
                char c1 = memoryMatrix[i];
                char c2 = memoryMatrix[i + 1];
                double d = pointsData.Distance(c1, c2);
                answer += d;
            }
            return answer;
        } // MeasureOfQuality()

        public void Solve(bool doProgressBar) // find best Traveling Salesman Problem solution
        {
            bool pb = doProgressBar; // just want a shorter variable
            const int numberOfSymbolsToPrint = 10; // 10 units so each symbol is 10.0% progress
            int increment = maxNumberCycles/numberOfSymbolsToPrint;
            if (pb) Console.WriteLine("\nEntering SBC Traveling Salesman Problem algorithm main processing loop\n");
            if (pb) Console.WriteLine("Progress: |==========|"); // 10 units so each symbol is 10% progress
            if (pb) Console.Write("           ");
            int cycle = 0;

            while (cycle < maxNumberCycles)
            {
                for (int i = 0; i < totalNumberBees; ++i) // each bee
                {
                    if (bees[i].status == beeState.ACTIVE) // active bee
                        ProcessActiveBee(i);
                    else if (bees[i].status == beeState.SCOUT) // scout bee
                        ProcessScoutBee(i);
                    else if (bees[i].status == beeState.INACTIVE) // inactive bee
                        ProcessInactiveBee(i);
                } // for each bee
                ++cycle;

                // print a progress bar
                if (pb && cycle%increment == 0)
                    Console.Write("^");
            } // main while processing loop

            if (pb) Console.WriteLine(""); // end the progress bar
        } // Solve()

        private void ProcessInactiveBee(int i)
        {
        }

        private void ProcessActiveBee(int i)
        {
            char[] neighbor = GenerateNeighbourSolution(bees[i].memorizedSolution); // find a neighbor solution
            double neighborQuality = MeasureOfQuality(neighbor); // get its quality
            double prob = random.NextDouble();
            // used to determine if bee makes a mistake; compare against probMistake which has some small value (~0.01)
            bool memoryWasUpdated = false; // used to determine if bee should perform a waggle dance when done
            bool numberOfVisitsOverLimit = false; // used to determine if bee will convert to inactive status

            if (neighborQuality < bees[i].qualityOfSolution)
                // active bee found better neighbor (< because smaller values are better)
            {
                if (prob < probMistake) // bee makes mistake: rejects a better neighbor food source
                {
                    ++bees[i].numberOfVisits; // no change to memory but update number of visits
                    if (bees[i].numberOfVisits > maxNumberVisits) numberOfVisitsOverLimit = true;
                }
                else // bee does not make a mistake: accepts a better neighbor
                {
                    Array.Copy(neighbor, bees[i].memorizedSolution, neighbor.Length);
                    // copy neighbor location into bee's memory
                    bees[i].qualityOfSolution = neighborQuality; // update the quality
                    bees[i].numberOfVisits = 0; // reset counter
                    memoryWasUpdated = true; // so that this bee will do a waggle dance 
                }
            }
            else // active bee did not find a better neighbor
            {
                //Console.WriteLine("c");
                if (prob < probMistake) // bee makes mistake: accepts a worse neighbor food source
                {
                    Array.Copy(neighbor, bees[i].memorizedSolution, neighbor.Length);
                    // copy neighbor location into bee's memory
                    bees[i].qualityOfSolution = neighborQuality; // update the quality
                    bees[i].numberOfVisits = 0; // reset
                    memoryWasUpdated = true; // so that this bee will do a waggle dance 
                }
                else // no mistake: bee rejects worse food source
                {
                    ++bees[i].numberOfVisits;
                    if (bees[i].numberOfVisits > maxNumberVisits) numberOfVisitsOverLimit = true;
                }
            }

            // at this point we need to determine a.) if the number of visits has been exceeded in which case bee becomes inactive
            // or b.) memory was updated in which case check to see if new memory is a global best, and then bee does waggle dance
            // or c.) neither in which case nothing happens (bee just returns to hive).

            if (numberOfVisitsOverLimit)
            {
                bees[i].status = 0; // current active bee transitions to inactive
                bees[i].numberOfVisits = 0; // reset visits (and no change to this bees memory)
                int x = random.Next(numberInactive);
                // pick a random inactive bee. x is an index into a list, not a bee ID
                bees[indexesOfInactiveBees[x]].status = beeState.ACTIVE; // make it active
                indexesOfInactiveBees[x] = i; // record now-inactive bee 'i' in the inactive list
            }
            else if (memoryWasUpdated) // current bee returns and performs waggle dance
            {
                // first, determine if the new memory is a global best. note that if bee has accepted a worse food source this can't be true
                if (bees[i].qualityOfSolution < bestMeasureOfQuality)
                    // the modified bee's memory is a new global best (< because smaller is better)
                {
                    Array.Copy(bees[i].memorizedSolution, bestMemoryMatrix, bees[i].memorizedSolution.Length);
                    // update global best memory
                    bestMeasureOfQuality = bees[i].qualityOfSolution; // update global best quality
                }
                DoWaggleDance(i);
            }
            else
                // number visits is not over limit and memory was not updated so do nothing (return to hive but do not waggle)
            {
            }
        } // ProcessActiveBee()

        private void ProcessScoutBee(int i)
        {
            char[] randomFoodSource = GenerateRandomSolution(); // scout bee finds a random food source. . . 
            double randomFoodSourceQuality = MeasureOfQuality(randomFoodSource); // and examines its quality
            if (randomFoodSourceQuality < bees[i].qualityOfSolution)
                // scout bee has found a better solution than its current one (< because smaller measure is better)
            {
                Array.Copy(randomFoodSource, bees[i].memorizedSolution, randomFoodSource.Length);
                // unlike active bees, scout bees do not make mistakes
                bees[i].qualityOfSolution = randomFoodSourceQuality;
                // no change to scout bee's numberOfVisits or status

                // did this scout bee find a better overall/global solution?
                if (bees[i].qualityOfSolution < bestMeasureOfQuality)
                    // yes, better overall solution (< because smaller is better)
                {
                    Array.Copy(bees[i].memorizedSolution, bestMemoryMatrix, bees[i].memorizedSolution.Length);
                    // copy scout bee's memory to global best
                    bestMeasureOfQuality = bees[i].qualityOfSolution;
                } // better overall solution

                DoWaggleDance(i); // scout returns to hive and does waggle dance
            } // if scout bee found better solution
        } // ProcessScoutBee()

        private void DoWaggleDance(int i)
        {
            for (int ii = 0; ii < numberInactive; ++ii) // each inactive/watcher bee
            {
                int b = indexesOfInactiveBees[ii]; // index of an inactive bee
                if (bees[b].status != 0) throw new Exception("Catastrophic logic error when scout bee waggles dances");
                if (bees[b].numberOfVisits != 0)
                    throw new Exception(
                        "Found an inactive bee with numberOfVisits != 0 in Scout bee waggle dance routine");
                if (bees[i].qualityOfSolution < bees[b].qualityOfSolution)
                    // scout bee has a better solution than current inactive/watcher bee (< because smaller is better)
                {
                    double p = random.NextDouble(); // will current inactive bee be persuaded by scout's waggle dance?
                    if (probPersuasion > p)
                        // this inactive bee is persuaded by the scout (usually because probPersuasion is large, ~0.90)
                    {
                        Array.Copy(bees[i].memorizedSolution, bees[b].memorizedSolution,
                            bees[i].memorizedSolution.Length);
                        bees[b].qualityOfSolution = bees[i].qualityOfSolution;
                    } // inactive bee has been persuaded
                } // scout bee has better solution than watcher/inactive bee
            } // each inactive bee
        } // DoWaggleDance()
    } // class ShortestPathHive
}