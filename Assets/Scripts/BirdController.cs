using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BirdController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Flight")]
    [SerializeField] private float jumpForce = 5.5f;
    [SerializeField] private float maxUpRotation = 25f;
    [SerializeField] private float maxDownRotation = -70f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    [Header("Animation")]
    [SerializeField] private Sprite[] flyingSprites;
    [SerializeField] private float flapFramesPerSecond = 10f;
    
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip flapSound;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float flapTimer;
    private int flapIndex;
    private bool canControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb.gravityScale = 0f; // 👈 agregar esta línea para que empiece parado
    }

    private void Update()
    {
        if (gameManager.State == GameManager.GameState.Playing)
        {
            AnimateBird();
        }

        if (gameManager.State == GameManager.GameState.Playing)
        {
            if (IsFlyInputPressed())
            {
                Flap();
            }

            RotateBird();
        }
    }

    public void OnWaitingToStart()
    {
        canControl = false;
    
        rb.linearVelocity = Vector2.zero;      // 👈 detener movimiento
        rb.angularVelocity = 0f;         // 👈 detener rotación
        rb.gravityScale = 0f;            // 👈 sin gravedad
        rb.simulated = false;            // 👈 🔥 CLAVE: desactiva física totalmente
    
        transform.rotation = Quaternion.identity;
    }

    public void OnGameStarted()
    {
        canControl = true;
    
        rb.simulated = true;   // 👈 volver a activar física
        rb.gravityScale = 4f;
    
        Flap();
    }

    public void OnGameOver()
    {
        canControl = false;
    }

    private void Flap()
    {
        if (!canControl)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(flapSound);
    }

    private void RotateBird()
    {
        float t = Mathf.InverseLerp(-8f, 6f, rb.linearVelocity.y);
        float targetRotation = Mathf.Lerp(maxDownRotation, maxUpRotation, t);
        Quaternion target = Quaternion.Euler(0f, 0f, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, rotationLerpSpeed * Time.deltaTime);
    }

    private void AnimateBird()
    {
        if (flyingSprites == null || flyingSprites.Length == 0)
        {
            return;
        }

        flapTimer += Time.deltaTime * flapFramesPerSecond;

        if (flapTimer >= 1f)
        {
            flapTimer = 0f;
            flapIndex = (flapIndex + 1) % flyingSprites.Length;
            spriteRenderer.sprite = flyingSprites[flapIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameManager.State != GameManager.GameState.Playing)
        {
            return;
        }
        audioSource.PlayOneShot(hitSound);

        gameManager.TriggerGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameManager.State != GameManager.GameState.Playing)
        {
            return;
        }

        if (other.CompareTag("ScoreZone"))
        {
            gameManager.AddScore();
            return;
        }
        audioSource.PlayOneShot(hitSound);
        gameManager.TriggerGameOver();
    }

    private static bool IsFlyInputPressed()
    {
        bool touchPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        return Input.GetMouseButtonDown(0) || touchPressed || Input.GetKeyDown(KeyCode.X);
    }
}