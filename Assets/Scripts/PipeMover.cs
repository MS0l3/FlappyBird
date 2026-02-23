using UnityEngine;

public class PipeMover : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float destroyX = -12f;

    private void Update()
    {
        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
