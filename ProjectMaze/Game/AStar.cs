using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticAlgorithm.GraphRepresentation;
using ProjectMaze.Visual;
using QuickGraph.Concepts;
using QuickGraph.Representations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Game
{
    public class AStar
    {

        private static int G_Value = 10;

        public static List<int> normalAStar(Phenotype phen, Tile goal, Tile start)
        {
            Dictionary<int, int> HValues = new Dictionary<int, int>();

            int gX = goal.getID() % Map.WIDTH_TILE_SIZE;
            int gy = (int) Math.Floor(goal.getID() / (double)Map.WIDTH_TILE_SIZE);

            // Build H Values
            foreach (Tile t in phen.getMap().getTiles())
            {
                if (t.getID() != goal.getID())
                {
                    int x = t.getID() % Map.WIDTH_TILE_SIZE;
                    int y = (int)Math.Floor(t.getID() / (double)Map.WIDTH_TILE_SIZE);

                    int H_value = Math.Abs(gX - x) + Math.Abs(gy - y);

                    HValues.Add(t.getID(), H_value);
                }
                else
                {
                    HValues.Add(t.getID(), 0);
                }
            }

            AdjGraph graph = phen.getGraph();
            
            Dictionary<int,int> pathList = new Dictionary<int,int>();
            List<int> closedList = new List<int>();
            List<int> openList = new List<int>();

            //foreach(Tile t in phen.getMap().obtainAvailableNeighbours(start))
            //{
            //    openList.Add(graph.getVertex(t.getID()));
            //}
            //closedList.Add(graph.getVertex(start.getID()));


            // Start the Process -- Add the start Node to the list
            openList.Add(start.getID());
            pathList.Add(start.getID(), start.getID());

            // Looping now begins!
            while (openList.Count > 0)
            {
                
                // Order List by shortest F Value = G + H
                orderList(HValues, openList);

                // Expand the lowest F value node
                int expander = openList[0];

                foreach(IEdge e in graph.OutEdges(phen.getGraph().getVertex(expander).getIVertex()))
                {
                    if(!closedList.Contains(graph.getEdge(e).getVertexTarget().getID()) && !openList.Contains(graph.getEdge(e).getVertexTarget().getID()))
                    {
                        // ADD to the open list and add the path.
                        openList.Add(graph.getEdge(e).getVertexTarget().getID());
                        pathList.Add(graph.getEdge(e).getVertexTarget().getID(), expander);
                    }
                }

                // Remove from open list. Add neighboring nodes to list. Process restarts.
                openList.RemoveAt(0);
                closedList.Insert(0, expander);

                // See if goal has been reached!
                if (closedList[0] == goal.getID())
                {
                    // Found Goal -- Backtrack and break 
                    List<int> pathFound = new List<int>();
                    int checker = closedList[0];
                    pathFound.Insert(0, checker);

                    while (checker != start.getID())
                    {
                        Console.WriteLine(pathList[checker] + " & " + checker);
                        pathFound.Insert(0, pathList[checker]);
                        checker = pathList[checker];
                    }
                    return pathFound;
                }
                
            }

            return new List<int>();
        }


        private static void orderList(Dictionary<int, int> H_Value, List<int> openList)
        {
            openList.Sort((a, b) => calcF(H_Value[a]).CompareTo(calcF(H_Value[b])));
        }

        private static int calcF(int H_Value)
        {
            return G_Value + H_Value;
        }
    }
}
