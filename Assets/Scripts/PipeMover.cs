using UnityEngine;

public class PipeMover : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float destroyX = -12f;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (gameManager.State != GameManager.GameState.Playing)
        {
            return; // 👈 no se mueve si no ha empezado
        }

        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}