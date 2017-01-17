using ProjectMaze.LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface IGeneticAlgorithm
    {
        void run();
        void run(int num_gen);
        List<IPhenotype> getBestIndividuals(int num);
        IPhenotype getBestIndividual();
        IPhenotype getIndividual(int index);
        double getAvgFitness();
        double getBestFitness();
        double getWorseFitness();
        IFitness getFitnesFunction();
        void createLogger(FolderManagement folderManager);
        
    }
}
