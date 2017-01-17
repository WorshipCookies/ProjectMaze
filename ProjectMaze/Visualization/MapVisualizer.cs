using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectMaze.Visual;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticAlgorithm.GraphRepresentation;
using QuickGraph.Concepts;
using ProjectMaze.Visual.Spawn;
using ProjectMaze.Util;
using System.Drawing.Drawing2D;

namespace ProjectMaze.Visualization
{
    public partial class MapVisualizer : Form
    {

        //private Map visual_map;
        private Phenotype visual_map;
        private bool is_outline;
        private bool show_graph;
        
        // Adjencency Code
        private bool show_adjencency;
        private string room1;
        private string room2;

        // Pathfinder Code
        private bool pathfinding;
        private int path_num;

        private bool astar;
        private int astar_num;

        public MapVisualizer(Phenotype visual_map, int height, int width)
        {
            
            this.visual_map = visual_map;
            this.is_outline = true;
            this.show_graph = false;

            // Adjacency Code
            this.show_adjencency = false;
            this.room1 = Convert.ToString(0);
            this.room2 = Convert.ToString(0);

            this.pathfinding = false;
            this.path_num = 0;

            this.astar = false;
            this.astar_num = 0;

            InitializeComponent();

            // Dynamic Menu stuff
            this.Size = new Size(width + 200, height + 200);
            this.button1.Location = new Point(width + 10, this.button1.Location.Y);
            this.button2.Location = new Point(width + 10, this.button2.Location.Y);
            this.button3.Location = new Point(width + 10, this.button3.Location.Y);
            this.button4.Location = new Point(width + 10, this.button4.Location.Y);
            this.button5.Location = new Point(width + 10, this.button5.Location.Y);
            this.button6.Location = new Point(width + 10, this.button6.Location.Y);
            this.button7.Location = new Point(width + 10, this.button7.Location.Y);
            this.button8.Location = new Point(width + 10, this.button8.Location.Y);
            this.button9.Location = new Point(width + 10, this.button9.Location.Y);
            this.button10.Location = new Point(width + 10, this.button10.Location.Y);
            this.button11.Location = new Point(width + 10, this.button11.Location.Y);
            this.button12.Location = new Point(width + 10, this.button12.Location.Y);
            this.label1.Location = new Point(width + 10, this.label1.Location.Y);
            this.label2.Location = new Point(this.label1.Location.X + 10, this.label2.Location.Y);
        }

        private void MapVisualizer_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(MapVisualizer_Paint);
        }

        public void updateMap(Phenotype visual_map)
        {
            this.visual_map = visual_map;
        }

        public void loadMapAndPicture(Phenotype visual_map, string filename)
        {
            this.visual_map = visual_map;
            printScreen(filename);
        }

