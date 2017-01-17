using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class PuzzleSpawnElement : ISpawnElement
    {
        private int type;
        private double tension_val;

        public PuzzleSpawnElement()
        {
            this.type = 5;
            this.tension_val = 0.5;
        }

        public int getTypeID()
        {
            return type;
        }

        public double tensionValue()
        {
            return tension_val;
        }
    }
}
