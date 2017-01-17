using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.Visual;
using ProjectMaze.Visualization;
using ProjectMaze.GeneticAlgorithm;
using System.Windows.Forms;
using ProjectMaze.GeneticInterfaces;
using QuickGraph.Concepts;
using ProjectMaze.LogSystem;
using System.Diagnostics;

using ProjectMaze.PureData;
using System.IO;
using ProjectMaze.Audio;
using ProjectMaze.Visual.Spawn;
using ProjectMaze.Game;
using ProjectMaze.Util;
using ProjectMaze.TensionMapGeneration;
using ProjectMaze.TensionMapGeneration.TensionMapFitnesses;

namespace ProjectMaze
{
    class Program
    {
        public static MapGenetic gen;
        public static MapVisualizer visualizer;
        public static TensionMapGenetic tensionGen;
        
        // Experimental Run Code Variable
        public static bool LOG_IT = false;
        public static int pop_num = 100;
        public static int gen_num = 100;
        public static int num_runs = 20;

        public static int tensionMap_popNum = 100;
        public static int tensionMap_genNum = 100;


        public static int WINDOW_HEIGHT = 700; // 700; 1000; 1800
        public static int WINDOW_WIDTH = 700;  // 700; 1000; 1000

        public static double[] tension_map = Tension_Maps.inverse_U_Wave;

