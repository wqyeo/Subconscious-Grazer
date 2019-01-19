using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Text))]
public class CButton : MonoBehaviour {

    [SerializeField, Tooltip("Which event will be invoked when this button is selected.")]
    private UnityEvent onButtonSelected;

    [SerializeField, Tooltip("Which event will be invoked when this button is highlighted/un-highlighted.")]
    private UnityEvent onButtonHighlighted, onButtonDehighlighted;

    protected Text buttonText;

    private float fadeTimer;
    private float alphaColor;

    private bool fadingOut;

    private bool shakeTxt;
    private float shakeTimer;

    private Vector2 originalPos;
    private Color originalColor;

    protected bool highlighted;

    protected virtual void Awake() {
        if (onButtonHighlighted == null) {
            onButtonHighlighted = new UnityEvent();
        }

        if (onButtonDehighlighted == null) {
            onButtonDehighlighted = new UnityEvent();
        }
    }

    // Use this for initialization
    protected virtual void Start () {
        buttonText = GetComponent<Text>();

        originalColor = buttonText.color;

        alphaColor = 1f;
        fadeTimer = 0f;
        shakeTimer = 0f;
        shakeTxt = false;
        originalPos = transform.position;
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        HandleTextShake(Time.unscaledDeltaTime);

        HandleTextFading(Time.unscaledDeltaTime);
    }

    private void HandleTextFading(float time) {
        // If this text is highlighted.
        if (highlighted) {
            // Increment the fade timer.
            fadeTimer += time;
            // If the fade timer is up
            if (fadeTimer > 0.065f) {
                // Reset timer.
                fadeTimer = 0f;
                // Check if the text should be fading out or in, set alpha values respectively.
                alphaColor += fadingOut ? -0.03f : 0.03f;
                var temp = buttonText.color;
                temp.a = alphaColor;
                buttonText.color = temp;

                // If the text has reached a certain alpha color value.
                if ((temp.a > 0.9f && !fadingOut)) {
                    // Start fading out.
                    fadingOut = true;
                } else if (temp.a < 0.6f && fadingOut) {
                    fadingOut = false;
                }
            }
        }
    }

    private void HandleTextShake(float time) {
        // If the timer for this text to stop shaking is up.
        if (shakeTimer < 0f && shakeTxt) {
            // Reset timer.
            shakeTimer = 0.15f;
            // Stop shaking this text.
            shakeTxt = false;
            // Place this text back into the original position.
            transform.position = originalPos;
        } else {
            // Count down the timer.
            shakeTimer -= time;
        }

        // If we need to shake this text.
        if (shakeTxt) {
            // Make this text around.
            transform.position += transform.position * Mathf.Sin(time * (Random.Range(-1f, 1f) > 0f ? 1.5f : -1.5f)) * shakeTimer;
        }
    }

    /// <summary>
    /// Highlight this button. (Hover over effect)
    /// </summary>
    public void HighlightButton() {
        originalPos = transform.position;
        alphaColor = 1f;
        highlighted = true;
        fadingOut = true;
        buttonText.fontStyle = FontStyle.Bold;
        shakeTxt = true;
        shakeTimer = 0.175f;
        buttonText.color = GlobalProperties.HighlightedButtonColor;

        onButtonHighlighted.Invoke();
    }

    /// <summary>
    /// Un-highlights this button.
    /// </summary>
    public void DehighlightButton() {
        highlighted = false;
        buttonText.fontStyle = FontStyle.Normal;
        buttonText.color = originalColor;

        onButtonDehighlighted.Invoke();
    }

    /// <summary>
    /// Invokes the event related to this button.
    /// </summary>
    public virtual void SelectButton() {
        // Return this button to it's normal state
        highlighted = false;
        buttonText.fontStyle = FontStyle.Normal;
        buttonText.color = originalColor;

        onButtonSelected.Invoke();
    }
}
