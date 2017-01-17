using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual;
using ProjectMaze.Visual.Spawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Game
{
    public class AIPlayer : Player
    {

        private List<int> objectivePath;

        private Phenotype visual_map;
        private Tile start;
        private Tile objective;
        private List<SpawnPoint> objectives;

        public AIPlayer(int pos_x, int pos_y, Phenotype visual_map) 
            : base(pos_x, pos_y)
        {
            this.visual_map = visual_map;
            objectivePath = new List<int>();
            objectives = new List<SpawnPoint>();
            //populateObjective();
        }

        public AIPlayer(float pos_x, float pos_y, Phenotype visual_map)
            : base(pos_x, pos_y)
        {
            this.visual_map = visual_map;
            objectivePath = new List<int>();
            objectives = new List<SpawnPoint>();
            //populateObjective();
        }

        public void populateObjective()
        {
            foreach (SpawnPoint sp in visual_map.getMap().getSpawnPoints())
            {
                if (sp.getType() == 0)
                {
                    objectives.Add(sp);
                }
            }

            calculatePath();

        }

        public List<int> calculatePath()
        {
            if (objectives.Count > 0)
            {
                this.start = visual_map.getMap().getTileByID(this.getCurrentTile());
                this.objective = objectives[0].getTile();

                objectivePath = AStar.normalAStar(visual_map, objective, start);
            }
            else
            {
                objectivePath = new List<int>(); // This node has been visited
            }
            return objectivePath;
        }

        public List<int> getLastPath()
        {
            if (objectivePath == null)
            {
                objectivePath = new List<int>();
                return objectivePath;
            }
            else
            {
                return objectivePath;
            }
        }

        public void move()
        {
            if (objectivePath != null && objectivePath.Count > 0)
            {
                // Move the player object through the map.
                if (this.getCurrentTile() == objectivePath[0])
                {
                    // Remove it from the path
                    objectivePath.RemoveAt(0);
                }

                if (objectivePath.Count > 0)
                {
                    //float[] coords = visual_map.getMap().getTileByID(objectivePath[0]).getCoords();
                    //float tX = coords[2] - (coords[2] - coords[0]);
                    //float tY = coords[3] - (coords[3] - coords[1]);

                    //if ( Util.GeneralFunctions.calculateDistance((int)this.getX(), (int)this.getY(), (int)tX, (int)tY) > 1.0)
                    //{
                    //    double[] pos = Util.GeneralFunctions.vectorNormalization((int)this.getX(), (int)this.getY(), (int)tX, (int)tY);

                    //    int new_x = Convert.ToInt32(this.getX() + (pos[0] * Player.MOVE_MOD));
                    //    int new_y = Convert.ToInt32(this.getY() + (pos[1] * Player.MOVE_MOD));

                    //    this.Update(new_x, new_y);
                    //}
                    //else
                    //{
                    //    Tile new_tile = visual_map.getMap().getTileByID(objectivePath[0]);
                    //    this.updateTile(new_tile.getID());
                    //    this.updateRoom(new_tile.getRoom().getID());
                    //}

                    //// Move towards the next point
                    if (getCurrentTile() < objectivePath[0])
                    {
                        // Go Down or Right
                        if ((objectivePath[0] - 1) == getCurrentTile())
                        {
                            // Go Down
                            this.Update(this.getX(), this.moveDown());
                        }
                        else
                        {
                            // Go Right
                            this.Update(this.moveRight(), this.getY());
                        }
                    }
                    else
                    {
                        // Go Up or Left
                        if (getCurrentTile() - 1 == objectivePath[0])
                        {
                            // Go Up
                            this.Update(this.getX(), this.moveUP());
                        }
                        else
                        {
                            // Go Left
                            this.Update(this.moveLeft(), this.getY());
                            
                        }
                    }
                }
                else
                {
                    objectives.RemoveAt(0);
                    // We have reached the objective
                    if (objectives.Count > 0)
                    {
                        // Calculate new Path
                        calculatePath();
                    } 
                }
            }
        }

        public int getObjectiveNum()
        {
            return objectives.Count;
        }
    }
}
