using UnityEngine;
using System.Collections;

public class PulpitController : MonoBehaviour
{
    private float destroyTime;
    private float spawnTriggerTime;
    private GameManager gameManager;
    private float timer = 0f;
    private bool spawnTriggered = false;
    private bool isDestroying = false;

    [Header("Warning Flash Settings")]
    public float warningDuration = 1f;
    public Color warningColor = Color.red;
    public float flashSpeed = 8f;

    [Header("Break Effect")]
    public GameObject breakEffectPrefab;

    private Renderer pulpitRenderer;
    private MaterialPropertyBlock propBlock;
    private Color originalColor;
    private bool warningStarted = false;

    public void Initialize(float destroyTime, float spawnTriggerTime, GameManager manager)
    {
        this.destroyTime = destroyTime;
        this.spawnTriggerTime = spawnTriggerTime;
        this.gameManager = manager;
    }

    private void Awake()
    {
        pulpitRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();

        if (pulpitRenderer != null)
        {
            originalColor = pulpitRenderer.sharedMaterial.color;
        }
    }

    private void Update()
    {
        if (isDestroying) return;

        timer += Time.deltaTime;

        if (!spawnTriggered && timer >= spawnTriggerTime)
        {
            spawnTriggered = true;
            gameManager.OnPulpitSpawnTrigger(transform.position);
        }

        float timeRemaining = destroyTime - timer;

        if (!warningStarted && timeRemaining <= warningDuration)
        {
            warningStarted = true;
            StartCoroutine(WarningFlash(timeRemaining));
        }

        if (timer >= destroyTime)
        {
            isDestroying = true;
            gameManager.OnPulpitDestroyed(gameObject);
            StartCoroutine(AnimateDestroy());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Doofus"))
        {
            gameManager.OnDoofusLanded(gameObject);
        }
    }

    private IEnumerator WarningFlash(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && !isDestroying)
        {
            elapsed += Time.deltaTime;

            float pulse = (Mathf.Sin(elapsed * flashSpeed) + 1f) * 0.5f;
            Color flashColor = Color.Lerp(originalColor, warningColor, pulse);

            pulpitRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", flashColor);
            pulpitRenderer.SetPropertyBlock(propBlock);

            yield return null;
        }
    }

    private IEnumerator AnimateDestroy()
    {
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.down * 1.5f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}