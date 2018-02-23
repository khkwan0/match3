using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//0 purple
//1 blue
//2 green
//3 orange
//4 red
//5 yellow
public class PlayerDataController : MonoBehaviour {

    private string playerDataFile = "player.json";
    public PlayerData playerData;

    public void LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFile);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(dataAsJson);
        }
        else
        {
            System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            playerData = new PlayerData
            {
                overallScore = 0,
                lastLevel = -1,
                timeStamp = secondsSinceEpoch,
                playerLevelData = new List<PlayerLevelData>(),
                sfxOnOff = 1,
                musicOnOff = 1,
                musicVolume = 1f,
                sfxVolume = 1f,
                rainbowHelper = 2,
                bombHelper = 2,
                horizontalHelper = 2,
                verticalHelper = 2,
                hammerHelper = 2
            };
            PlayerLevelData pld = new PlayerLevelData
            {
                score = 0,
                timestamp = -1,
                stars = 0
            };
            playerData.playerLevelData.Add(pld);
            string jsonString = JsonUtility.ToJson(playerData);
            Debug.Log(jsonString);
            File.WriteAllText(filePath, jsonString);
        }
    }

    public void StartLevel(int level)
    {
        int levelsCompleted = playerData.lastLevel;
        if (level > levelsCompleted)
        {
            for (int i = 0; i < (level - levelsCompleted); i++)
            {
                PlayerLevelData pld = new PlayerLevelData
                {
                    score = 0,
                    stars = 0,
                    timestamp = GetSecondsSinceEpoch(),
                    regular = new int[6],  
                    horizontal = new int[6],
                    vertical = new int[6],
                    cross = new int[6]
                };
                playerData.playerLevelData.Add(pld);
            }
        } else if (level >= 0) 
        {
            PlayerLevelData pld = new PlayerLevelData
            {
                score = 0,
                stars = 0,
                timestamp = GetSecondsSinceEpoch(),
                regular = new int[6],
                horizontal = new int[6],
                vertical = new int[6],
                cross = new int[6]
            };
            playerData.playerLevelData[level] = pld;
        }
    }

    public void AddRewards(List<Rewards> rewards)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            switch(rewards[i].reward)
            {
                case "hammer": playerData.hammerHelper++; break;
                case "rainbow": playerData.rainbowHelper++; break;
                case "vertical": playerData.verticalHelper++; break;
                case "bomb": playerData.bombHelper++;break;
                case "horizontal": playerData.horizontalHelper++; break;
                default: break;
            }
        }
    }
    public void EnableSFX(bool on) 
    {
        if (on)
        {
            playerData.sfxOnOff = 1;
        }
        else
        {
            playerData.sfxOnOff = 0;
        }
        SavePlayerData();
    }

    public void EnableMusic(bool on)
    {
        if (on)
        {
            playerData.musicOnOff = 1;
        }
        else
        {
            playerData.musicOnOff = 0;
        }
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, playerDataFile);
        string jsonString = JsonUtility.ToJson(playerData);
        File.WriteAllText(filePath, jsonString);
    }

    public void SetPlayerOverallScore(int overallScore)
    {
        playerData.overallScore = overallScore;
    }

    public void SetPlayerLastLevel(int lastLevel)
    {
        playerData.lastLevel = lastLevel;
    }

    public void PlayerSaveWin(int level, int numScore, int timestamp, int stars)
    {
        //PlayerLevelData pld = new PlayerLevelData
        //{
        //    score = numScore,
        //    timestamp = timestamp,
        //    stars = stars
        //};
        //Debug.Log(level);
        //if (playerData.playerLevelData[level] == null)
        //{
        //    playerData.playerLevelData.Add(pld);
        //    SetPlayerLastLevel(level);
        //} else
        //{
        //    playerData.playerLevelData[level] = pld;
        //}
        if (level > playerData.lastLevel)
        {
            SetPlayerLastLevel(level);
        }
        SavePlayerData();
    }

    public void SetLevelScore(int level, int score)
    {
        if (playerData.playerLevelData[level] != null)
        {
            playerData.playerLevelData[level].score = score;
            playerData.playerLevelData[level].timestamp = GetSecondsSinceEpoch();
        } else
        {
            PlayerLevelData pld = new PlayerLevelData
            {
                score = score,
                stars = 0,
                timestamp = GetSecondsSinceEpoch(),
                regular = new int[6],
                horizontal = new int[6],
                vertical = new int[6],
                cross = new int[6]
            };
            playerData.playerLevelData.Add(pld);
        }
    }

    public void AddLevelStar(int level)
    {
        playerData.playerLevelData[level].stars++;
    }

    public void AddTileCount(int level, TilePiece._TileType tileType, int value)
    {
        if (playerData.playerLevelData == null || playerData.playerLevelData.Count == 0 || playerData.playerLevelData[level] == null)
        {
            PlayerLevelData pld = new PlayerLevelData
            {
                score = 0,
                stars = 0,
                timestamp = GetSecondsSinceEpoch(),
                regular = new int[6],
                horizontal = new int[6],
                vertical = new int[6],
                cross = new int[6]
            };
            playerData.playerLevelData.Add(pld);
        }
        if (playerData.playerLevelData[level].regular.Length == 0)        
        {
            playerData.playerLevelData[level].regular = new int[6];
        }
        if (playerData.playerLevelData[level].horizontal.Length == 0)
        {
            playerData.playerLevelData[level].horizontal = new int[6];
        }
        if (playerData.playerLevelData[level].vertical.Length == 0)
        {
            playerData.playerLevelData[level].vertical = new int[6];
        }
        if (playerData.playerLevelData[level].cross.Length == 0)
        {
            playerData.playerLevelData[level].cross = new int[6];
        }
        switch (tileType)
        {
            case TilePiece._TileType.Regular:
                playerData.playerLevelData[level].regular[value]++;
                break;
            case TilePiece._TileType.HorizontalBlast:
                playerData.playerLevelData[level].horizontal[value]++;
                break;
            case TilePiece._TileType.VerticalBlast:
                playerData.playerLevelData[level].vertical[value]++;
                break;
            case TilePiece._TileType.CrossBlast:
                playerData.playerLevelData[level].cross[value]++;
                break;
            case TilePiece._TileType.Rainbow:
                playerData.playerLevelData[level].rainbow++;
                break;
            default: break;
        }        
    }

    public int GetTileCount(int level, TilePiece._TileType tileType, int value)
    {
        int count = 0;
        switch (tileType)
        {
            case TilePiece._TileType.Regular:
                count = playerData.playerLevelData[level].regular[value];
                break;
            case TilePiece._TileType.HorizontalBlast:
                count = playerData.playerLevelData[level].horizontal[value];
                break;
            case TilePiece._TileType.VerticalBlast:
                count = playerData.playerLevelData[level].vertical[value];
                break;
            case TilePiece._TileType.CrossBlast:
                count = playerData.playerLevelData[level].cross[value];
                break;
            case TilePiece._TileType.Rainbow:
                count = playerData.playerLevelData[level].rainbow;
                break;
            default:
                break;
        }
        return count;
    }

    private int GetSecondsSinceEpoch()
    {
        System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        return secondsSinceEpoch;
    }

    public void AddOverallScore(int score)
    {
        playerData.overallScore += score;
    }

    public void DeductHelper(GameController._helperType helperType)
    {
        switch (helperType) {
            case GameController._helperType.Bomb: playerData.bombHelper--; break;
            case GameController._helperType.Hammer: playerData.hammerHelper--; break;
            case GameController._helperType.Rainbow: playerData.rainbowHelper--; break;
            case GameController._helperType.Horizontal: playerData.horizontalHelper--; break;
            case GameController._helperType.Vertical: playerData.verticalHelper--; break;
            default: break;
        }
        SavePlayerData();
    }
}
