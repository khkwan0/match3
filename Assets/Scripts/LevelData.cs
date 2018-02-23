using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData {
    public int level;
    public int rows;
    public int cols;
    public int numTileValues;
    public int numMoves;
    public int maxTime;
    public int maxFillPoints;
    public int tier1Fill;
    public int tier2Fill;
    public int tier3Fill;
    public List<Helpers> helpers;
    public List<Rewards> rewards;
    public MissionData mission;
    public List<BoardSpec> boardSpec;
    public List<int> bombTypes;
    public int bombspawnrate;
    public int bombminmoves;
    public int bombmaxmoves;
    public int timer;
}
