using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D)), DisallowMultipleComponent]
public class EnemyBorder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            ObjectPool.Instance.AddToPool(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            ObjectPool.Instance.AddToPool(collision.gameObject);
        }
    }
}