        private void MapVisualizer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {


            this.label1.Text = this.room1;
            this.label2.Text = this.room2;

            // Do Drawing of the map here!!! What is necessary - An outline of every room, with a door!
            // This is Visual Debugging Code - Check adjency of Rooms!
            // The system will go through each room, check its neighbours and draw a black line between tiles that aren't in the same room.
            if (!show_graph)
            {
                if (is_outline)
                {
                    // Draw in an Outline Fashion
                    foreach (Tile t in visual_map.getMap().getTiles())
                    {
                        // Prints Room ID on tiles --- Too Remove Numbers comment the next line ONLY!!!
                        // e.Graphics.DrawString(Convert.ToString(t.getRoom().getID()), new Font("Arial", 14, FontStyle.Bold), Brushes.Black, t.getCoords()[0], t.getCoords()[1]);
                        // e.Graphics.DrawString(Convert.ToString(t.getID()), new Font("Arial", 14, FontStyle.Bold), Brushes.Black, t.getCoords()[0], t.getCoords()[1]);

                        if (t.getRoom().getID() == 0)
                        {
                            float[] coords = t.getCoords();
                            HatchBrush myHatch = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Black, Color.White);
                            e.Graphics.FillRectangle(myHatch, coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
                        }

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
                }
                else
                {
                    List<Brush> brushes = this.getBrushList();
                    // Draw Color Coded Fashion
                    foreach (Tile t in visual_map.getMap().getTiles())
                    {
                        float[] coords = t.getCoords();
                        e.Graphics.FillRectangle(brushes[t.getRoom().getID() % brushes.Count], coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
                    }
                }


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

                // Draw Enemies and Items here!
                foreach (SpawnPoint sp in visual_map.getMap().getSpawnPoints())
                {
                    //int tiles_count = visual_map.getMap().getRoomByID(sp.getRoom()).getTiles().Count;
                    //Tile t = visual_map.getMap().getRoomByID(sp.getRoom()).getTiles()[(int)tiles_count/2];

                    // Draw on tile location.
                    if (visual_map.getMainItem() != null && sp.getType() == 0 && sp.getRoom() == visual_map.getMainItem().getRoom()) // ITEM!
                    {
                        float[] coords = sp.getTile().getCoords();//t.getCoords();
                        e.Graphics.FillRectangle(Brushes.Blue, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] +15)) - 15, (coords[3] - (coords[1] + 15)) - 15);
                    }
                    else if (sp.getType() == 4)
                    {
                        //float[] coords = sp.getTile().getCoords();

                        int roomID = sp.getTile().getRoom().getID();
                        int max = visual_map.getMap().getRoomByID(roomID).getTiles().Count;
                        float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[Util.MyRandom.getRandom().random().Next(max)].getCoords();

                        e.Graphics.FillEllipse(Brushes.Crimson, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] + 15)) - 15, (coords[3] - (coords[1] + 15)) - 15);
                        //e.Graphics.FillRectangle(Brushes.Crimson, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] + 15)) - 15, (coords[3] - (coords[1] + 15)) - 15);
                    }
                    else if (sp.getType() == 3)
                    {
                        int roomID = sp.getTile().getRoom().getID();
                        int max = visual_map.getMap().getRoomByID(roomID).getTiles().Count;
                        float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[Util.MyRandom.getRandom().random().Next(max)].getCoords();
                        
                        e.Graphics.FillPie(Brushes.DarkOrange, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] + 15)) - 15, (coords[3] - (coords[1] + 15)) - 15, 0f, 180f);
                        //e.Graphics.FillRectangle(Brushes.Crimson, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] + 15)) - 15, (coords[3] - (coords[1] + 15)) - 15);
                    }
                    else if (sp.getType() == 5)
                    {
                        int roomID = sp.getTile().getRoom().getID();
                        int max = visual_map.getMap().getRoomByID(roomID).getTiles().Count;
                        float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[Util.MyRandom.getRandom().random().Next(max)].getCoords();

                        e.Graphics.FillPie(Brushes.DarkViolet, coords[0] + 15, coords[1] + 15, (coords[2] - (coords[0] + 15)) - 15, (coords[3] - (coords[1] + 15)) - 15, 0f, 180f);
                    }
                    else if(sp.getType() == 1) // ENEMY
                    {
                        // Check to see if an item exists in this room
                        if (visual_map.getMap().itemExists(sp.getRoom()))
                        {
                            //int roomID = sp.getTile().getRoom().getID();
                            //float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[0].getCoords(); 

                            int roomID = sp.getTile().getRoom().getID();
                            int max = visual_map.getMap().getRoomByID(roomID).getTiles().Count;
                            float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[Util.MyRandom.getRandom().random().Next(max)].getCoords();

                            //float[] coords = sp.getTile().getCoords();//t.getCoords();
                            Point[] triangle = { new Point((int) ( ((coords[0] + coords[2]) / 2)), (int)coords[1] + 10), new Point((int)coords[0] + 10, (int)coords[3] - 10), new Point((int)coords[2] - 10, (int)coords[3] - 10) };

                            e.Graphics.FillPolygon(Brushes.ForestGreen, triangle);
                        }
                        else
                        {
                            //float[] coords = sp.getTile().getCoords();//t.getCoords();

                            int roomID = sp.getTile().getRoom().getID();
                            int max = visual_map.getMap().getRoomByID(roomID).getTiles().Count;
                            float[] coords = visual_map.getMap().getRoomByID(roomID).getTiles()[Util.MyRandom.getRandom().random().Next(max)].getCoords();

                            Point[] triangle = { new Point((int)((coords[0] + coords[2]) / 2), (int)coords[1] + 10), new Point((int)coords[0] + 10, (int)coords[3] - 10), new Point((int)coords[2] - 10, (int)coords[3] - 10) };

                            e.Graphics.FillPolygon(Brushes.ForestGreen, triangle);
                       
                        }
                         //e.Graphics.FillRectangle(Brushes.ForestGreen, coords[0] + 10, coords[1] + 10, (coords[2] - (coords[0] + 10)) - 10, (coords[3] - (coords[1] + 10)) - 10);
                    }

                }

                // If Path Finding View is turned on, draw the path.
                if (pathfinding)
                {
                    List<IEdge> path = Program.getPathFinding()[path_num];
                    foreach (IEdge edge in path)
                    {
                        Edge redge = visual_map.getGraph().getEdge(edge);

                        float[] coords = visual_map.getMap().getTileByID(redge.getVertexSource().getID()).getCoords();
                        e.Graphics.FillRectangle(Brushes.Green, coords[0]+15, coords[1]+15, (coords[2] - coords[0])-15, (coords[3] - coords[1])-15);

                        float[] coords2 = visual_map.getMap().getTileByID(redge.getVertexTarget().getID()).getCoords();
                        e.Graphics.FillRectangle(Brushes.Green, coords2[0]+15, coords2[1]+15, (coords2[2] - coords2[0])-15, (coords2[3] - coords2[1])-15);
                    }
                }

                // Path Finding for A* specificially
                if (astar)
                {
                    List<int> path = Program.getSpawnPath(visual_map.getMap().getSpawnPoints()[astar_num]);
                    foreach (int tile in path)
                    {
                        Tile redge = visual_map.getMap().getTileByID(tile);

                        float[] coords = redge.getCoords();
                        e.Graphics.FillRectangle(Brushes.Violet, coords[0] + 15, coords[1] + 15, (coords[2] - coords[0]) - 15, (coords[3] - coords[1]) - 15);

                    }
                }

                // If Adjacency View is turned on, draw the adjacent tiles of Room X to Room Y.
                if (show_adjencency)
                {
                    // Draw the Adjacency Tiles that are associated to room1 and room2
                    int room1Value = Convert.ToInt32(room1);
                    int room2Value = Convert.ToInt32(room2);

                    if (room1Value != room2Value)
                    {
                        // Get Adjacency Tiles
                        List<int[]> tileIds = visual_map.getMap().getAdjacentTileFromRoomToRoom(visual_map.getMap().getRoomByID(room1Value), visual_map.getMap().getRoomByID(room2Value));
                        foreach(int[] i in tileIds)
                        {
                            float[] coords = visual_map.getMap().getTileByID(i[0]).getCoords();
                            float[] coords2 = visual_map.getMap().getTileByID(i[1]).getCoords();

                            e.Graphics.FillRectangle(Brushes.Red, coords[0], coords[1], (coords[2] - coords[0]), (coords[3] - coords[1]));
                            e.Graphics.FillRectangle(Brushes.Blue, coords2[0], coords2[1], (coords2[2] - coords2[0]), (coords2[3] - coords2[1]));
                        }

                    }

                }
            }
            else
            {
                // Show Graph Code
                // Do graph drawing here!
                AdjGraph graph = visual_map.getGraph();

                float scaler = 0.7f;

                int INCREMENT = 15;
                int line_counter = 0;
                int column_counter = 0;
                // For each vertex we will draw!
                foreach (KeyValuePair<int, Vertex> v in graph.getVertices())
                {
                    float[] coords = visual_map.getMap().getTileByID(v.Value.getID()).getCoords();

                    if (line_counter % Map.WIDTH_TILE_SIZE == 0)
                    {
                        column_counter += INCREMENT;
                        line_counter = 0;
                    }

                    e.Graphics.DrawEllipse(Pens.Black, coords[0], coords[1], coords[2] - coords[0], coords[3] - coords[1]);//(coords[0] + column_counter) * scaler, (coords[1] + line_counter) * scaler, (coords[2] - coords[0]) * scaler, (coords[3] - coords[1]) * scaler);

                    foreach (IEdge edge in graph.OutEdges(v.Value.getIVertex()))
                    {
                        Edge edg = graph.getEdge(edge);
                        float[] coordsSource = visual_map.getMap().getTileByID(edg.getVertexSource().getID()).getCoords(); // Get Coords of Source
                        float[] coordsTarget = visual_map.getMap().getTileByID(edg.getVertexTarget().getID()).getCoords(); // Get Coords of Target

                        float[] ScaledCoordsSource = { (coordsSource[0] + column_counter) * scaler, (coordsSource[1] + line_counter) * scaler, (coordsSource[2] + column_counter) * scaler, (coordsSource[3] + line_counter) * scaler };
                        float[] ScaledCoordsTarget = { (coordsTarget[0] + column_counter) * scaler, (coordsTarget[1] + line_counter) * scaler, (coordsTarget[2] + column_counter) * scaler, (coordsTarget[3] + line_counter) * scaler };

                        Pen p;

                        if (edg.getType() == 0) // It's a Normal Connection!
                        {
                            p = new Pen(Color.Black);
                        }
                        else // It's a Door!
                        {
                            p = new Pen(Color.Green);
                        }

                        p.Width = 2;

                        float[] help = { ( (coordsSource[2] - coordsSource[0]) / 2) + coordsSource[0], ((coordsSource[3] - coordsSource[1]) / 2 ) + coordsSource[1], 
                                        ((coordsTarget[2] - coordsTarget[0]) / 2) + coordsTarget[0],  ((coordsTarget[3] - coordsTarget[1]) / 2 ) + coordsTarget[1] };
                        e.Graphics.DrawLine(p, ((coordsSource[2] - coordsSource[0]) / 2) + coordsSource[0], ((coordsSource[3] - coordsSource[1]) / 2) + coordsSource[1],
                                            ((coordsTarget[2] - coordsTarget[0]) / 2) + coordsTarget[0], ((coordsTarget[3] - coordsTarget[1]) / 2) + coordsTarget[1]);

                    }
                    line_counter += INCREMENT;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.mutateIt();
            this.Refresh();
        }

        // Changes the Visual Type from Color Coding to Only Lines
        private void button2_Click(object sender, EventArgs e)
        {
            this.show_graph = false;
            this.show_adjencency = false;
            if (is_outline)
            {
                this.is_outline = false;
            }
            else
            {
                this.is_outline = true;
            }
            this.Refresh();
        }

        private List<Brush> getBrushList()
        {
            List<Brush> brushes = new List<Brush>();

            brushes.Add(new SolidBrush(Color.Black));
            brushes.Add(new SolidBrush(Color.Blue));
            brushes.Add(new SolidBrush(Color.Green));
            brushes.Add(new SolidBrush(Color.Orange));
            brushes.Add(new SolidBrush(Color.Yellow));
            brushes.Add(new SolidBrush(Color.Magenta));
            brushes.Add(new SolidBrush(Color.Brown));
            brushes.Add(new SolidBrush(Color.Gray));
            brushes.Add(new SolidBrush(Color.Beige));
            brushes.Add(new SolidBrush(Color.Cyan));
            brushes.Add(new SolidBrush(Color.Fuchsia));
            brushes.Add(new SolidBrush(Color.HotPink));

            return brushes;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (show_graph)
            {
                this.show_graph = false;
            }
            else
            {
                this.show_graph = true;
            }
            this.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //if (!show_adjencency)
            //{
            //    show_adjencency = true;
            //    this.show_graph = false;
            //    this.room_num = 0;

            //}
            //else
            //{
            //    if (this.room_num >= visual_map.getMap().getRooms().Count-1)
            //    {
            //        this.room_num = 0;
            //    }
            //    else
            //    {
            //        this.room_num++;
            //    }
            //}

            //Console.WriteLine("Total Room Num " + visual_map.getMap().getRooms().Count);
            //Console.WriteLine("Current Room Num " + room_num);

            if (!show_adjencency)
            {
                show_adjencency = true;
            }
            else
            {
                this.show_adjencency = false;
            }

            this.Refresh();
        }

        //private List<int[]> getAdjacencyTiles(Tile t)
        //{
            
        //    List<int[]> adj_tiles = new List<int[]>();
        //    foreach (int[] i in visual_map.getMap().getAllRoomAdjacentTiles(visual_map.getMap().getRoomByID(room_num)))
        //    {
        //        if (i[0] == t.getID())
        //        {
        //            adj_tiles.Add(i);
        //        }
        //    }
        //    return adj_tiles;
        //}

        private void button5_Click(object sender, EventArgs e)
        {
            // Adjust pathfinder variables.
            if (!pathfinding)
            {
                path_num = 0;
                pathfinding = true;
            }
            else
            {
                if (path_num >= Program.getPathFinding().Count-1)
                {
                    pathfinding = false;
                    path_num = 0;
                }
                else
                {
                    path_num++;
                }
            }

            this.Refresh();
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            int curent_room = Convert.ToInt32(room1);
            if (curent_room >= visual_map.getMap().getRooms().Count - 1)
            {
                curent_room = 0;
            }
            else
            {
                curent_room++;
            }

            room1 = Convert.ToString(curent_room);
            this.Refresh();
        }

        private void label2_Click_1(object sender, EventArgs e)
        {
            int curent_room = Convert.ToInt32(room2);
            if (curent_room >= visual_map.getMap().getRooms().Count - 1)
            {
                curent_room = 0;
            }
            else
            {
                curent_room++;
            }

            room2 = Convert.ToString(curent_room);
            this.Refresh();
        }

        public void printScreen(string filename)
        {
            Bitmap bitmap = new Bitmap(this.Width, this.Height); 
            this.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            bitmap.Save(filename);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //printScreen();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new GameVisualizer(this.visual_map, false);
            form2.FormClosed += (s, args) => this.Close();
            form2.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Program.playSomething(visual_map);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Adjust pathfinder variables.
            if (!astar)
            {
                astar_num = 0;
                astar = true;
            }
            else
            {
                if (astar_num >= visual_map.getMap().getSpawnPoints().Count -1)
                {
                    astar = false;
                    astar_num = 0;
                }
                else
                {
                    astar_num++;
                }
            }

            this.Refresh();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new GameVisualizer(this.visual_map, true);
            form2.FormClosed += (s, args) => this.Close();
            form2.Show();
        }

        // Save Map Button
        private void button11_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "snc";
            sfd.Filter = "snc files (*.snc)|*.snc";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(sfd.FileName);
                ReadWriteToFile.writeToFile(visual_map, sfd.FileName);
            }

        }

        // Load Map Button
        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(ofd.FileName);
                Phenotype phen = ReadWriteToFile.readFromFile(ofd.FileName);
                Program.resetGenetic(phen);
            }
        }

    }
}
