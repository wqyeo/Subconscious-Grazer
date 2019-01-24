using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBorder : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            DisableEnemyIfExiting(collision.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            DisableEnemyIfExiting(collision.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            HandleEnemyEnteringField(collision.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Enemy>() != null) {
            HandleEnemyEnteringField(collision.gameObject.GetComponent<Enemy>());
        }
    }

    private void HandleEnemyEnteringField(Enemy enemy) {
        if (!enemy.JustSpawned) { return; }

        enemy.JustSpawned = false;
        enemy.Invulnerable = false;

        Debug.Log("Entering Field!");
    }

    private void DisableEnemyIfExiting(Enemy enemy) {
        if (enemy.JustSpawned) { return; }

        Debug.Log("Exiting Field!");


        enemy.CanAct = false;
        if (enemy.ControllingAI.TypeOfAI == AIType.TeleportingAI) {
            enemy.gameObject.SetActive(false);
        }
    }
}
