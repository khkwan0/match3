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
            Debug.Log(dataAsJson);
        }
        else
        {
            playerData = new PlayerData
            {
                overallScore = 0,
                lastLevel = -1
            };
            string jsonString = JsonUtility.ToJson(playerData);
            File.WriteAllText(filePath, jsonString);
        }
    }
}
