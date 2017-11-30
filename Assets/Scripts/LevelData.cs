using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData {
    public int level;
    public int rows;
    public int cols;
    public int numTileValues;
    public int numMoves;
    public int mission; // 0 for reach amount of points
    public int missionGoal; 
}
