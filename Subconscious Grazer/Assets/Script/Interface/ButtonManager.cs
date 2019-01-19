using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

    private enum Orientation {
        Horizontal,
        Vertical
    }

    [SerializeField, Tooltip("The array of buttons this button manager manages.")]
    private CButton[] buttons;

    [SerializeField, Tooltip("How these buttons are arranged (To determine the input to highlight another button.)")]
    private Orientation btnOrientation;

    // Determine the button used to go between each buttons.
    private KeyCode prevKeyCode, nextKeyCode;

    // Which button the user is currently targetting.
    private int highlightedButtonIndex;

    private void Awake() {
        highlightedButtonIndex = 0;

        // If the button are orientated horizontally.
        if (btnOrientation == Orientation.Horizontal) {
            // Left and right arrow keys to navigate around.
            prevKeyCode = KeyCode.LeftArrow;
            nextKeyCode = KeyCode.RightArrow;
        } else {
            prevKeyCode = KeyCode.UpArrow;
            nextKeyCode = KeyCode.DownArrow;
        }
    }

    // Use this for initialization
    void Start () {
        // Highlights the first button.
        buttons[0].HighlightButton();
    }
	
	// Update is called once per frame
	void Update () {
        // If the user cycles through the next button.
        if (Input.GetKeyDown(nextKeyCode)) {
            CycleButtons(true);
        } else if (Input.GetKeyDown(prevKeyCode)) {
            CycleButtons(false);
        } 
        // If the user presses the select key.
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Z)) {
            // Select the currently highlighted button.
            buttons[highlightedButtonIndex].SelectButton();
        }
	}

    private void CycleButtons(bool next = true) {
        // Un-highlights the current button
        buttons[highlightedButtonIndex].DehighlightButton();

        // If this is the last button but the user requests for the next button.
        if (buttons.Length <= highlightedButtonIndex + 1 && next) {
            // Reset back to highlight the first button.
            highlightedButtonIndex = 0;
        }
        // If the user wants the previous button but is targetting the first button.
        else if (highlightedButtonIndex <= 0 && !next) {
            // Reset to the final button.
            highlightedButtonIndex = buttons.Length - 1;
        } else {
            // Go to the next or previous button respectively.
            int temp = next ? 1 : -1;
            highlightedButtonIndex += temp;
        }
        // Highlights the new button
        buttons[highlightedButtonIndex].HighlightButton();

    }
}
