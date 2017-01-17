using ProjectMaze.Game;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticAlgorithm.GraphRepresentation;
using ProjectMaze.Util;
using ProjectMaze.Visual;
using QuickGraph.Concepts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class AudioDirector2
    {
        private string path = Directory.GetCurrentDirectory() + "\\soundbank"; // This can be customized for different types of sound
        private string soundfx_path = Directory.GetCurrentDirectory() + "\\soundfx";

        private Dictionary<int, AudioFileObject> SoundFileDictionary;
        
        private Dictionary<int, AudioFXFileObject> SoundFXFileDictionary;

        private Dictionary<int, SoundStream> SoundStreamDictionary;

        private Dictionary<int, SoundStream> SoundStreamFXDictionary;

        private Dictionary<int, NeighbourDictionary> RoomNeighbours;

        private int previousSoundStreamID; // a list of streams that are currently being played in the background.

        private Phenotype visual_map;

        GlobalRankAudioDictionary globalRanks;

        // For now we will create a sound for each room
        public AudioDirector2(Phenotype visual_map)
        {
            this.visual_map = visual_map;

            globalRanks = new GlobalRankAudioDictionary();

            buildSoundFileDictionary(); // Always build File Dictionary First!
            
            //buildSoundFXFileDictionary(); // Build the Sound FX Dictionary.

            buildSoundStreamDictionary();
            constructNeighboursList();

            previousSoundStreamID = -1; 
        }

        public void Update(Player p)
        {
            updatePlaySound(p);
            
        }

        private void updateSoundStructure(Player p)
        {
            List<int> neighbours = RoomNeighbours[p.getCurrentRoom()].getNeighbours();
            List<int> playing = whoIsPlaying();
            playing = playing.Except(playing.Where(play => neighbours.Any(n => play == n))).ToList();

            foreach (int n in neighbours)
            {
                if(SoundStreamDictionary.ContainsKey(n))
                    SoundStreamDictionary[n].setVolume(translateDistanceToVolume(p, RoomNeighbours[p.getCurrentRoom()].getNeighbourPoint(n)));

                //if (SoundStreamFXDictionary.ContainsKey(n))
                //{
                //    SoundStreamFXDictionary[n].setVolume(translateDistanceToVolume(p, RoomNeighbours[p.getCurrentRoom()].getNeighbourPoint(n)));
                //}
            }

            foreach (int n in playing)
            {
                SoundStreamDictionary[n].setVolume(0.0f);

                //if (SoundStreamFXDictionary.ContainsKey(n))
                //{
                //    SoundStreamFXDictionary[n].setVolume(0.0f);
                //}

            }

            SoundStreamDictionary[p.getCurrentRoom()].setVolume(1.0f);
        }

        // Method that will update what is currently being played by the mixer.
        private void updatePlaySound(Player p)
        {
            updateSoundStructure(p);
            //if (previousSoundStreamID != p.getCurrentRoom())
            //{
            //    if (previousSoundStreamID == -1) // Kickstarting the system
            //    {
            //        SoundStreamDictionary[p.getCurrentRoom()].setVolume(7.0f); // For Now just play sound that is attached to that specific room!
            //        Console.WriteLine("Kickstarted!");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Stream Switched from " + previousSoundStreamID + " to " + p.getCurrentRoom());
            //        SoundStreamDictionary[previousSoundStreamID].setVolume(0.05f);
            //        SoundStreamDictionary[p.getCurrentRoom()].setVolume(1.0f);
            //    }
            //    previousSoundStreamID = p.getCurrentRoom();
            //}
        }

        // Build a dictionary of file strings that are in the bank (and could potentially be loaded by the Audio Director).
        private void buildSoundFileDictionary()
        {
            SoundFileDictionary = new Dictionary<int, AudioFileObject>();
            int id = 0;
            foreach (string s in Directory.GetFiles(path, "*.wav").Select(Path.GetFileName)) // Create an object for sound!
            {
                Console.WriteLine(path + "\\" + s);
                SoundFileDictionary.Add(id, new AudioFileObject(path, s));
                id++;
            }
        }

        // Build a sound fx dictionary of file strings that are in the bank (and could potentially be loaded by the Audio Director).
        private void buildSoundFXFileDictionary()
        {
            SoundFXFileDictionary = new Dictionary<int, AudioFXFileObject>();
            int id = 0;
            foreach (string s in Directory.GetFiles(soundfx_path, "*.wav").Select(Path.GetFileName)) // Create an object for sound!
            {
                Console.WriteLine(soundfx_path + "\\" + s);
                SoundFXFileDictionary.Add(id, new AudioFXFileObject(soundfx_path, s));
                id++;
            }
        }

        // For now attaches a sound to each room. More Intelligent way of picking sound to play is required!
        private void buildSoundStreamDictionary()
        {
            SoundStreamDictionary = new Dictionary<int, SoundStream>();
            //Queue<int> roomPath = getPathRoomSequence(visual_map.getPaths()[0]); // Get critical path
            //Queue<MyTuple> roomPath = getAnxietyPathRoomSequence();

            List<MyTuple> roomPath = new List<MyTuple>(visual_map.getAnxietyMap());

                        

            //List<AudioFileObject> audioFiles = streamSelection(roomPath); // Old Sound Selection Code.
            List<AudioFileObject> audioFiles = globalRankStreamSelection(roomPath); // New Global Rankings Code
            
            for (int i = 0; i < audioFiles.Count; i++)
            {
                SoundStream ss = new SoundStream(audioFiles[i].getFileName(), true, audioFiles[i].getValue()); // Sound Stream Creation
                SoundStreamDictionary.Add(i, ss); // Associate the Sound Stream to the Room
                addSoundStreamToMixer(ss); // Add the Sound Stream to the Mixer
            }

            // Temp Sound FX Build
            //streamFXSelection(new List<MyTuple>(roomPath));
        }

        // Adds a Sound Stream to the Audio Playback Engine Mixer.
        private void addSoundStreamToMixer(SoundStream sStream)
        {
            AudioPlaybackEngine.Instance.AddMixerInput(sStream);
        }

        private void constructNeighboursList()
        {
            RoomNeighbours = new Dictionary<int,NeighbourDictionary>();

            // Neighbours for this version are rooms that are only connected by doors
            foreach (Room r in visual_map.getMap().getRooms())
            {
                RoomNeighbours.Add(r.getID(), new NeighbourDictionary(r.getID(), visual_map.getMap().getDoors()));
            }
            Console.WriteLine("");
        }

        // THIS NEEDS TO BE PERFECTED!!! 
        private float translateDistanceToVolume(Player p, float[] point)
        {
            float dist = (float)Math.Sqrt(Math.Pow((p.getX() - point[0]), 2) + Math.Pow((p.getY() - point[1]), 2));
            //Console.WriteLine("Distance is " + dist);
            //if (dist <= 100f)
            //{
            //    return 1f;
            //}
            //else if (dist <= 125f)
            //{
            //    return 0.5f;
            //}
            //else if (dist <= 150f)
            //{
            //    return 0.1f;
            //}
            //else if(dist <= 200f)
            //{
            //    return 0.05f;
            //} else 
            //{
            //    return 0.00f;
            //}
            return distanceToVolume(dist);
        }


        // New Improved Way of Selecting Sounds -- Using Global Ranks
        private List<AudioFileObject> globalRankStreamSelection(List<MyTuple> values)
        {
            // In this situation we might want the GlobalRankStreamSelection to "suggest" what list of sounds to use
            ISoundInfo[] chosenAssets = globalRanks.suggestSoundList(visual_map);

            List<AudioFileObject> audioFileObject_List = new List<AudioFileObject>();
            for(int i = 0; i < chosenAssets.Length; i++)
            {
                audioFileObject_List.Add(new AudioFileObject(path,chosenAssets[i]));
            }
            return audioFileObject_List;
        }

        private List<AudioFileObject> streamSelection()
        {
            int numRooms = visual_map.getMap().getRooms().Count;
            List<AudioFileObject> audioFiles = new List<AudioFileObject>();
            List<int> instIDs = ListInstrumentID();
            for (int i = 0; i < numRooms; i++)
            {
                int chosenInst = instIDs[MyRandom.getRandom().random().Next(instIDs.Count)];
                List<AudioFileObject> files = getFileFromInstrumentID(chosenInst, audioFiles);
                while (files.Count <= 0)
                {
                    instIDs.Remove(chosenInst);
                    chosenInst = instIDs[MyRandom.getRandom().random().Next(instIDs.Count)];
                    files = getFileFromInstrumentID(chosenInst, files);
                }
                audioFiles.Add(files[MyRandom.getRandom().random().Next(files.Count)]);
                //instIDs.Remove(chosenInst);
            }
            audioFiles = audioFiles.OrderBy(o => o.getValue()).ToList(); // Order audio files by value (THIS COULD CHANGE IN THE FUTURE!)
            return audioFiles;
        }

        private List<AudioFileObject> streamSelection(List<MyTuple> values)
        {
            List<int> instIDs = ListInstrumentID();
            //List<AudioFileObject> files = new List<AudioFileObject>(visual_map.getMap().getRooms().Count);
            AudioFileObject[] files = new AudioFileObject[visual_map.getMap().getRooms().Count];


            List<AudioFileObject> shuffler = new List<AudioFileObject>();

            // Initialize AudioFileList
            for(int i = 0; i < visual_map.getMap().getRooms().Count; i++)
            {
                int chosenInst = instIDs[MyRandom.getRandom().random().Next(instIDs.Count)];
                List<AudioFileObject> inst_files = getFileFromInstrumentID(chosenInst, shuffler);
                while(inst_files.Count <= 0)
                {
                    instIDs.Remove(chosenInst);
                    chosenInst = instIDs[MyRandom.getRandom().random().Next(instIDs.Count)];
                    inst_files = getFileFromInstrumentID(chosenInst, shuffler);
                }
                shuffler.Add(inst_files[MyRandom.getRandom().random().Next(inst_files.Count)]);
                //instIDs.Remove(chosenInst);
            }

            List<MyTuple> backup_val = values;
            // Distribute the files according to intensity and path.
            while (backup_val.Count > 0)
            {
                int max_index = getMaxIntensityValue(backup_val);
                MyTuple max_tuple = backup_val[max_index];

                int max_audio_index = getMinIntensityAudioFile(shuffler);
                AudioFileObject max_file = shuffler[max_audio_index];

                files[max_tuple.getID()] = max_file;

                backup_val.RemoveAt(max_index);
                shuffler.RemoveAt(max_audio_index);
            }

            // Distribute the remaining sound files to the rest of the level.
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == null)
                {
                    int rand_select = MyRandom.getRandom().random().Next(shuffler.Count);
                    files[i] = shuffler[rand_select];
                    shuffler.RemoveAt(rand_select);
                }
            }
            return new List<AudioFileObject>(files);
        }

        private void streamFXSelection(List<MyTuple> values)
        {
            SoundStreamFXDictionary = new Dictionary<int, SoundStream>();
            if (values.Count > 0)
            {
                int index_max = 0;
                for (int i = 0; i< values.Count; i++)
                {
                    if (values[index_max].getValue() <= values[i].getValue())
                    {
                        index_max = i;
                    }
                }

                if (index_max != 0)
                {
                    AudioFXFileObject afo = SoundFXFileDictionary[MyRandom.getRandom().random().Next(SoundFXFileDictionary.Count-1)];
                    SoundStream ss = new SoundStream(afo.getFileName(), true, afo.getValue());
                    SoundStreamFXDictionary.Add(values[index_max].getID(), ss);
                    addSoundStreamToMixer(ss);
                }
            }
        }

        private List<int> ListInstrumentID()
        {
            List<int> instrumentIDs = new List<int>();
            foreach(AudioFileObject a in SoundFileDictionary.Values)
            {
                if (!instrumentIDs.Contains(a.getInstrumentID()))
                {
                    instrumentIDs.Add(a.getInstrumentID());
                }
            }
            return instrumentIDs;
        }

        private List<AudioFileObject> getFileFromInstrumentID(int id, List<AudioFileObject> currentFiles)
        {
            List<AudioFileObject> files = new List<AudioFileObject>();

            foreach (AudioFileObject a in SoundFileDictionary.Values)
            {
                if (a.getInstrumentID() == id && !currentFiles.Contains(a))
                {
                    files.Add(a);
                }
            }
            return files;
        }

        private Queue<int> getPathRoomSequence(List<IEdge> path)
        {
            Queue<int> room_seq = new Queue<int>();
            foreach (IEdge edge in path)
            {
                Edge redge = visual_map.getGraph().getEdge(edge);
                int sourceID = visual_map.getMap().getTileByID(redge.getVertexSource().getID()).getRoom().getID();
                int targetID = visual_map.getMap().getTileByID(redge.getVertexTarget().getID()).getRoom().getID();

                if (!room_seq.Contains(sourceID))
                {
                    room_seq.Enqueue(sourceID);
                }

                if (!room_seq.Contains(targetID))
                {
                    room_seq.Enqueue(targetID);
                }
            }
            return room_seq;
        }

        private Queue<MyTuple> getAnxietyPathRoomSequence()
        {
            List<int> mainPath = visual_map.getMainPath();
            //Queue<MyTuple> values = linearTension(mainPath);
            Queue<MyTuple> values = sawTension(mainPath);
            return values;

        }

        private List<int> whoIsPlaying()
        {
            List<int> playing = new List<int>();
            foreach (int n in SoundStreamDictionary.Keys)
            {
                if (SoundStreamDictionary[n].getVolume() > 0.0f)
                {
                    playing.Add(n);
                }
            }
            return playing;
        }

        public void printSoundsPlaying()
        {
            Console.Clear();
            foreach (SoundStream ss in SoundStreamDictionary.Values)
            {
                Console.WriteLine(ss.getFileName() + " is at Volume " + ss.getVolume());
            }
        }

        public void printIntensityOfRoom(int roomID)
        {
            Console.WriteLine("Sound Intensity of Room " + roomID + " is " + SoundStreamDictionary[roomID].getInstensity());
        }

        public float distanceToVolume(float distance)
        {
            double dist = (double)distance;

            float volume = (float) (- ( 250 * ( Math.Log(0.01 * dist, 2.0) ) / (Math.Log(Math.E, 2.0) ) ) );

            if (volume < 0)
            {
                return 0;
            }
            else
            {
                volume = ( (volume / 250) * 0.9f );
                
                if (volume > 0.9f) // Failsafe...
                    volume = 0.9f;

                //Console.WriteLine("Distance = " + dist + " ; Volume = " + volume);
                return volume;
            }

            
        }

        public bool containsRoom(Queue<MyTuple> values, int roomID)
        {
            foreach (MyTuple mt in values)
            {
                if (mt.getID() == roomID)
                    return true;
            }
            return false;
        }

        public int getMaxIntensityValue(List<MyTuple> values)
        {
            if (values.Count > 0)
            {
                MyTuple max = values[0];
                int max_index = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    if (max.getValue() <= values[i].getValue())
                    {
                        max = values[i];
                        max_index = i;
                    }
                }
                return max_index;
            }
            else
            {
                return -1;
            }
        }

        public int getMinIntensityAudioFile(List<AudioFileObject> shuffler)
        {
            if (shuffler.Count > 0)
            {
                AudioFileObject max = shuffler[0];
                int max_index = 0;
                for (int i = 0; i < shuffler.Count; i++)
                {
                    if (max.getValue() >= shuffler[i].getValue())
                    {
                        max = shuffler[i];
                        max_index = i;
                    }
                }
                return max_index;
            }
            else
            {
                return -1;
            }
        }

        // Linear Tension Structure.
        public Queue<MyTuple> linearTension(List<int> path)
        {
            Queue<MyTuple> values = new Queue<MyTuple>();
            double incrementer = 1 / path.Count;
            double current_value = 0;
            foreach (int i in path)
            {
                values.Enqueue(new MyTuple(i, current_value));
                current_value += incrementer;
            }
            return values;
        }

        // Saw Tension Structure
        public Queue<MyTuple> sawTension(List<int> path)
        {
            List<MyTuple> values = new List<MyTuple>();

            // Create Anxiety Mapping
            for (int i = 0; i < path.Count; i++)
            {
                int roomID = path[i];
                
                if (!AnxietySuspenseFitness.containsRoom(values, roomID))
                {
                    // If there is a monster in the room 
                    if (AnxietySuspenseFitness.checkForMonster(roomID, visual_map))
                    {
                        values.Add(new MyTuple(roomID, 1.0));
                    }
                    else
                    {
                        values.Add(new MyTuple(roomID, 0.0));
                    }
                }
            }

            AnxietySuspenseFitness.refineAnxietyMap(values);

            Queue<MyTuple> tuples = new Queue<MyTuple>();
            foreach (MyTuple t in values)
            {
                tuples.Enqueue(t);
            }
            return tuples;
        }
    }
}
