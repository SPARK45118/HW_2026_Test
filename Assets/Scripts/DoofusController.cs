using System.Collections;
using UnityEngine;

public class DoofusController : MonoBehaviour
{
    private GameConfig gameConfig;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    private bool isJumping = false;

    [Header("Speed Tuning")]
    [Range(1f, 3f)]
    public float speedMultiplier = 1.5f;

    private void Start()
    {
        gameConfig = FindFirstObjectByType<GameConfig>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        float effectiveSpeed = gameConfig.Data.player_data.speed * speedMultiplier;

        transform.Translate(
            movement * effectiveSpeed * Time.deltaTime,
            Space.World
        );

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        float startY = transform.position.y;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            float height = 4 * jumpHeight * t * (1 - t);
            transform.position = new Vector3(transform.position.x, startY + height, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        isJumping = false;
    }
}