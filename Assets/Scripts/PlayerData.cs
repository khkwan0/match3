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
    public int rainbowHelper;
    public int bombHelper;
    public int hammerHelper;
    public int horizontalHelper;
    public int verticalHelper;
    public int seenIntro;
}
