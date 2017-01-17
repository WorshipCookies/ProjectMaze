using ProjectMaze.GeneticAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectMaze.Game
{
    public class Player
    {
        private float pos_x;
        private float pos_y;

        public static int PIXEL_LENGTH = 15;
        public static int MOVE_MOD = 2;//15;

        private int currentRoom_ID;
        private int currentTile_ID;

        public Player(int pos_x, int pos_y)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;
        }

        public Player(float pos_x, float pos_y)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;
        }

        public void Update(int pos_x, int pos_y)
        {
            this.pos_x = (float)pos_x;
            this.pos_y = (float)pos_y;
        }

        public void Update(float pos_x, float pos_y)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;
        }

        public void UpdateX(float pos_x)
        {
            this.pos_x = pos_x;
        }

        public void UpdateY(float pos_y)
        {
            this.pos_y = pos_y;
        }

        public float getX()
        {
            return pos_x;
        }

        public float getY()
        {
            return pos_y;
        }

        public float[] getBoundingBox()
        {
            float[] bbox = new float[4];
            bbox[0] = pos_x - PIXEL_LENGTH; // X1 Pos
            bbox[1] = pos_y - PIXEL_LENGTH; // Y1 Pos
            bbox[2] = pos_x + PIXEL_LENGTH; // X2 Pos
            bbox[3] = pos_y + PIXEL_LENGTH; // Y2 Pos
            return bbox;
        }

        public static float[] getNewBoundingBox(float new_pos_x, float new_pos_y)
        {
            float[] bbox = new float[4];
            bbox[0] = new_pos_x - PIXEL_LENGTH; // X1 Pos
            bbox[1] = new_pos_y - PIXEL_LENGTH; // Y1 Pos
            bbox[2] = new_pos_x + PIXEL_LENGTH; // X2 Pos
            bbox[3] = new_pos_y + PIXEL_LENGTH; // Y2 Pos
            return bbox;
        }

        public void updateTile(int TileID)
        {
            this.currentTile_ID = TileID;
        }

        public void updateRoom(int RoomID)
        {
            this.currentRoom_ID = RoomID;
        }

        public int getCurrentRoom()
        {
            return currentRoom_ID;
        }

        public int getCurrentTile()
        {
            return currentTile_ID;
        }

        public float moveUP()
        {
            return pos_y - Player.MOVE_MOD;
        }

        public float moveDown()
        {
            return pos_y + Player.MOVE_MOD;
        }

        public float moveRight()
        {
            return pos_x + Player.MOVE_MOD;
        }

        public float moveLeft()
        {
            return pos_x - Player.MOVE_MOD;
        }
    }
}
