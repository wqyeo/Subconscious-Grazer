using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionParticleCallback : MonoBehaviour {
    [SerializeField, Tooltip("The boss controlling this particle system")]
    private Boss boss;

    public void OnParticleSystemStopped() {
        boss.TransitionToNextSpell();
    }
}
