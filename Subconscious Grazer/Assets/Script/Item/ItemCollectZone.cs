using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent]
public class ItemCollectZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        var collectable = other.GetComponent<Item>();
        // If this is a collectable.
        if (collectable != null) {
            collectable.SeekToPlayer();
        }
    }
}
