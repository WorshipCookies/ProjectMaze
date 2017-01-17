using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration
{

    // Combining different curves of tension -- This Fitness sums all of the associated fitnesses
    public class CombinedTensionFitnesses : IFitness
    {
        private List<IFitness> fitnesses;
        private ILogger logger;

        // Different Fitness curves!
        public CombinedTensionFitnesses()
        {
            fitnesses = new List<IFitness>();
        }

        public CombinedTensionFitnesses(List<IFitness> fitnesses)
        {
            this.fitnesses = fitnesses;
        }

        public void AddFitness(IFitness fitness)
        {
            fitnesses.Add(fitness);
        }

        public double evaluate(IPhenotype pheno)
        {
            double val = 0;
            foreach(IFitness f in fitnesses)
            {
                val += f.evaluate(pheno);
            }
            val = (val / fitnesses.Count);
            return val;
        }

        public string getFitnessNames()
        {
            string value = "";

            foreach(IFitness f in fitnesses)
            {
                string[] fitnessStruct = f.GetType().ToString().Split('.');
                string fitnessName = fitnessStruct[fitnessStruct.Length - 1];

                value += fitnessName + "_";
            }
            return value;
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
