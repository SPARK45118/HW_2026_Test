using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float speed;
}

[System.Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time;
    public float max_pulpit_destroy_time;
    public float pulpit_spawn_time;
}

[System.Serializable]
public class GameData
{
    public PlayerData player_data;
    public PulpitData pulpit_data;
}

public class GameConfig : MonoBehaviour
{
    public TextAsset diaryFile;

    public GameData Data { get; private set; }

    private void Awake()
    {
        LoadData();
    }

    private void LoadData()
    {
        if (diaryFile == null)
        {
            Debug.LogError("Doofus Diary JSON is not assigned!");
            return;
        }

        Data = JsonUtility.FromJson<GameData>(diaryFile.text);

        Debug.Log("Doofus Diary loaded successfully.");
        Debug.Log("Doofus Speed: " + Data.player_data.speed);
    }
}