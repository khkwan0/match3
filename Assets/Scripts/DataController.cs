using System.Collections;
using System.IO;
using UnityEngine;

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
        } else
        {
            Debug.LogError("Cannot load game data");
        }
    }

    public int NumLevels
    {
        get { return gameData.levelData.Count; }
    }
}