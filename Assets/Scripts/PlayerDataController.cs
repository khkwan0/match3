using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerDataController : MonoBehaviour {

	private string playerDataFile = "player.json";
    public PlayerData playerData;

    public void LoadPlayerData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, playerDataFile);
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
                playerLevelData = new List<PlayerLevelData>()
            };
            PlayerLevelData pld = new PlayerLevelData
            {
                score = 0,
                timestamp = -1,
                stars = 0
            };
            playerData.playerLevelData.Add(pld);
            string jsonString = JsonUtility.ToJson(playerData);
            File.WriteAllText(filePath, jsonString);
        }
    }

    public void SavePlayerData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, playerDataFile);
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
        SetPlayerOverallScore(numScore + playerData.overallScore);
        PlayerLevelData pld = new PlayerLevelData
        {
            score = numScore,
            timestamp = timestamp,
            stars = stars
        };
        Debug.Log(level);
        if (playerData.playerLevelData[level] == null)
        {
            playerData.playerLevelData.Add(pld);
            SetPlayerLastLevel(level);
        } else
        {
            playerData.playerLevelData[level] = pld;
        }
        SavePlayerData();
    }
}
