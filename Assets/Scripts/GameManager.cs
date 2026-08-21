using UnityEngine;
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

    [Header("Challenge Complete UI")]
    public GameObject challengeCompletePanel;
    public TMP_Text challengeCompleteText;
    public int challengeTargetScore = 50;
    private bool challengeCompleteShown = false;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public bool isGameOver = false;

    [Header("Start Screen UI")]
    public GameObject startPanel;
    private bool gameStarted = false;

    [Header("Music")]
    public MusicController musicController;

    private DoofusController doofusController;
    private Vector3 doofusStartPosition;

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
        doofusController = FindAnyObjectByType<DoofusController>();

        if (doofusController != null)
        {
            doofusStartPosition = doofusController.transform.position;
        }

        Time.timeScale = 0f;
        gameStarted = false;

        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (challengeCompletePanel != null) challengeCompletePanel.SetActive(false);

        UpdateScoreUI();
    }

    public void OnPlayButtonPressed()
    {
        gameStarted = true;
        Time.timeScale = 1f;

        if (startPanel != null) startPanel.SetActive(false);

        if (musicController != null) musicController.PlayMusic();

        lastSpawnPosition = Vector3.zero;
        SpawnPulpit(lastSpawnPosition);

        if (doofusController != null)
        {
            doofusController.SetGameStarted(true);
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
        if (!gameStarted || isGameOver) return;

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
        if (!gameStarted || isGameOver) return;

        if (pulpit != currentPulpit)
        {
            currentPulpit = pulpit;
            score++;
            UpdateScoreUI();
            CheckChallengeComplete();
        }
    }

    private void CheckChallengeComplete()
    {
        if (challengeCompleteShown) return;

        if (score >= challengeTargetScore)
        {
            challengeCompleteShown = true;

            if (challengeCompleteText != null)
            {
                challengeCompleteText.text = "Hi Hitwicket Team!\nChallenge Complete - " + challengeTargetScore + " Pulpits Reached!";
            }

            if (challengeCompletePanel != null)
            {
                StartCoroutine(AnimateChallengePanel());
            }
        }
    }

    private IEnumerator AnimateChallengePanel()
    {
        challengeCompletePanel.SetActive(true);

        RectTransform rt = challengeCompletePanel.GetComponent<RectTransform>();
        CanvasGroup cg = challengeCompletePanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = challengeCompletePanel.AddComponent<CanvasGroup>();

        Vector2 offScreenPos = new Vector2(-400f, rt.anchoredPosition.y);
        Vector2 onScreenPos = rt.anchoredPosition;

        // Slide + fade in with a slight scale pop
        rt.anchoredPosition = offScreenPos;
        cg.alpha = 0f;
        rt.localScale = Vector3.one * 0.8f;

        float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float eased = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubic

            rt.anchoredPosition = Vector2.Lerp(offScreenPos, onScreenPos, eased);
            cg.alpha = Mathf.Lerp(0f, 1f, eased);
            rt.localScale = Vector3.one * Mathf.Lerp(0.8f, 1f, eased);

            yield return null;
        }

        rt.anchoredPosition = onScreenPos;
        cg.alpha = 1f;
        rt.localScale = Vector3.one;

        yield return new WaitForSeconds(6f);

        // Fade out
        elapsed = 0f;
        float fadeDuration = 0.3f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
        challengeCompletePanel.SetActive(false);
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

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Score: " + score;

        if (musicController != null) musicController.StopMusic();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        score = 0;
        challengeCompleteShown = false;

        if (scoreText != null) scoreText.text = "0";
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(false);
        if (challengeCompletePanel != null) challengeCompletePanel.SetActive(false);

        if (musicController != null) musicController.PlayMusic();

        foreach (GameObject p in activePulpits)
        {
            if (p != null) Destroy(p);
        }
        activePulpits.Clear();
        currentPulpit = null;

        if (doofusController != null)
        {
            doofusController.ResetState(doofusStartPosition);
        }

        lastSpawnPosition = Vector3.zero;
        SpawnPulpit(lastSpawnPosition);
    }
}