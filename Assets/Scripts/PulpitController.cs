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

    public void Initialize(float destroyTime, float spawnTriggerTime, GameManager manager)
    {
        this.destroyTime = destroyTime;
        this.spawnTriggerTime = spawnTriggerTime;
        this.gameManager = manager;
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

    private IEnumerator AnimateDestroy()
    {
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