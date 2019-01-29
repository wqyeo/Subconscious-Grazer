using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic)), DisallowMultipleComponent]
public class FadeAround : MonoBehaviour {
    [SerializeField, Tooltip("Fade around these two values.")]
    private float minFadeValue, maxFadeValue;

    [SerializeField, Tooltip("How fast to fade this object.")]
    private float fadeSpeed;

    [SerializeField, Tooltip("Start fading at the start?")]
    private bool fadeOnStart = true;

    private bool goingMax;

    private float currProgress;

    public bool IsActive { get; set; }

    private void Awake() {
        IsActive = fadeOnStart;
        goingMax = false;
        currProgress = 0;
    }

    private void OnValidate() {
        // If minFadeValue is set to a higher value than maxFadeValue in the inspector.
        if (maxFadeValue < minFadeValue) {
            // Swap around the values.
            var temp = minFadeValue;
            minFadeValue = maxFadeValue;
            maxFadeValue = temp;
        }

        // If the fade speed is set to negative.
        if (fadeSpeed < 0) {
            // Set it back to positive.
            fadeSpeed = Mathf.Abs(fadeSpeed);
        }
    }

    // Update is called once per frame
    void Update() {

        if (!IsActive) { return; }
        // If the progress is not done yet.
        if (currProgress < 1) {
            float from, to;
            GetFadingFromAndTo(out from, out to);

            FadeGraphicByProgress(from, to, currProgress);

            // Update the current fade progress.
            currProgress += Time.deltaTime * fadeSpeed;
        } else {
            // Reset progress and fade the other direction.
            currProgress = 0;
            goingMax = !goingMax;
        }
    }

    private void FadeGraphicByProgress(float from, float to, float progress) {
        var temp = GetComponent<Graphic>().color;
        temp.a = Mathf.Lerp(from, to, progress);
        GetComponent<Graphic>().color = temp;
    }

    private void GetFadingFromAndTo(out float from, out float to) {
        from = goingMax ? maxFadeValue : minFadeValue;
        to = goingMax ? minFadeValue : maxFadeValue;
    }

}
