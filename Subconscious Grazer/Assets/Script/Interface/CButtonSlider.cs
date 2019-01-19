using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CButtonSlider : CButton {

    [SerializeField, Tooltip("The text to display at the front with the value.")]
    private string textDisplay;

    public int SliderValue { get; set; }

    public delegate void OnSliderValueChanged(int value);

    public OnSliderValueChanged _OnSliderValueChanged;

    protected override void Awake() {
        SliderValue = 100;

        _OnSliderValueChanged = delegate {};

        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        // If this button is highlighted
        if (highlighted) {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && SliderValue != 0) {
                AdjustValue(-5);
            } else if (Input.GetKeyDown(KeyCode.RightArrow) && SliderValue != 100) {
                AdjustValue(5);
            }
        }

        base.Update();
    }

    private void AdjustValue(int value) {
        SliderValue += value;

        buttonText.text = textDisplay + SliderValue.ToString();
    }

    public override void SelectButton() {}
}
