using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System;

public class GameManager : Singleton<GameManager> {

    [MustBeAssigned, SerializeField]
    private Image scareFlashPanel;

    [SerializeField, Tooltip("Text to show the current points.")]
    private Text playerPointText;

    [SerializeField, Tooltip("The text warning the player of a spell card.")]
    private Text spellCardWarnText;

    [SerializeField, Tooltip("The text to show the current spell card being invoked.")]
    private Text currentSpellCardNameText;

    [SerializeField, Tooltip("The image filler for the boss's health")]
    private Image bossHealthBar;

    [SerializeField, Tooltip("Countdown text for the spell cards.")]
    private Text spellCardCountDownText;

    private Action onSpellCountDownEndCallback;

    private bool countingDown;
    private int currentCountDown;

    private float secondsCounter;

    public int Points { get; private set; }
    public float PowerPoints { get; private set; }

    private void Update() {
        if (countingDown) {
            UpdateCountDown(Time.unscaledDeltaTime);
        }
    }

    private void DecrementCountDown() {
        --currentCountDown;

        spellCardCountDownText.text = currentCountDown.ToString();

        // If the count down is finished
        if (currentCountDown <= 0) {
            FinishCountDown();
        } else if (currentCountDown <= 5) {
            // Play audio to warn the player that the spell card is ending.
            AudioManager.Instance.PlayAudioClipIfExists(AudioType.TimeOut);
        }
    }

    private void UpdateCountDown(float deltaTime) {
        secondsCounter += deltaTime;
        // If 1 second has passed.
        if (secondsCounter >= 1) {
            // Update the count down.
            DecrementCountDown();
            secondsCounter = 0f;
        }
    }

    private void FinishCountDown() {
        // Stop counting down
        countingDown = false;

        if (onSpellCountDownEndCallback != null) {
            onSpellCountDownEndCallback();
        }
    }

    public void AddPoints(int amtToAdd) {
        Points += amtToAdd;

        if (Points < 0) {
            Points = 0;
        }

        playerPointText.text = "Score: " + Points;
    }

    public void AddPowerPoints(float amtToAdd) {
        PowerPoints += amtToAdd;
        // The player can only benefit from 4 whole power points.
        PowerPoints = Mathf.Clamp(PowerPoints, 0f, 4.1f);

        Player.Instance.UpdatePowerPoints();
    }

    public void SetScareFlashPanelAlphaColor(byte alphaValue32) {
        // Used for one of the spell cards for the boss only.
        Color32 temp = scareFlashPanel.color;
        temp.a = alphaValue32;
        scareFlashPanel.color = temp;
    }

    public void PenalizePlayer() {
        /// TODO: CODE REFACTOR
        AddPoints(-200);
        AddPowerPoints(-0.1f);
    }

    public void HideSpellCardWarningAndStopCountDown() {
        HideOrShowSpellCardInterface(false);
        countingDown = false;
        onSpellCountDownEndCallback = null;
    }

    public void WarnSpellCard(SpellCardName spellCardName) {
        HideOrShowSpellCardInterface(true);
        currentSpellCardNameText.text = GetSpellCardNameAsStringWithoutSpaces(spellCardName);
    }

    /// <summary>
    /// Start the spellcard count-down
    /// </summary>
    /// <param name="duration">How long to countdown for (seconds)</param>
    /// <param name="callback">Callback when countdown is finished</param>
    public void CountDownSpellCard(int duration, Action callback = null) {
        currentCountDown = duration;
        onSpellCountDownEndCallback = callback;
        countingDown = true;

        spellCardCountDownText.text = currentCountDown.ToString();
    }

    private void HideOrShowSpellCardInterface(bool show) {
        spellCardWarnText.gameObject.SetActive(show);
        currentSpellCardNameText.gameObject.SetActive(show);
        spellCardCountDownText.gameObject.SetActive(show);
        bossHealthBar.gameObject.SetActive(show);
    }

    private string GetSpellCardNameAsStringWithoutSpaces(SpellCardName spellCardName) {
        return spellCardName.ToString().Replace("_", " ");
    }

    public void FillHealthBar() {
        StartCoroutine(ProgressivelyFillHealthBar());
    }

    public void SetHealthBarValue(float value) {
        bossHealthBar.fillAmount = Mathf.Clamp01(value);
    }

    private IEnumerator ProgressivelyFillHealthBar() {
        float progress = 0f;

        while (progress < 1f) {
            bossHealthBar.fillAmount = Mathf.Clamp01(progress);

            progress += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
