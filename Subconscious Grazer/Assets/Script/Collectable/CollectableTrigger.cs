using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent]
public class CollectableTrigger : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        var collectable = other.GetComponent<Collectable>();
        // If this is a collectable.
        if (collectable != null) {
            // Absorb it to the player.
            collectable.AbsorbCollectable();
        }
    }
}
