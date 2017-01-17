using ProjectMaze.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class NeighbourDictionary
    {
        Dictionary<int, float[]> neighbourDictionary; // Room ID - Door Tile
        List<int> neighbours;
        
        public NeighbourDictionary(int thisRoom, List<Door> doors)
        {
            neighbourDictionary = new Dictionary<int, float[]>();
            neighbours = new List<int>();
            foreach (Door d in doors)
            {
                if (d.getConnectingTiles()[0].getRoom().getID() == thisRoom)
                {
                    int neighbour = d.getConnectingTiles()[1].getRoom().getID();
                    neighbours.Add(neighbour);
                    neighbourDictionary.Add(neighbour, createPoint(d.getConnectingTiles()[1]));
                }
                else if (d.getConnectingTiles()[1].getRoom().getID() == thisRoom)
                {
                    int neighbour = d.getConnectingTiles()[0].getRoom().getID();
                    neighbours.Add(neighbour);
                    neighbourDictionary.Add(neighbour, createPoint(d.getConnectingTiles()[0]));
                }
            }
        }

        public float[] createPoint(Tile t)
        {
            float[] center_point = new float[2];
            center_point[0] = (t.getCoords()[2] - (t.getCoords()[2] - t.getCoords()[0])); // the X Axis
            center_point[1] = (t.getCoords()[3] - (t.getCoords()[3] - t.getCoords()[1])); // the Y Axis
            return center_point;
        }

        public List<int> getNeighbours()
        {
            return neighbours;
        }

        public float[] getNeighbourPoint(int neighbourID)
        {
            return neighbourDictionary[neighbourID];
        }

        public List<float[]> getPoints()
        {
            List<float[]> points = new List<float[]>();
            foreach (int i in neighbours)
            {
                points.Add(neighbourDictionary[i]);
            }
            return points;
        }
    }
}
