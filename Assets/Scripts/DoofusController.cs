using System.Collections;
using UnityEngine;

public class DoofusController : MonoBehaviour
{
    private GameConfig gameConfig;
    private GameManager gameManager;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    private bool isJumping = false;

    [Header("Speed Tuning")]
    [Range(1f, 3f)]
    public float speedMultiplier = 1.5f;

    [Header("Ground Detection")]
    public LayerMask pulpitLayer;
    public float raycastDistance = 2f;

    [Header("Fall Settings")]
    public float gravity = -20f;       // acceleration, negative = downward
    public float fallDeathY = -10f;    // trigger game over once Doofus drops this far
    private float fallVelocity = 0f;
    private bool isFalling = false;
    private bool isGameOver = false;

    private void Start()
    {
        gameConfig = FindAnyObjectByType<GameConfig>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (isFalling)
        {
            fallVelocity += gravity * Time.deltaTime;
            transform.position += Vector3.up * fallVelocity * Time.deltaTime;

            if (transform.position.y < fallDeathY)
            {
                isGameOver = true;
                gameManager.OnDoofusFell();
            }
            return; // skip normal movement/input while falling
        }

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

        CheckCurrentPulpit();
    }

    private void CheckCurrentPulpit()
    {
        if (isJumping) return; // don't count as falling mid-jump

        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, pulpitLayer);

        if (grounded)
        {
            gameManager.OnDoofusLanded(hit.collider.gameObject);
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
}