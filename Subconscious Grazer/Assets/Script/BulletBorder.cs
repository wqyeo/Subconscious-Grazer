using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBorder : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Bullet>()) {
            ObjectPool.Instance.AddToPool(collision.gameObject);
        }
    }
}
