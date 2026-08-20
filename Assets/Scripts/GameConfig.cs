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

    [Header("Fallback Defaults (used if JSON fails to load)")]
    public float fallbackSpeed = 3f;
    public float fallbackMinDestroy = 4f;
    public float fallbackMaxDestroy = 5f;
    public float fallbackSpawnTime = 2.5f;

    private void Awake()
    {
        LoadData();
    }

    private void LoadData()
    {
        if (diaryFile == null)
        {
            Debug.LogWarning("Doofus Diary JSON is not assigned! Using fallback defaults.");
            UseFallbackDefaults();
            return;
        }

        try
        {
            GameData parsed = JsonUtility.FromJson<GameData>(diaryFile.text);

            if (parsed == null || parsed.player_data == null || parsed.pulpit_data == null)
            {
                Debug.LogWarning("Doofus Diary JSON is malformed or missing fields! Using fallback defaults.");
                UseFallbackDefaults();
                return;
            }

            if (parsed.player_data.speed <= 0f)
            {
                Debug.LogWarning("Speed value in JSON is invalid (<= 0). Using fallback speed.");
                parsed.player_data.speed = fallbackSpeed;
            }

            if (parsed.pulpit_data.min_pulpit_destroy_time <= 0f ||
                parsed.pulpit_data.max_pulpit_destroy_time <= 0f ||
                parsed.pulpit_data.min_pulpit_destroy_time > parsed.pulpit_data.max_pulpit_destroy_time)
            {
                Debug.LogWarning("Pulpit destroy time range in JSON is invalid. Using fallback range.");
                parsed.pulpit_data.min_pulpit_destroy_time = fallbackMinDestroy;
                parsed.pulpit_data.max_pulpit_destroy_time = fallbackMaxDestroy;
            }

            if (parsed.pulpit_data.pulpit_spawn_time <= 0f ||
                parsed.pulpit_data.pulpit_spawn_time >= parsed.pulpit_data.min_pulpit_destroy_time)
            {
                Debug.LogWarning("Pulpit spawn time in JSON is invalid or too late. Using fallback spawn time.");
                parsed.pulpit_data.pulpit_spawn_time = fallbackSpawnTime;
            }

            Data = parsed;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse Doofus Diary JSON: " + e.Message + ". Using fallback defaults.");
            UseFallbackDefaults();
        }
    }

    private void UseFallbackDefaults()
    {
        Data = new GameData
        {
            player_data = new PlayerData { speed = fallbackSpeed },
            pulpit_data = new PulpitData
            {
                min_pulpit_destroy_time = fallbackMinDestroy,
                max_pulpit_destroy_time = fallbackMaxDestroy,
                pulpit_spawn_time = fallbackSpawnTime
            }
        };
    }
}