using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject pulpitPrefab;
    private GameConfig gameConfig;
    private List<GameObject> activePulpits = new List<GameObject>();
    private Vector3 lastSpawnPosition;

    [Header("Spacing Settings")]
    public float pulpitSize = 9f;
    public float gapMin = 1f;
    public float gapMax = 3f;
    [Range(0f, 1f)] public float gapChance = 0.35f;

    [Header("Scoring")]
    public TMP_Text scoreText;
    private GameObject currentPulpit;
    public int score = 0;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;    // drag GameOverPanel here
    public TMP_Text finalScoreText;     // drag FinalScoreText here
    public bool isGameOver = false;

    private static readonly Vector3[] Directions = new Vector3[]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    private void Start()
    {
        gameConfig = FindAnyObjectByType<GameConfig>();
        lastSpawnPosition = Vector3.zero;
        SpawnPulpit(lastSpawnPosition);

        UpdateScoreUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void SpawnPulpit(Vector3 position)
    {
        GameObject newPulpit = Instantiate(pulpitPrefab, position, Quaternion.identity);
        PulpitController controller = newPulpit.GetComponent<PulpitController>();

        float randomDestroyTime = Random.Range(
            gameConfig.Data.pulpit_data.min_pulpit_destroy_time,
            gameConfig.Data.pulpit_data.max_pulpit_destroy_time
        );

        controller.Initialize(randomDestroyTime, gameConfig.Data.pulpit_data.pulpit_spawn_time, this);
        activePulpits.Add(newPulpit);
        lastSpawnPosition = position;

        if (currentPulpit == null)
        {
            currentPulpit = newPulpit;
        }
    }

    public void OnPulpitSpawnTrigger(Vector3 currentPosition)
    {
        if (isGameOver) return;

        if (activePulpits.Count < 2)
        {
            Vector3 newPos = GetValidAdjacentPosition(lastSpawnPosition);
            if (newPos != Vector3.negativeInfinity)
                SpawnPulpit(newPos);
        }
    }

    private Vector3 GetValidAdjacentPosition(Vector3 origin)
    {
        List<Vector3> dirs = new List<Vector3>(Directions);
        for (int i = dirs.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }

        foreach (Vector3 dir in dirs)
        {
            bool useGap = Random.value < gapChance;
            float distance = useGap
                ? pulpitSize + Random.Range(gapMin, gapMax)
                : pulpitSize;

            Vector3 candidate = origin + dir * distance;

            if (!OverlapsAnyPulpit(candidate))
                return candidate;
        }

        return origin + Directions[Random.Range(0, 4)] * (pulpitSize * 2f);
    }

    private bool OverlapsAnyPulpit(Vector3 candidate)
    {
        float minAllowedDistance = pulpitSize * 0.95f;
        foreach (GameObject p in activePulpits)
        {
            if (p == null) continue;
            float dist = Vector3.Distance(
                new Vector3(p.transform.position.x, 0, p.transform.position.z),
                new Vector3(candidate.x, 0, candidate.z)
            );
            if (dist < minAllowedDistance)
                return true;
        }
        return false;
    }

    public void OnPulpitDestroyed(GameObject pulpit)
    {
        activePulpits.Remove(pulpit);
    }

    public void OnDoofusLanded(GameObject pulpit)
    {
        if (isGameOver) return;

        if (pulpit != currentPulpit)
        {
            currentPulpit = pulpit;
            score++;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
            StartCoroutine(ScorePopEffect());
        }
    }

    private IEnumerator ScorePopEffect()
    {
        Vector3 originalScale = scoreText.transform.localScale;
        Vector3 poppedScale = originalScale * 1.3f;

        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scoreText.transform.localScale = Vector3.Lerp(originalScale, poppedScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scoreText.transform.localScale = Vector3.Lerp(poppedScale, originalScale, elapsed / duration);
            yield return null;
        }
    }

    public void OnDoofusFell()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}