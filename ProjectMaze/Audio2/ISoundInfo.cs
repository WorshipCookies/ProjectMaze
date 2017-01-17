namespace ProjectMaze.Audio2
{
    public interface ISoundInfo
    {

        string getFileName();

        void setGlobalRankValue(double globalRankValue);

        double getGlobalRankValue();

        int getID();
    }
}