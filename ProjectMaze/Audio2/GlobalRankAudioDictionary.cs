using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectMaze.Audio2
{
    public class GlobalRankAudioDictionary
    {
        string path = Directory.GetCurrentDirectory() + "\\lookupTableValues";
        string soundIDFiles = "\\soundTableFull.csv";
        string effectRankings = "\\effectFamilyTensionGlobalRankSound_Combined.csv";
        string baseSoundRankings = "\\soundTensionGlobalRank.csv";

        private Dictionary<int, List<EffectInfo>> effectDictionary;
        private Dictionary<int, BaseSoundInfo> baseSoundDictionary;
        private Dictionary<int, List<int>> availableEffectFamilyDictionary;

        public Boolean useEffectsOnly = false;

        public Boolean logger = true;


        public GlobalRankAudioDictionary()
        {
            string soundIds = path + soundIDFiles;
            var reader = new StreamReader(File.OpenRead(@soundIds));

            effectDictionary = new Dictionary<int, List<EffectInfo>>();
            baseSoundDictionary = new Dictionary<int, BaseSoundInfo>();
            availableEffectFamilyDictionary = new Dictionary<int, List<int>>();

            reader.ReadLine(); // Ignore the first line
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                int soundID = Convert.ToInt32(values[0]);
                string fileName = values[1];
                int effectFamily = Convert.ToInt32(values[2]);
                int baseSoundID = Convert.ToInt32(values[3]);

                if (baseSoundID == -1)
                {
                    // Base Sound
                    baseSoundDictionary.Add(soundID, new BaseSoundInfo(fileName, soundID));
                }
                else
                {
                    // Effect Sound
                    if (!effectDictionary.ContainsKey(baseSoundID))
                    {
                        effectDictionary.Add(baseSoundID, new List<EffectInfo>());
                    }

                    effectDictionary[baseSoundID].Add(new EffectInfo(fileName, effectFamily, soundID, baseSoundID));
                }
            }

            reader.Close();
            Console.WriteLine("Loaded the Table IDs ... ");


            Console.WriteLine("Loading Effect Rankings ... ");
            string effectIds = path + effectRankings;
            var effectRankReader = new StreamReader(File.OpenRead(@effectIds));

            int currentSoundID = 0;
            while (!effectRankReader.EndOfStream)
            {
                var line = effectRankReader.ReadLine();
                var values = line.Split(',');

                if (values[3] == "")
                {
                    // Sound ID Line
                    currentSoundID = Convert.ToInt32(values[0].Split('_')[1]);

                    availableEffectFamilyDictionary.Add(currentSoundID, new List<int>());

                    // We know that the first line refers to the base sound 
                    line = effectRankReader.ReadLine();
                    values = line.Split(',');

                    int familyID = Convert.ToInt32(values[0]);
                    double globalRank = Convert.ToDouble(values[3]);

                    EffectInfo bSound = new EffectInfo(baseSoundDictionary[currentSoundID].getFileName(), 0, -1, currentSoundID);
                    bSound.setGlobalRankValue(globalRank);

                    effectDictionary[currentSoundID].Add(bSound);
                }
                else
                {
                    // 0 = Family ID (or Sound ID make sure to remove it) + 3 = Global Rank Value
                    int familyID = Convert.ToInt32(values[0]);
                    double globalRank = Convert.ToDouble(values[3]);

                    if (!availableEffectFamilyDictionary[currentSoundID].Contains(familyID))
                    {
                        availableEffectFamilyDictionary[currentSoundID].Add(familyID);
                    }

                    foreach (EffectInfo e in effectDictionary[currentSoundID])
                    {
                        if (e.getEffectFamily() == familyID)
                        {
                            e.setGlobalRankValue(globalRank);
                        }
                    }
                    
                }
            }
            effectRankReader.Close();
            Console.WriteLine("Finished Loading Effect Rankings ... ");

            Console.WriteLine("Loading Base Sound Rankings ... ");
            string baseSoundIds = path + baseSoundRankings;
            var baseSoundRankReader = new StreamReader(File.OpenRead(@baseSoundIds));
            while (!baseSoundRankReader.EndOfStream)
            {
                var line = baseSoundRankReader.ReadLine();
                var values = line.Split(',');

                int baseSoundID = Convert.ToInt32(values[0]);
                double globalRank = Convert.ToDouble(values[3]);

                baseSoundDictionary[baseSoundID].setGlobalRankValue(globalRank);
            }

            Console.WriteLine("Finished Loading Base Sound Rankings ... ");
        }

        public ISoundInfo[] suggestSoundList(Phenotype visual_map)
        {
            // Tuple goes Like (RoomID, TensionValue)
            List<MyTuple> tensionMap = visual_map.getAnxietyMap();
            int totalRooms = visual_map.getMap().getRooms().Count;

            // Lets do it with Sounds First

            
            ISoundInfo[] allocated_sounds = new ISoundInfo[totalRooms]; // Index = Room ID; Value = The Sound Allocated to that Room

            if (useEffectsOnly)
            {
                allocateSoundEffects(allocated_sounds, visual_map);
            }
            else
            {
                allocateSounds(allocated_sounds, visual_map);
            }

            if (logger)
            {
                logToFile(allocated_sounds);
            }

            Console.WriteLine("I Am Here");
            return allocated_sounds;
        }
        
        // This will create a soundscape of only base sounds.
        public void allocateSounds(ISoundInfo[] soundArray, Phenotype visual_map)
        {
            // Get the Tension Map.
            List<MyTuple> tensionMap = visual_map.getAnxietyMap();

            // This will order sounds by the value
            List<BaseSoundInfo> sounds = new List<BaseSoundInfo>();
            foreach (int i in baseSoundDictionary.Keys)
            {
                sounds.Add(baseSoundDictionary[i]);
            }
            List<BaseSoundInfo> SortedList = sounds.OrderByDescending(o => o.getGlobalRankValue()).ToList();

            // Sound Allocation Algorithm
            int soundDist = SortedList.Count / soundArray.Length; // The distance of each sound based on the size of the room

            // Make sure soundDist is never to high
            while( (soundDist * soundArray.Length) > SortedList.Count-1)
            {
                soundDist--;
            }

            List<MyTuple> backup = new List<MyTuple>();
            foreach(MyTuple tup in tensionMap)
            {
                backup.Add(tup.copy());
            }

            // Spread the Audio Love Across the Critical Path
            int inc = 0;
            int currentDist = 0;
            while (inc < tensionMap.Count)
            {
                MyTuple tup = backup.Max();
                soundArray[tup.getID()] = SortedList[currentDist];

                currentDist += soundDist;
                backup.Remove(tup);
                inc++;
            }

            // Check for Alternative Paths here -- Share the load!
            if (tensionMap.Count < soundArray.Length)
            {
                // Get The Alternative Path and Calculate the Remaining Tension Curve
                List<List<int>> alt_paths = visual_map.getAllSubPaths();

                // Right now were only doing this for the sub objective path -- For other paths we need to find a way to do it.
                foreach (List<int> subPath in alt_paths)
                {
                    // Get a Tension Map of the Sub Path.
                    List<MyTuple> subPathTension = AnxietySuspenseFitness.createTensionMap(subPath, visual_map);
                    backup = new List<MyTuple>();
                    foreach (MyTuple tup in subPathTension)
                    {
                        backup.Add(tup.copy());
                    }

                    inc = 0;
                    while (inc < subPathTension.Count)
                    {
                        MyTuple tup = backup.Max();
                        if (soundArray[tup.getID()] == null)
                        {
                            soundArray[tup.getID()] = SortedList[currentDist];
                            currentDist += soundDist;
                        }
                        inc++;
                        backup.Remove(tup);
                    }
                }
            }

            //If there are any rooms that still feel lonely (i.e. without a sound), assign one, we need to double check! Safety First!
            List<int> remainingRooms = new List<int>();
            for (int i = 0; i < soundArray.Length; i++)
            {
                if (soundArray[i] == null)
                {
                    remainingRooms.Add(i);
                    //soundArray[i] = SortedList[currentDist];
                    //currentDist += soundDist;
                }
            }

            // If there are remaining rooms, treat them as a single path and spread the audio love.
            if (remainingRooms.Count > 0)
            {
                List<MyTuple> subPathTension = AnxietySuspenseFitness.createTensionMap(remainingRooms, visual_map);
                while (subPathTension.Count > 0 )
                {
                    MyTuple tup = subPathTension.Max();
                    if (soundArray[tup.getID()] == null)
                    {
                        soundArray[tup.getID()] = SortedList[currentDist];
                        currentDist += soundDist;
                    }
                    subPathTension.Remove(tup);
                }
            }
        }

        // This will create a soundscape of only one base sound + all of its effects.
        public void allocateSoundEffects(ISoundInfo[] soundArray, Phenotype visual_map)
        {
            // Get the Tension Map.
            List<MyTuple> tensionMap = visual_map.getAnxietyMap();
            int maxRooms = visual_map.getMap().getRooms().Count;

            // Lets do with the total number of rooms in the map. If that doesn't work we'll use only the tension map.
            List<int> usableIDs = usableSounds(maxRooms);

            if(usableIDs.Count == 0)
            {
                // If Empty Default to using base sounds only.
                Console.WriteLine("Not enough Effect Data to Run Effect Sonification. Defaulting to Base Sound Data ... ");
                allocateSounds(soundArray, visual_map);
            }
            else
            {
                // If there is enough effect data we can choose one effect at random for now.
                int chosenID = usableIDs[MyRandom.getRandom().random().Next(usableIDs.Count)];

                // Obtain all the effects associated to this Sound ID
                List<int> effectsAvailable = availableEffectFamilyDictionary[chosenID];
                effectsAvailable.Add(0); // Add the base sound "effect"
                List<EffectInfo> listEffects = new List<EffectInfo>();

                foreach (int i in effectsAvailable)
                {
                    listEffects.Add(pickEffectByFamily(chosenID, i));
                }

                List<EffectInfo> SortedList = listEffects.OrderByDescending(o => o.getGlobalRankValue()).ToList(); // Order them by the Global Rank Value

                // Sound Allocation Algorithm
                int soundDist = SortedList.Count / soundArray.Length; // The distance of each sound based on the size of the room

                // Make sure soundDist is never to high
                while ((soundDist * soundArray.Length) > SortedList.Count - 1)
                {
                    soundDist--;
                }




                List<MyTuple> backup = new List<MyTuple>();
                foreach (MyTuple tup in tensionMap)
                {
                    backup.Add(tup.copy());
                }

                // Spread the Audio Love Across the Critical Path
                int inc = 0;
                int currentDist = 0;
                while (inc < tensionMap.Count)
                {
                    MyTuple tup = backup.Max();
                    soundArray[tup.getID()] = SortedList[currentDist];

                    currentDist += soundDist;
                    backup.Remove(tup);
                    inc++;
                }

                // Check for Alternative Paths here -- Share the load!
                if (tensionMap.Count < soundArray.Length)
                {
                    // Get The Alternative Path and Calculate the Remaining Tension Curve
                    List<List<int>> alt_paths = visual_map.getAllSubPaths();

                    // Right now were only doing this for the sub objective path -- For other paths we need to find a way to do it.
                    foreach (List<int> subPath in alt_paths)
                    {
                        // Get a Tension Map of the Sub Path.
                        List<MyTuple> subPathTension = AnxietySuspenseFitness.createTensionMap(subPath, visual_map);
                        backup = new List<MyTuple>();
                        foreach (MyTuple tup in subPathTension)
                        {
                            backup.Add(tup.copy());
                        }

                        inc = 0;
                        while (inc < subPathTension.Count)
                        {
                            MyTuple tup = backup.Max();
                            if (soundArray[tup.getID()] == null)
                            {
                                soundArray[tup.getID()] = SortedList[currentDist];
                                currentDist += soundDist;
                            }
                            inc++;
                            backup.Remove(tup);
                        }
                    }
                }

                //If there are any rooms that still feel lonely (i.e. without a sound), assign one, we need to double check! Safety First!
                List<int> remainingRooms = new List<int>();
                for (int i = 0; i < soundArray.Length; i++)
                {
                    if (soundArray[i] == null)
                    {
                        remainingRooms.Add(i);
                        //soundArray[i] = SortedList[currentDist];
                        //currentDist += soundDist;
                    }
                }

                // If there are remaining rooms, treat them as a single path and spread the audio love.
                if (remainingRooms.Count > 0)
                {
                    List<MyTuple> subPathTension = AnxietySuspenseFitness.createTensionMap(remainingRooms, visual_map);
                    inc = 0;
                    while (inc < subPathTension.Count)
                    {
                        MyTuple tup = subPathTension.Max();
                        if (soundArray[tup.getID()] == null)
                        {
                            soundArray[tup.getID()] = SortedList[currentDist];
                            currentDist += soundDist;
                        }
                        inc++;
                        subPathTension.Remove(tup);
                    }
                }
            }
            Console.WriteLine("Effect Has Finished!");
        }

        /**
        * This Function will return the Sound IDs that are usable based on the max room value. 
        * This can change based on our effect dataset as more results come in.
        */ 
        public List<int> usableSounds(int maxRooms)
        {
            List<int> usableSoundIds = new List<int>();
            int[] availableEffects = sortAvailableFamily();
            for (int i = 0; i < availableEffects.Length; i++)
            {
                if(availableEffects[i] >= maxRooms)
                {
                    usableSoundIds.Add(i);
                } 
            }
            return usableSoundIds;
        }

        public int[] sortAvailableFamily()
        {
            // For each base sound (Index), the value will be the total number of different effect families in that data set.
            int[] effectTypesPerSound = new int[baseSoundDictionary.Count];
            foreach (int bSound in availableEffectFamilyDictionary.Keys)
            {
                effectTypesPerSound[bSound] = availableEffectFamilyDictionary[bSound].Count;
            }
            return effectTypesPerSound;
        }

        // Picks a random asset of that effect based on both Sound ID and Effect Family ID
        public EffectInfo pickEffectByFamily(int baseSoundID, int effectFamilyID)
        {
            List<EffectInfo> familyEffects = effectDictionary[baseSoundID];
            List<EffectInfo> sameFamilyFiles = new List<EffectInfo>();

            foreach(EffectInfo eI in familyEffects)
            {
                if(eI.getEffectFamily() == effectFamilyID)
                {
                    sameFamilyFiles.Add(eI);
                }
            }
            return sameFamilyFiles[MyRandom.getRandom().random().Next(sameFamilyFiles.Count - 1)];
        }

        public void logToFile(ISoundInfo[] soundArray)
        {
            string mainLogFolder = Path.GetDirectoryName(Application.ExecutablePath) + "\\Logs";
            string filename = "soundAllocation.csv";
            
            // File Creator 
            //if (!File.Exists(mainLogFolder + "\\" + filename))
            //{
            //    File.Create(mainLogFolder + "\\" + filename);
            //}

            using (StreamWriter writer = new StreamWriter(mainLogFolder + "\\" + filename, false))
            {
                writer.Write("RoomID,SoundFile,Value,EffectFamily,BaseSound\n");
                for(int i = 0; i < soundArray.Length; i++)
                {
                    ISoundInfo current = soundArray[i];
                    if(current is BaseSoundInfo)
                    {
                        writer.Write(Convert.ToString(i) + "," + current.getFileName() + "," 
                            + Convert.ToString(current.getGlobalRankValue()) + ","
                            + "0," + Convert.ToString(current.getID()) + "\n");
                    }
                    else if(current is EffectInfo)
                    {
                        EffectInfo aux = (EffectInfo)current;
                        writer.Write(Convert.ToString(i) + "," + aux.getFileName() + "," 
                            + Convert.ToString(aux.getGlobalRankValue()) + ","
                            + Convert.ToString(aux.getEffectFamily()) + "," 
                            + Convert.ToString(aux.getBaseSoundID()) + "\n" );
                    }
                }
            }
        }
    }
}
