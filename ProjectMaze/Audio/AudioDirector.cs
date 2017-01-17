using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ProjectMaze.Audio.Interfaces;
using ProjectMaze.Game;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio
{
    /**
     * This is the sonification process of a level. The audio director will interpret the generated map and sonify it!
     * */
    public class AudioDirector
    {

        // Create a list of all sounds in a folder
        private string path = Directory.GetCurrentDirectory() + "\\soundbank";
        private Dictionary<int, string> sounddictionary;
        private Dictionary<int, ISoundPoint> point_list; // Constains the list of ISoundPoints from the AudioDirector 

        private List<int> playingIDs; // What IDs are currentle playing (to_play == true).
        private Phenotype pheno; // Its always good to keep track of the map just in case.

        
        private MixingWaveProvider32 mixer;

        // For now lets just create an Audio Point for each Room
        public AudioDirector(Phenotype pheno)
        {
            this.pheno = pheno;
            buildSoundDictionary();
            buildRoomSoundPointDictionary();
            initializeAudioPipeline();

            playingIDs = new List<int>();
        }

        // This is a tester solution to play only sound from one room -- this section will be improved for more intelligent behaviour!
        public void Update(Player player, List<int> toPlayIDs, List<int> toStopIDs, float volume)
        {
            foreach (int i in toPlayIDs)
            {
                if (!playingIDs.Contains(i))
                {
                    playingIDs.Add(i);
                    point_list[i].setPlay(true);
                    point_list[i].updateParam("volume", volume);
                }
            }

            foreach (int i in toStopIDs)
            {
                if (playingIDs.Contains(i))
                {
                    playingIDs.Remove(i);
                    point_list[i].setPlay(false);
                    point_list[i].updateParam("volume", 0);
                }
            }
        }

        // Not sure if necessary...
        public void DisposeAll()
        {
            AudioEngine.getInstance().Dispose();
        }

        private void buildSoundDictionary()
        {
            sounddictionary = new Dictionary<int, string>();
            int id = 0;
            foreach (string s in Directory.GetFiles(path, "*.wav").Select(Path.GetFileName))
            {
                Console.WriteLine(path + "\\" + s);
                sounddictionary.Add(id, path + "\\" + s);
                id++;
            }
        }

        private void buildRoomSoundPointDictionary()
        {
            point_list = new Dictionary<int, ISoundPoint>();

            int sound_inc = 0;
            // For each room lets create a sound point
            foreach (Room r in pheno.getMap().getRooms())
            {
                // Get a random tile from a room and set it as the "sound point tile"
                Tile t = r.getTiles()[r.getTiles().Count - 1];
                int sound_chooser = sound_inc % sounddictionary.Count; // THIS IS TEMPORARY FOR TESTING PURPOSES ONLY
                RoomSoundPoint rp = RoomSoundPoint.RoomSoundPointFactory(normalizeRoomSize(r.getTiles().Count), sounddictionary[sound_chooser], t); // Room Size for now == the number o tiles in a room
                point_list.Add(r.getID(), rp);
            }
        }

        // This function normalizes the number of tiles in a room to a value between 0 and 1. 
        private float normalizeRoomSize(int size)
        {
            int min_size = Map.MIN_ROOM_SIZE;
            int max_size = Map.HEIGHT_TILE_SIZE * Map.WIDTH_TILE_SIZE;

            return ((size - min_size) / (max_size - min_size));
        }

        private void initializeAudioPipeline()
        {
            AudioEngine.initializeEngine(); // Initialize the Driver Engine.
            LibPDSample.initializeLibPD(); // Initialize LibPD
            //this.mixer = new MixingSampleProvider(point_list.Values.ToList());
            this.mixer = new MixingWaveProvider32();
            this.mixer.AddInputStream(point_list[0]);
            AudioEngine.getInstance().Play(this.mixer);
        }


    }
}
