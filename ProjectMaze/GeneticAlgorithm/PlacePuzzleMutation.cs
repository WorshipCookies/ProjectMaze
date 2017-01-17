using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class PlacePuzzleMutation : IMutation
    {
        private double mutationRate;
        private ILogger logger;

        private static string name_ID = "Puzzle Place Spawn Mutation";

        private static int MAX_PUZZLE_PER_ROOM = 1;

        public PlacePuzzleMutation(double mutationRate)
        {
            this.mutationRate = mutationRate;
            this.logger = null;
        }

        public double getMutationRate()
        {
            return mutationRate;
        }

        public void mutate(IGenotype geno)
        {
            // This mutation adds or removes a monster from a room (i.e. Max Number of Monsters per Room = 1).
            // Choose a random room. If a monster is in it remove, if not add!
            try
            {
                Genotype gen = (Genotype)geno;

                // Get All Room Id's currently Available.
                List<int> roomIds = gen.getAllRoomIDs();

                // Pick a room Id at random
                int pickedRoom = roomIds[Util.MyRandom.getRandom().random().Next(roomIds.Count)];

                // If a Monster is already in that room remove it; if not add it!
                List<int> pos_puzzle = isPuzzleInRoom(gen, pickedRoom);

                if (pos_puzzle.Count >= MAX_PUZZLE_PER_ROOM)
                {
                    // If a monster exists remove it!
                    if (MAX_PUZZLE_PER_ROOM > 1)
                    {
                        //Choose randomly
                        gen.getSpawnPoints().RemoveAt(pos_puzzle[Util.MyRandom.getRandom().random().Next(pos_puzzle.Count)]);
                    }
                    else
                    {
                        gen.getSpawnPoints().RemoveAt(pos_puzzle[0]); // Remove the monster!
                    }
                }
                else
                {
                    if (MAX_PUZZLE_PER_ROOM > 1 && pos_puzzle.Count > 0)
                    {
                        // 50-50 chance of adding one more monster or removing it
                        if (Util.MyRandom.getRandom().random().NextDouble() < 0.5)
                        {
                            // Add Monster
                            int[] new_monster = { 5, pickedRoom };
                            gen.getSpawnPoints().Add(new_monster);
                        }
                        else
                        {
                            // Remove Monster by choosing randomly
                            gen.getSpawnPoints().RemoveAt(pos_puzzle[Util.MyRandom.getRandom().random().Next(pos_puzzle.Count)]);
                        }

                    }
                    else
                    {
                        // Can only add monster!
                        int[] new_puzzle = { 5, pickedRoom };
                        gen.getSpawnPoints().Add(new_puzzle);
                    }
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void setLogger(ILogger log)
        {
            logger = log;
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
        }

        public List<int> isPuzzleInRoom(Genotype gen, int roomID)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 5 && gen.getSpawnPoints()[i][1] == roomID)
                {
                    pos.Add(i);
                }
            }
            return pos;
        }
    }
}
