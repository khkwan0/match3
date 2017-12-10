using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData {
    public int level;
    public int rows;
    public int cols;
    public int numTileValues;
    public int numMoves;
    public MissionData mission;
    public List<BoardSpec> boardSpec;
}