        [STAThread]
        public static void Main(string[] args)
        {
            //Map map = Map.mapFactoryStatic(700, 700);

            Console.WriteLine("Welcome to Maze Generator 9000 ");

            if (LOG_IT)
            {
                // Log Check
                Console.WriteLine(FolderManagement.mainLogFolder);
                FolderManagement.mainLogFolderCheck();

                //TensionMapExperiment exp = new TensionMapExperiment(4, 100, 100);
                //exp.runExp();

                //for (int runs = 0; runs < 8; runs++)
                //{
                //    TensionPickRandomizer fitRandomizer = new TensionPickRandomizer();
                //    List<IFitness> tensionMapFitnesses = fitRandomizer.chooseRandomFitness();

                //foreach (IFitness f in tensionMapFitnesses)
                //{
                //    string[] fitnessStruct = f.GetType().ToString().Split('.');
                //    string fitnessName = fitnessStruct[fitnessStruct.Length - 1];

                //    Console.WriteLine("Starting Fitness " + fitnessName);

                //    if (fitnessName == "CombinedTensionFitnesses")
                //    {
                //        CombinedTensionFitnesses aux = (CombinedTensionFitnesses)f;
                //        fitnessName += "_" + aux.getFitnessNames();
                //    }
                //    else if (fitnessName == "MultiplyTensionFitnesses")
                //    {
                //        MultiplyTensionFitnesses aux = (MultiplyTensionFitnesses)f;
                //        fitnessName += "_" + aux.getFitnessNames();
                //    }

                        IFitness f = new DenovementTensionFitness();
                        string fitnessName = "DenouementTensionFitness_CIGEXP";
                        FolderManagement folderManager = new FolderManagement("Experiment_" + fitnessName + " " + Stopwatch.GetTimestamp());
                        
                        // Run Cycle goes here
                        for (int i = 0; i < num_runs; i++)
                        {
                            folderManager.setNewRunFolder("Run " + i);

                            // TEMP CODE FOR TENSION MAP GENERATION
                            List<IMutation> tensionMutations = new List<IMutation>();
                            tensionMutations.Add(new TensionMutation(0.2));
                            tensionGen = new TensionMapGenetic(tensionMap_popNum, 0.8, new RouletteSelection(5), new OnePointCrossover(0.7), 8, f, tensionMutations, folderManager);


                            // Run Tension Map Generation
                            tensionGen.run(tensionMap_genNum);

                            tension_map = ((TensionMapPhenotype)tensionGen.getBestIndividual()).getTensionMap(); // Give the generated tension map to the level generator.

                            // Kickstart method - Create a new Run!
                            if (i == 0)
                            {
                                // Static Individual - With New Room Mutation
                                List<IMutation> mutations = new List<IMutation>();
                                mutations.Add(new AddRoomMutation(0.9));
                                mutations.Add(new SimpleMutation(0.8));
                                mutations.Add(new DoorMutation(0.9));
                                mutations.Add(new ItemMoveSpawnMutation(0.6));
                                mutations.Add(new MoveMonsterMutation(0.7));
                                mutations.Add(new MoveLightMutation(0.7));
                                //mutations.Add(new MoveSoundFXMutation(0.7));
                                mutations.Add(new PlaceMonsterMutation(0.5));
                                mutations.Add(new PlaceLightMutation(0.5));
                                //mutations.Add(new PlaceSoundFXMutation(0.5));

                                gen = new MapGenetic(pop_num, 1.0, WINDOW_HEIGHT, WINDOW_WIDTH, new RouletteSelection(3), new SimpleFitness(), mutations, true, folderManager);

                                Application.EnableVisualStyles();

                                // Show Phenotype
                                visualizer = new MapVisualizer((Phenotype)gen.getBestIndividual(), WINDOW_WIDTH, WINDOW_HEIGHT);

                                visualizer.Show();

                                //Application.Run(visualizer);
                            }
                            else
                            {
                                // Static Individual - With New Room Mutation
                                List<IMutation> mutations = new List<IMutation>();
                                mutations.Add(new AddRoomMutation(0.9));
                                mutations.Add(new SimpleMutation(0.8));
                                mutations.Add(new DoorMutation(0.9));
                                mutations.Add(new ItemMoveSpawnMutation(0.6));
                                mutations.Add(new MoveMonsterMutation(0.7));
                                mutations.Add(new MoveLightMutation(0.7));
                                //mutations.Add(new MoveSoundFXMutation(0.7));
                                mutations.Add(new PlaceMonsterMutation(0.5));
                                mutations.Add(new PlaceLightMutation(0.5));
                                //mutations.Add(new PlaceSoundFXMutation(0.5));

                                gen = new MapGenetic(pop_num, 1.0, WINDOW_HEIGHT, WINDOW_WIDTH, new RouletteSelection(3), new SimpleFitness(), mutations, true, folderManager);
                            }

                            // Genetic Cycle goes here!
                            gen.run(gen_num);
                        }
                        visualizer.Close();
                    //}
                //}
            }
            // NON BATCH EXPERIMENT MODE!
            else
            {
                //Tension_Maps.translateMap(4, Tension_Maps.U_Shape);

                // Static Individual - With New Room Mutation
                List<IMutation> mutations = new List<IMutation>();
                mutations.Add(new AddRoomMutation(0.9));
                mutations.Add(new SimpleMutation(0.8));
                mutations.Add(new DoorMutation(0.9));
                mutations.Add(new ItemMoveSpawnMutation(0.6));
                mutations.Add(new MoveMonsterMutation(0.7));
                mutations.Add(new MoveLightMutation(0.7));
                //mutations.Add(new MoveSoundFXMutation(0.7));
                //mutations.Add(new MovePuzzleMutation(0.7)); -- Remove For Now (No 3D Implementation yet...)
                mutations.Add(new PlaceMonsterMutation(0.5));
                mutations.Add(new PlaceLightMutation(0.5));
                //mutations.Add(new PlaceSoundFXMutation(0.5));
                //mutations.Add(new PlacePuzzleMutation(0.5)); -- Remove For Now (No 3D Implementation yet...)

                gen = new MapGenetic(pop_num, 1.0, WINDOW_HEIGHT, WINDOW_WIDTH, new RouletteSelection(3), new SimpleFitness(), mutations, true);

                Application.EnableVisualStyles();

                // Show Phenotype
                visualizer = new MapVisualizer((Phenotype)gen.getBestIndividual(), WINDOW_WIDTH, WINDOW_HEIGHT);

                Application.Run(visualizer);

            }

            //neighbourWalk();
        }

