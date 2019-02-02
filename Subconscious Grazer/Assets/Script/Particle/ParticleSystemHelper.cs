using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemHelper : MonoBehaviour {
    public delegate void ParticleSystemStoppedDelegate();

    public ParticleSystemStoppedDelegate onParticleSystemStopped;

    public void PlayParticleSystem() {
        GetComponent<ParticleSystem>().Play();
    }

    public void OnParticleSystemStopped() {
        if (onParticleSystemStopped != null) {
            onParticleSystemStopped();
        }
    }
}
