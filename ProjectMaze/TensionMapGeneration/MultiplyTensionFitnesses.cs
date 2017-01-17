using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration
{
    public class MultiplyTensionFitnesses : IFitness
    {

        private List<IFitness> fitnesses;
        private ILogger logger;

        public MultiplyTensionFitnesses()
        {
            fitnesses = new List<IFitness>();
        }

        public MultiplyTensionFitnesses(List<IFitness> fitnesses)
        {
            this.fitnesses = fitnesses;
        }

        public void AddFitness(IFitness fitness)
        {
            fitnesses.Add(fitness);
        }

        public double evaluate(IPhenotype pheno)
        {
            double val = 1;
            foreach (IFitness f in fitnesses)
            {
                val = val * f.evaluate(pheno);
            }
            return val;
        }

        public string getFitnessNames()
        {
            string value = "";

            foreach (IFitness f in fitnesses)
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