        public static void neighbourWalk()
        {

            // Neighbour debugging function - seems to be working...
            Map map = Map.mapFactory(700, 700);

            Tile current_tile = map.getTiles()[0];
            Console.WriteLine("To Escape Press Use -1");
            int selection = 0;
            while (selection >= 0)
            {
                Console.WriteLine("You Are Currently on Tile " + current_tile.getID());
                Tile[] neighbours = map.obtainNeighbours(current_tile);
                Console.Write("You can move ");
                if (neighbours[0] != null)
                {
                    Console.Write("UP(0) ");
                }
                if (neighbours[1] != null)
                {
                    Console.Write("DOWN(1) ");
                }
                if (neighbours[2] != null)
                {
                    Console.Write("LEFT(2) ");
                }
                if (neighbours[3] != null)
                {
                    Console.Write("RIGHT(3) ");
                }

                selection = Convert.ToInt32(Console.ReadLine());
                if (selection == -1)
                {
                    selection = -1;
                }
                else if (selection > neighbours.Length - 1)
                {
                    Console.WriteLine("That is not a valid move!");
                }
                else if (neighbours[selection] == null)
                {
                    Console.WriteLine("You can't move there...");
                }
                else
                {
                    current_tile = neighbours[selection];
                }
            }
        }

        public static void mutateIt()
        {
            Console.WriteLine("Mutating!");
            // Run One Generation
            //gen.run();
            //Run 25 Generations
            gen.run(100);

            Phenotype phen = (Phenotype) gen.getBestIndividual();
            Console.WriteLine("The Fittest Individual has a value of " + phen.getFitness());
            Console.WriteLine("Multi Path Fitness = " + phen.getMultiPathFitness() + " ; Monster Fitness = " + phen.getMonsterFitness());

            foreach (MyTuple mt in phen.getAnxietyMap())
            {
                Console.Write("Room " + mt.getID() + " -- " + mt.getValue() + " ");
            }

            visualizer.updateMap((Phenotype)gen.getBestIndividual());
        }

        public static void loadPhenotypeToVisualizer(Phenotype pheno, string filename)
        {
            visualizer.loadMapAndPicture(pheno, filename);
        }

        public static List<List<IEdge>> getPathFinding()
        {
            return ((Phenotype)gen.getBestIndividual()).getPaths();
        }

        public static List<int> getSpawnPath(SpawnPoint sp)
        {
            Phenotype phen = (Phenotype)gen.getBestIndividual();
            Tile start = phen.getMap().getRoomByID(0).getTiles()[0];
            return AStar.normalAStar(phen, sp.getTile(), start);
        }

        public static void playSomething(Phenotype pheno)
        {
            //Console.WriteLine("I Am Here");
            //PlayerThread.playSomething();
            Phenotype phen = (Phenotype)gen.getBestIndividual();
            Console.WriteLine("The Fittest Individual has a value of " + phen.getFitness());
            Console.WriteLine("Multi Path Fitness = " + phen.getMultiPathFitness() + " ; Monster Fitness = " + phen.getMonsterFitness());


            Console.WriteLine("Approximation Values");
            List<MyTuple> target_values = Tension_Maps.translateMap(pheno.getAnxietyMap().Count, tension_map);

            for (int i = 0; i < target_values.Count; i++)
            {
                double value = 2 - Math.Abs((target_values[i].getValue() - pheno.getAnxietyMap()[i].getValue()));
                Console.WriteLine("Difference Value -- Room " + pheno.getAnxietyMap()[i].getID() + " = " + value + " ; Target Value = " + target_values[i].getValue() + " ; Real Value = " + pheno.getAnxietyMap()[i].getValue());
            }


        }

        public static void resetGenetic(Phenotype phen)
        {
            // Static Individual - With New Room Mutation
            List<IMutation> mutations = new List<IMutation>();
            mutations.Add(new AddRoomMutation(0.70));
            mutations.Add(new SimpleMutation(0.95));
            mutations.Add(new DoorMutation(0.90));
            mutations.Add(new ItemMoveSpawnMutation(0.75));
            mutations.Add(new MoveMonsterMutation(0.6));
            mutations.Add(new PlaceMonsterMutation(0.5));

            gen = new MapGenetic(pop_num, 1.0, phen.getMap().getHeightWidth()[0], phen.getMap().getHeightWidth()[1], new RouletteSelection(3), new SimpleFitness(), mutations, phen);

            visualizer.Hide();
            visualizer = new MapVisualizer((Phenotype)gen.getBestIndividual(), WINDOW_WIDTH, WINDOW_HEIGHT);

            visualizer.Show();
        }

    }
}
