using System.Collections;
using UnityEngine;

public class DoofusController : MonoBehaviour
{
    private GameConfig gameConfig;
    private GameManager gameManager;
    private CameraFollow cameraFollow;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    private bool isJumping = false;

    [Header("Speed Tuning")]
    [Range(1f, 3f)]
    public float speedMultiplier = 2.0f;

    [Header("Dash Settings")]
    public float dashSpeedMultiplier = 2.5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 3f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    [Header("Ground Detection")]
    public LayerMask pulpitLayer;
    public float raycastDistance = 2f;

    [Header("Fall Settings")]
    public float gravity = -20f;
    public float fallDeathY = -10f;
    private float fallVelocity = 0f;
    private bool isFalling = false;
    private bool isGameOver = false;

    [Header("Water Splash")]
    public GameObject splashEffectPrefab;
    public float waterY = -8f;
    private bool splashTriggered = false;

    private bool gameStarted = false;

    private void Start()
    {
        gameConfig = FindAnyObjectByType<GameConfig>();
        gameManager = FindAnyObjectByType<GameManager>();
        cameraFollow = FindAnyObjectByType<CameraFollow>();
    }

    public void SetGameStarted(bool started)
    {
        gameStarted = started;
    }

    public void ResetState(Vector3 startPosition)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        isFalling = false;
        fallVelocity = 0f;
        isGameOver = false;
        isJumping = false;
        splashTriggered = false;
        isDashing = false;
        dashTimer = 0f;
        dashCooldownTimer = 0f;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!gameStarted || isGameOver) return;

        if (isFalling)
        {
            fallVelocity += gravity * Time.deltaTime;
            transform.position += Vector3.up * fallVelocity * Time.deltaTime;

            if (!splashTriggered && transform.position.y <= waterY)
            {
                splashTriggered = true;
                isGameOver = true;

                if (splashEffectPrefab != null)
                {
                    Vector3 splashPos = new Vector3(transform.position.x, waterY, transform.position.z);
                    Instantiate(splashEffectPrefab, splashPos, Quaternion.identity);
                }

                StartCoroutine(DelayedGameOver());
            }
            return;
        }

        UpdateDashTimers();

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0f && movement.magnitude > 0.1f)
        {
            StartDash();
        }

        float effectiveSpeed = gameConfig.Data.player_data.speed * speedMultiplier;

        if (isDashing)
        {
            effectiveSpeed *= dashSpeedMultiplier;
        }

        transform.Translate(
            movement * effectiveSpeed * Time.deltaTime,
            Space.World
        );

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(Jump());
        }

        CheckCurrentPulpit();
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
    }

    private void UpdateDashTimers()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    // Optional: expose cooldown progress (0 = ready, 1 = just used) for a UI cooldown indicator
    public float GetDashCooldownProgress()
    {
        if (dashCooldown <= 0f) return 0f;
        return Mathf.Clamp01(dashCooldownTimer / dashCooldown);
    }

    private void CheckCurrentPulpit()
    {
        if (isJumping) return;

        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, pulpitLayer);

        if (grounded)
        {
            gameManager.OnDoofusLanded(hit.collider.gameObject);

            if (isFalling && cameraFollow != null)
            {
                cameraFollow.TriggerShake();
            }

            isFalling = false;
            fallVelocity = 0f;
        }
        else
        {
            isFalling = true;
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

    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.OnDoofusFell();
    }
}