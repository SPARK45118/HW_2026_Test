using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject pulpitPrefab;
    private GameConfig gameConfig;
    private List<GameObject> activePulpits = new List<GameObject>();
    private Vector3 lastSpawnPosition;

    [Header("Spacing Settings")]
    public float pulpitSize = 9f;       // matches your Pulpit's width/depth
    public float gapMin = 1f;           // extra gap on top of touching distance
    public float gapMax = 3f;
    [Range(0f, 1f)] public float gapChance = 0.35f; // chance of a gap vs touching

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
    }

    public void OnPulpitSpawnTrigger(Vector3 currentPosition)
    {
        if (activePulpits.Count < 2)
        {
            Vector3 newPos = GetValidAdjacentPosition(lastSpawnPosition);
            if (newPos != Vector3.negativeInfinity)
                SpawnPulpit(newPos);
        }
    }

    private Vector3 GetValidAdjacentPosition(Vector3 origin)
    {
        // Shuffle directions so we don't always try forward first
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
                : pulpitSize; // exactly touching, edge-to-edge

            Vector3 candidate = origin + dir * distance;

            if (!OverlapsAnyPulpit(candidate))
                return candidate;
        }

        // All 4 directions blocked (rare) — fall back to same spot with gap doubled
        return origin + Directions[Random.Range(0, 4)] * (pulpitSize * 2f);
    }

    private bool OverlapsAnyPulpit(Vector3 candidate)
    {
        float minAllowedDistance = pulpitSize * 0.95f; // small tolerance
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
}