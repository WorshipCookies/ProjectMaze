using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ProjectMaze.Game
{
    public class Enemy
    {
        private float pos_x;
        private float pos_y;

        public static int PIXEL_LENGTH = 15;
        public static int MOVE_MOD = 2;//15;

        private int currentRoom_ID;
        private int currentTile_ID;

        private Brush brush;
        private int enemy_type;
        private int ID;

        public Enemy(int pos_x, int pos_y, int id)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;

            this.enemy_type = 10;
            this.ID = id;
            this.brush = Brushes.ForestGreen;
        }

        public Enemy(float pos_x, float pos_y, int id)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;

            this.enemy_type = 10;
            this.ID = id;
            this.brush = Brushes.ForestGreen;
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

        public int getEnemyType()
        {
            return this.enemy_type;
        }

        public int getID()
        {
            return this.ID;
        }

        public Brush getBrush()
        {
            return brush;
        }
    }
}
