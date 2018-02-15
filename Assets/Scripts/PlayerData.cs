using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int overallScore;
    public int lastLevel;
    public int timeStamp;
    public List<PlayerLevelData> playerLevelData;
    public int musicOnOff;
    public int sfxOnOff;
    public float musicVolume;
    public float sfxVolume;
}
