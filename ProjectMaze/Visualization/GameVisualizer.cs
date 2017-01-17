using LibPDBinding;
using ProjectMaze.Audio;
using ProjectMaze.Audio2;
using ProjectMaze.Game;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual;
using ProjectMaze.Visual.Spawn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectMaze.Visualization
{
    public partial class GameVisualizer : Form
    {

        private Player player;
        private Phenotype visual_map;
        private int roomChosen;
        private AudioDirector2 audiodirector;

        public static float volume;

        private List<Rectangle> walls;
        private static int WALL_THICKNESS = 1;

        private bool AI_Simulation;
        private DateTime lastMove;
        private static int TIME_TO_MOVE = 50;

        public GameVisualizer(Phenotype pheno, bool AI_Simulation)
        {
            this.visual_map = pheno;
            this.AI_Simulation = AI_Simulation;

            if (AI_Simulation)
            {
                this.player = initializeAIPlayer();
                this.lastMove = DateTime.Now;
            }
            else
            {
                this.player = initializePlayer(); // for NOW
                this.KeyPreview = true;
                this.KeyDown += new KeyEventHandler(tb_KeyDown);
            }
            
            this.buildDoors();
            
            this.walls = buildWalls();

            /// COMMENTING THE AUDIO 
            //this.audioDirector = new AudioDirector(pheno);
            //audioMix = new AudioMixer();
            //audioMix.addSoundPoint(new SoundPoint(0));
            //audioMix.Play();

            //AudioPlaybackEngine ape = AudioPlaybackEngine.Instance;
            //ape.PlaySound(Directory.GetCurrentDirectory() + "\\soundbank\\abandon.wav");
            audiodirector = new AudioDirector2(visual_map);

            volume = 5f;

            this.roomChosen = 0;
            
            InitializeComponent();
        }

        private void GameVisualizer_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(GameVisualizer_Paint);
        }

        private void GameVisualizer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            // Draw in an Outline Fashion
            foreach (Tile t in visual_map.getMap().getTiles())
            {
                // Prints Room ID on tiles --- Too Remove Numbers comment the next line ONLY!!!
                //e.Graphics.DrawString(Convert.ToString(t.getRoom().getID()) + " " + Convert.ToString(t.getID()), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, t.getCoords()[0], t.getCoords()[1]);
                //e.Graphics.DrawString(Convert.ToString(t.getID()), new Font("Arial", 14, FontStyle.Bold), Brushes.Black, t.getCoords()[0], t.getCoords()[1]);

                // Coordinate system is in (Height, Width)! It might get confusing...
                Tile[] neighbours = visual_map.getMap().obtainNeighbours(t);
                if (neighbours[0] == null || neighbours[0].getRoom() != t.getRoom())
                {
                    // Draw Up
                    float[] coords = t.getCoords();
                    e.Graphics.DrawLine(Pens.Black, coords[0], coords[1], coords[0], coords[3]);
                }
                if (neighbours[1] == null || neighbours[1].getRoom() != t.getRoom())
                {
                    // Draw Down
                    float[] coords = t.getCoords();
                    e.Graphics.DrawLine(Pens.Black, coords[2], coords[1], coords[2], coords[3]);
                }
                if (neighbours[2] == null || neighbours[2].getRoom() != t.getRoom())
                {
                    // Draw Left
                    float[] coords = t.getCoords();
                    e.Graphics.DrawLine(Pens.Black, coords[0], coords[1], coords[2], coords[1]);
                }
                if (neighbours[3] == null || neighbours[3].getRoom() != t.getRoom())
                {
                    // Draw Right
                    float[] coords = t.getCoords();
                    e.Graphics.DrawLine(Pens.Black, coords[0], coords[3], coords[2], coords[3]);
                }
            }

            // Draw Color Coded Fashion
            //foreach (Tile t in visual_map.getMap().getTiles())
            //{
            //    float[] coords = t.getCoords();
            //    e.Graphics.DrawRectangle(Pens.Black, coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
            //}

            // Draw the Doors now! - NOTE THIS MIGHT NEED DEBUGGING... This Code is not ideal!!
            foreach (Door d in visual_map.getMap().getDoors())
            {
                Tile[] tiles = visual_map.getMap().obtainNeighbours(d.getConnectingTiles()[0]);
                if (d.getConnectingTiles()[1] == null)
                {
                    Pen p = new Pen(Color.Red, 10);

                    if (tiles[0] == null)
                    {
                        // Draw Up
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[1], coords[0], coords[3]);
                    }
                    else if (tiles[1] == null)
                    {
                        // Draw Down
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[2], coords[1], coords[2], coords[3]);
                    }
                    else if (tiles[2] == null)
                    {
                        // Draw Left
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[1], coords[2], coords[1]);
                    }
                    else if (tiles[3] == null)
                    {
                        // Draw Right
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[3], coords[2], coords[3]);
                    }
                }
                else
                {
                    Pen p = new Pen(Color.Red, 10);
                    if (tiles[0] != null && tiles[0].getID() == d.getConnectingTiles()[1].getID())
                    {
                        // Draw Up
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[1], coords[0], coords[3]);
                    }
                    if (tiles[1] != null && tiles[1].getID() == d.getConnectingTiles()[1].getID())
                    {
                        // Draw Down
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[2], coords[1], coords[2], coords[3]);
                    }
                    if (tiles[2] != null && tiles[2].getID() == d.getConnectingTiles()[1].getID())
                    {
                        // Draw Left
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[1], coords[2], coords[1]);
                    }
                    if (tiles[3] != null && tiles[3].getID() == d.getConnectingTiles()[1].getID())
                    {
                        // Draw Right
                        float[] coords = d.getConnectingTiles()[0].getCoords();
                        e.Graphics.DrawLine(p, coords[0], coords[3], coords[2], coords[3]);
                    }
                }
            }

            // Print Tiles
            //foreach (Tile t in visual_map.getMap().getTiles())
            //{
            //    float[] coords = t.getCoords();
            //    e.Graphics.DrawRectangle(Pens.Orange, coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
            //}

            float[] item_coords = visual_map.getMainItem().getTile().getCoords();
            e.Graphics.FillRectangle(Brushes.Blue, item_coords[0] + 15, item_coords[1] + 15, (item_coords[2] - (item_coords[0] + 15)) - 15, (item_coords[3] - (item_coords[1] + 15)) - 15);

            // Draw Enemies and Items here!
            foreach (SpawnPoint sp in visual_map.getMap().getSpawnPoints())
            {
                //int tiles_count = visual_map.getMap().getRoomByID(sp.getRoom()).getTiles().Count;
                //Tile t = visual_map.getMap().getRoomByID(sp.getRoom()).getTiles()[(int)tiles_count/2];

                // Draw on tile location.
                if (sp.getType() == 1) // ITEM!
                {
                    // Check to see if an item exists in this room
                    if (visual_map.getMap().itemExists(sp.getRoom()))
                    {
                        float[] coords = sp.getTile().getCoords();//t.getCoords();
                        Point[] triangle = { new Point((int)(((coords[0] + coords[2]) / 2) + 10) + 10, (int)coords[1] + 10 + 20), new Point((int)coords[0] + 10 + 20, (int)coords[3] - 10 + 20), new Point((int)coords[2] - 10 + 20, (int)coords[3] - 10 + 20) };

                        e.Graphics.FillPolygon(Brushes.ForestGreen, triangle);
                    }
                    else
                    {
                        float[] coords = sp.getTile().getCoords();//t.getCoords();
                        Point[] triangle = { new Point((int)((coords[0] + coords[2]) / 2), (int)coords[1] + 10), new Point((int)coords[0] + 10, (int)coords[3] - 10), new Point((int)coords[2] - 10, (int)coords[3] - 10) };

                        e.Graphics.FillPolygon(Brushes.ForestGreen, triangle);

                    }
                }

            }

            if (AI_Simulation)
            {
                foreach (int tile in ((AIPlayer)player).getLastPath())
                {
                    Tile redge = visual_map.getMap().getTileByID(tile);

                    float[] coords = redge.getCoords();
                    e.Graphics.FillRectangle(Brushes.Violet, coords[0] + 15, coords[1] + 15, (coords[2] - coords[0]) - 15, (coords[3] - coords[1]) - 15);

                }
            }
            


            // Draw Player!
            float[] player_coords = player.getBoundingBox();
            e.Graphics.FillRectangle(Brushes.Black, player_coords[0], player_coords[1], (player_coords[2] - player_coords[0]), (player_coords[3] - player_coords[1]));


            // Print Collision Walls
            foreach (Rectangle r in walls)
            {
                e.Graphics.FillRectangle(Brushes.Blue, r);
                float[] bbox = player.getBoundingBox();
                Rectangle player_rect = new Rectangle((int)bbox[0], (int)bbox[1], (int)(bbox[2] - bbox[0]), (int)(bbox[3] - bbox[1]));
                if (r.IntersectsWith(player_rect))
                {
                    Rectangle intersection = Rectangle.Intersect(r, player_rect);
                    e.Graphics.FillRectangle(Brushes.Green, intersection);
                }
            }

            // Paint Room Chosen -- Sound testing solution
            //foreach (Tile t in visual_map.getMap().getRoomByID(roomChosen).getTiles())
            //{
            //    float[] coords = t.getCoords();
            //    e.Graphics.FillRectangle(Brushes.Blue, coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            audiodirector.Update(player);

            if (AI_Simulation)
            {
                TimeSpan timeDiff = DateTime.Now - lastMove;
                if (timeDiff.TotalMilliseconds > TIME_TO_MOVE)
                {
                    updateAIPlayer();
                    lastMove = DateTime.Now; // update last move
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            audiodirector.printSoundsPlaying();
        }

        public void tb_KeyDown(object sender, KeyEventArgs e)
        {

            // In case of keydown updates the player
            float new_x = player.getX();
            float new_y = player.getY();
            if (e.KeyCode == Keys.W)
            {
                new_y = player.moveUP();
            }
            if (e.KeyCode == Keys.S)
            {
                new_y = player.moveDown();
            }
            if (e.KeyCode == Keys.D)
            {
                new_x = player.moveRight();
            }
            if (e.KeyCode == Keys.A)
            {
                new_x = player.moveLeft();
            }

            if (!doesCollide(new_x, new_y))
            {
                player.Update(new_x, new_y);
                Tile new_tile = changeTile();

                if (new_tile != null)
                {
                    player.updateTile(new_tile.getID());
                    player.updateRoom(new_tile.getRoom().getID());

                    Console.WriteLine("Tile Has Been Updated. Current ID == " + player.getCurrentTile());
                    Console.WriteLine("Current Room is == " + player.getCurrentRoom());
                    audiodirector.printIntensityOfRoom(player.getCurrentRoom());

                }
            }
            this.Refresh();
        }

        public void updateAIPlayer()
        {
            AIPlayer ai_player = (AIPlayer)this.player;

            if (ai_player.getObjectiveNum() > 0)
            {
                // Move Player!
                ai_player.move();
            }
            else
            {
                Console.WriteLine("Game has finished!");
            }

            // In case of keydown updates the player
            float new_x = player.getX();
            float new_y = player.getY();

            if (!doesCollide(new_x, new_y))
            {
                player.Update(new_x, new_y);
                Tile new_tile = changeTileAI();//changeTile();

                if (new_tile != null)
                {
                    player.updateTile(new_tile.getID());
                    player.updateRoom(new_tile.getRoom().getID());

                    Console.WriteLine("Tile Has Been Updated. Current ID == " + player.getCurrentTile());
                    Console.WriteLine("Current Room is == " + player.getCurrentRoom());

                }
            }
            this.Refresh();

        }

        // Collision Detection Function!
        public bool doesCollide(float new_x, float new_y)
        {
            float[] bbox = Player.getNewBoundingBox(new_x, new_y);
            Tile player_tile = visual_map.getMap().getTileByID(player.getCurrentTile());

            // Check Full Map Collision
            Tile firstTile = visual_map.getMap().getTileByID(0); // First Tile
            Tile lastTile = visual_map.getMap().getTileByID(visual_map.getMap().getTiles().Count-1);
            if (bbox[0] <= firstTile.getCoords()[0] || bbox[1] <= firstTile.getCoords()[1] || bbox[2] >= lastTile.getCoords()[2] || bbox[3] >= lastTile.getCoords()[3])
            {
                return true;
            }

            Rectangle player_rect = new Rectangle((int)bbox[0], (int)bbox[1], (int)(bbox[2]-bbox[0]), (int)(bbox[3] - bbox[1]));
            foreach (Rectangle r in walls)
            {
                if (r.IntersectsWith(player_rect))
                {
                    Rectangle intersection = Rectangle.Intersect(r, player_rect);
                    float x1 = intersection.X;
                    float y1 = intersection.Y;

                    if (x1 > player.getBoundingBox()[0])
                    {
                        float newX = x1 - Player.PIXEL_LENGTH;
                        player.UpdateX(newX);
                        
                    }
                    else if (x1 < player.getBoundingBox()[0])
                    {
                        float newX = (x1 + intersection.Width) + Player.PIXEL_LENGTH;
                        player.UpdateX(newX);
                    }

                    if (y1 < player.getBoundingBox()[1])
                    {
                        float newY = (y1 + intersection.Height) + Player.PIXEL_LENGTH;
                        player.UpdateY(newY);
                    }
                    else if (y1 > player.getBoundingBox()[1])
                    {
                        float newY = y1 - Player.PIXEL_LENGTH;
                        player.UpdateY(newY);
                    }


                    Console.WriteLine("Hit a Wall!!");
                    return true;
                }
            }

            return false;
        }

        // Tile Change Function!
        public Tile changeTile()
        {
            //float[] bbox = Player.getNewBoundingBox(new_x, new_y);
            Tile player_tile = visual_map.getMap().getTileByID(player.getCurrentTile());
            Tile[] neighbours = visual_map.getMap().obtainNeighbours(player_tile);

            float[] playerTileCoords = player_tile.getCoords();
            
            // Check Up
            if (player.getY() < playerTileCoords[1])
            {
                return neighbours[2];
            }
            
            // Check Down
            if (player.getY() > playerTileCoords[3])
            {
                return neighbours[3];
            }

            // Check Left
            if (player.getX() < playerTileCoords[0])
            {
                return neighbours[0];
            }

            // Check Right
            if (player.getX() > playerTileCoords[2])
            {
                return neighbours[1];
            }

            return null;
        }

        public Tile changeTileAI()
        {
            Tile player_tile = visual_map.getMap().getTileByID(player.getCurrentTile());
            Tile[] neighbours = visual_map.getMap().obtainNeighbours(player_tile);

            float[] playerTileCoords = player_tile.getCoords();
            float[] pboundingbox = player.getBoundingBox();

            foreach (Tile t in neighbours)
            {
                if (t != null)
                {
                    float[] coords = t.getCoords();
                    if (pboundingbox[0] > coords[0] && pboundingbox[1] > coords[1] && pboundingbox[2] < coords[2] && pboundingbox[3] < coords[3])
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        // Player Initialization Function!
        public Player initializePlayer()
        {
            Tile startTile = visual_map.getMap().getRoomByID(0).getTiles()[0];

            float[] playerCoords = new float[2];
            playerCoords[0] = startTile.getCoords()[0] + Player.PIXEL_LENGTH*2;
            playerCoords[1] = startTile.getCoords()[1] + Player.PIXEL_LENGTH*2;

            Player player = new Player(playerCoords[0], playerCoords[1]);
            player.updateTile(startTile.getID());
            player.updateRoom(startTile.getRoom().getID());

            return player;
        }

        public Player initializeAIPlayer()
        {
            Tile startTile = visual_map.getMap().getRoomByID(0).getTiles()[0];

            float[] playerCoords = new float[2];
            playerCoords[0] = startTile.getCoords()[0] + Player.PIXEL_LENGTH * 2;
            playerCoords[1] = startTile.getCoords()[1] + Player.PIXEL_LENGTH * 2;

            Player player = new AIPlayer(playerCoords[0], playerCoords[1], visual_map);
            player.updateTile(startTile.getID());
            player.updateRoom(startTile.getRoom().getID());

            ((AIPlayer)player).populateObjective(); // Populate the objective after the tile has been set.

            return player;
        }

        // Will create the walls between the tiles of different rooms for collision detection
        public List<Rectangle> buildWalls()
        {
            List<Rectangle> walls = new List<Rectangle>();

            foreach (Tile t in visual_map.getMap().getTiles())
            {
                // Coordinate system is in (Height, Width)! It might get confusing...
                Tile[] neighbours = visual_map.getMap().obtainNeighbours(t);
                if (neighbours[0] != null && neighbours[0].getRoom() != t.getRoom())
                {
                    if (!(t.getDoor() != null && t.getDoor().areConnected(t, neighbours[0])))
                    {
                        // Wall Up
                        float[] coords = t.getCoords();
                        walls.Add(new Rectangle((int)coords[0], (int)coords[1], WALL_THICKNESS, (int)(coords[2] - coords[0])));
                    }
                }
                if (neighbours[1] != null && neighbours[1].getRoom() != t.getRoom())
                {
                    if (!(t.getDoor() != null && t.getDoor().areConnected(t, neighbours[1])))
                    {
                        // Wall Down
                        float[] coords = t.getCoords();
                        walls.Add(new Rectangle((int)coords[2], (int)coords[1], WALL_THICKNESS, (int)(coords[2] - coords[0])));
                    }

                }
                if (neighbours[2] != null && neighbours[2].getRoom() != t.getRoom())
                {
                    if (!(t.getDoor() != null && t.getDoor().areConnected(t, neighbours[2])))
                    {
                        // Wall Left
                        float[] coords = t.getCoords();
                        walls.Add(new Rectangle((int)coords[0], (int)coords[1], (int)(coords[3] - coords[1]), WALL_THICKNESS));
                    }
                }
                if (neighbours[3] != null && neighbours[3].getRoom() != t.getRoom())
                {
                    if (!(t.getDoor() != null && t.getDoor().areConnected(t, neighbours[3])))
                    {
                        // Wall Right
                        float[] coords = t.getCoords();
                        walls.Add(new Rectangle((int)coords[0], (int)coords[3], (int)(coords[3] - coords[1]), WALL_THICKNESS));
                    }
                }
            }

            return walls;
        }

        // Create the doors for the playable map! -- NOT IDEAL SOLUTION MIGHT REMOVE IN FUTURE VERSIONS!!!
        public void buildDoors()
        {
            int id = 0;
            foreach (Door d in visual_map.getMap().getDoors())
            {
                d.setID(id);
                foreach (Tile t in d.getConnectingTiles())
                {
                    t.setDoor(d);
                    t.getRoom().addDoor(d);
                }
                id++;
            }
        }

    }
}
