using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

    [MustBeAssigned, SerializeField]
    private Image scareFlashPanel;

    [SerializeField, Tooltip("Text to show the current points.")]
    private Text playerPointText;

    public int Points { get; private set; }
    public float PowerPoints { get; private set; }

    public void AddPoints(int amtToAdd) {
        Points += amtToAdd;

        if (Points < 0) {
            Points = 0;
        }

        playerPointText.text = "Score: " + Points;
    }

    public void AddPowerPoints(float amtToAdd) {
        PowerPoints += amtToAdd;

        PowerPoints = Mathf.Clamp(PowerPoints, 0, 4);

        Player.Instance.UpdatePowerPoints();
    }

    public void SetScareFlashPanelAlphaColor(byte alphaValue32) {
        Color32 temp = scareFlashPanel.color;
        temp.a = alphaValue32;
        scareFlashPanel.color = temp;
    }

    public void PenalizePlayer() {
        AddPoints(-100);
        AddPowerPoints(-0.1f);
    }
}
