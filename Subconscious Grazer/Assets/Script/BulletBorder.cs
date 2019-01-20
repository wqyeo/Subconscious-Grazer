﻿using UnityEngine;

public class BulletBorder : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Bullet>() != null) {
            ObjectPool.Instance.AddToPool(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Bullet>() != null) {
            ObjectPool.Instance.AddToPool(collision.gameObject);
        }
    }
}
