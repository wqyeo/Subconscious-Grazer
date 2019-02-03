using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D)), DisallowMultipleComponent]
public class PlayerGrazeRegion : MonoBehaviour
{
    [Separator("Player graze properties", true)]

    [SerializeField, Tooltip("Points to give when the player grazes a bullet.")]
    private int pointReward;

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("EnemyBullet")) {
            HandleBulletGraze(other.gameObject.GetComponent<Bullet>());
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("EnemyBullet")) {
            HandleBulletGraze(other.gameObject.GetComponent<Bullet>());
        }
    }

    private void HandleBulletGraze(Bullet grazedBullet) {
        // Do nothing if bullet was already grazed
        if (grazedBullet.Grazed) { return; }

        grazedBullet.Grazed = false;
        GameManager.Instance.AddPoints(pointReward);
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.PlayerGraze);
    }
}
