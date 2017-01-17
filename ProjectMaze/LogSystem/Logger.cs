using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.LogSystem
{
    public class Logger : ILogger
    {

        private FolderManagement folderManager;
        private string filename;
        private string photoLog;
        private string phenoLog;
        private string phenoTensionLog;

        private int ID; // ID of the type of Logger. 0 = Gen Log; 1 = Python Log; 

        // For Gen to Gen Logging
        public static Logger genLogFactory(FolderManagement folderManager, string filename)
        {
            string file = filename + ".csv";
            string picLog = "picLog";
            string phenoLog = "phenoLog";
            return new Logger(folderManager, file, picLog, phenoLog, 0);
        }

        // For Python Logging
        public static Logger expLogFactory(FolderManagement folderManager, string filename)
        {
            string file = filename + ".csv";
            return new Logger(folderManager, file, 1);
        }

        public static Logger LogFactory(FolderManagement folderManager)
        {
            return new Logger(folderManager);
        }

        private Logger(FolderManagement folderManager, string filename, string photoLog, string phenoLog, int ID)
        {
            this.folderManager = folderManager;
            this.ID = ID;
            this.filename = filename;

            // File Creator 
            if (!File.Exists(folderManager.getGenFolder() + "\\" + filename))
            {
                File.Create(folderManager.getGenFolder() + "\\" + filename);
            }
            //File.OpenWrite(folderManager.getGenFolder() + "\\" + filename);

            // PhotoLog Directory Creator
            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + photoLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + photoLog);
            }

            // PhenoLog Directory Creator
            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + phenoLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + phenoLog);
            }

        }

        private Logger(FolderManagement folderManager, string filename, int ID)
        {
            this.folderManager = folderManager;
            this.ID = ID;
            this.photoLog = "";
            this.phenoLog = "";
            this.filename = filename;

            if (!File.Exists(folderManager.getExperimentFolder() + "\\" + filename))
            {
                File.Create(folderManager.getExperimentFolder() + "\\" + filename);
            }
            //File.OpenWrite(folderManager.getExperimentFolder() + "\\" + filename);
        }

        private Logger(FolderManagement folderManager)
        {
            this.folderManager = folderManager;
            this.photoLog = "picLog";
            this.phenoLog = "phenoLog";
            this.phenoTensionLog = "levelTensionMapLog";
        }

        // Writes to File
        public void writeLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + filename, true))
            {
                writer.Write(log);
            }
        }

        public void writeLogTimeStamp(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + filename, true))
            {
                writer.Write(Stopwatch.GetTimestamp() + " : " + log);
            }
        }

        public void takePicture(IPhenotype pheno, string filename)
        {
            // PhotoLog Directory Creator
            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + photoLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + photoLog);
            }
            Program.loadPhenotypeToVisualizer((Phenotype)pheno, folderManager.getGenFolder() + "\\" + photoLog + "\\" + filename + ".bmp");
        }

        public void savePhenoStructure(IPhenotype pheno, string filename)
        {
            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + phenoLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + phenoLog);
            }

            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + phenoTensionLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + phenoTensionLog);
            }

            Util.ReadWriteToFile.writeToFile((Phenotype)pheno, folderManager.getGenFolder() + "\\" + phenoLog + "\\" + filename + ".snc");

            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + phenoTensionLog + "\\" + filename + ".txt", true))
            {
                int count = 0;
                foreach(MyTuple t in ((Phenotype)pheno).getAnxietyMap())
                {
                    if (count == 0)
                    {
                        writer.Write("[ " + Convert.ToString(t.getValue()) + " , ");
                    }
                    else if (count == ((Phenotype)pheno).getAnxietyMap().Count - 1)
                    {
                        writer.Write(Convert.ToString(t.getValue()) + " ]");
                    }
                    else
                    {
                        writer.Write(Convert.ToString(t.getValue()) + " , ");
                    }
                    count++;
                }
            }
        }

        public void setFolderManager(FolderManagement folderManager) // If type == 0 genFolder, else ExpFolder
        {
            this.folderManager = folderManager;
        }

        public FolderManagement getFolderManager()
        {
            return folderManager;
        }

        public void changeFile(string filename)
        {
            this.filename = filename + ".csv";
        }

        public void writePythonLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonLog.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonStructureFitnessLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\StructureFitnessLog.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonMonsterFitnessLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\MonsterFitnessLog.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonBestFitnessIndividual(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonBestFitnessIndividual.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonBestStructureFitnessIndividual(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonBestStructureFitnessIndividual.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonBestAnxietyFitnessIndividual(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonBestAnxietyFitnessIndividual.csv", true))
            {
                writer.Write(log);
            }
        }
    }
}
