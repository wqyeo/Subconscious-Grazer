using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D)), DisallowMultipleComponent]
public class DisposableBorder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        IDisposableObj disposableObj = collision.gameObject.GetComponent<IDisposableObj>();

        if (collision.gameObject.GetComponent<IDisposableObj>() != null) {
            disposableObj.Dispose();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IDisposableObj disposableObj = collision.gameObject.GetComponent<IDisposableObj>();

        if (collision.gameObject.GetComponent<IDisposableObj>() != null) {
            disposableObj.Dispose();
        }
    }
}
