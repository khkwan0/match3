using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{

    private string gameLevelsFileName = "levels.json";
    public GameData gameData;

    public void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameLevelsFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(dataAsJson);
            Debug.Log(gameData.levelData[0].level);
        } else
        {
            Debug.LogError("Cannot load game data");
        }
    }
}