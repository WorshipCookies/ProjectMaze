using ProjectMaze.GeneticInterfaces;
using ProjectMaze.TensionMapGeneration;
using ProjectMaze.TensionMapGeneration.TensionMapFitnesses;
using ProjectMaze.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionPickRandomizer
    {

        List<IFitness> tensionMapFitnesses;

        public TensionPickRandomizer()
        {
            tensionMapFitnesses = new List<IFitness>();
            tensionMapFitnesses.Add(new DecreasingTensionFitness());
            tensionMapFitnesses.Add(new DenovementTensionFitness());
            tensionMapFitnesses.Add(new EscalatingTensionFitness());
            tensionMapFitnesses.Add(new RestingPointFitness());
            tensionMapFitnesses.Add(new RiseFallTensionFitness());
            tensionMapFitnesses.Add(new SoapOperaTensionFitness());
            tensionMapFitnesses.Add(new SurprisingMomentFitness());
            tensionMapFitnesses.Add(new UnresolvedTensionFitness());
        }


        public List<IFitness> chooseRandomFitness()
        {
            List<int> fit_index = Enumerable.Range(0, tensionMapFitnesses.Count-1).ToList();

            double randval = MyRandom.getRandom().random().NextDouble();

            List<IFitness> newList = new List<IFitness>();

            // Combination Fitness Function
            if (randval > 0.99)
            {
                // Pick a Fitness Randomly
                int pos1 = MyRandom.getRandom().random().Next(fit_index.Count);
                IFitness fit1 = tensionMapFitnesses[fit_index[pos1]];
                fit_index.RemoveAt(pos1);

                int pos2 = MyRandom.getRandom().random().Next(fit_index.Count);
                IFitness fit2 = tensionMapFitnesses[fit_index[pos2]];
                fit_index.RemoveAt(pos2);

                CombinedTensionFitnesses combo = new CombinedTensionFitnesses();
                combo.AddFitness(fit1);
                combo.AddFitness(fit2);

                newList.Add(combo);

            }

            // Multiplication Fitness Function
            else
            {
                // Pick a Fitness Randomly
                int pos1 = MyRandom.getRandom().random().Next(fit_index.Count);
                IFitness fit1 = tensionMapFitnesses[fit_index[pos1]];
                fit_index.RemoveAt(pos1);

                int pos2 = MyRandom.getRandom().random().Next(fit_index.Count);
                IFitness fit2 = tensionMapFitnesses[fit_index[pos2]];
                fit_index.RemoveAt(pos2);

                MultiplyTensionFitnesses multi = new MultiplyTensionFitnesses();
                multi.AddFitness(fit1);
                multi.AddFitness(fit2);

                newList.Add(multi);
            }
            return newList;
        }


    }
}
