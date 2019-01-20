using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderSetter : MonoBehaviour {
    [MustBeAssigned, SerializeField, Tooltip("The top border")]
    private GameObject topBorder;

    [MustBeAssigned, SerializeField, Tooltip("The bottom border")]
    private GameObject bottomBorder;

    [MustBeAssigned, SerializeField, Tooltip("The left border")]
    private GameObject leftBorder;

    [MustBeAssigned, SerializeField, Tooltip("The right border")]
    private GameObject rightBorder;

    [SerializeField, Tooltip("Offset for the heigth and width of the border respectively.")]
    private float xOffset, yOffset;

    // Use this for initialization
    void Start() {
        SetBorderPos();
    }

    private void SetBorderPos() {
        // Get the top middle position of the camera.
        Vector2 temp = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height));
        // Find the offset (border size) of the border.
        var borderOffset = topBorder.GetComponent<SpriteRenderer>().bounds.size.y;
        // Move the top left border to the top-middle position of the camera, adjusting with the offset.
        topBorder.transform.position = (temp + new Vector2(0, borderOffset / 2f + yOffset));

        temp = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, 0));
        borderOffset = bottomBorder.GetComponent<SpriteRenderer>().bounds.size.y;
        bottomBorder.transform.position = (temp - new Vector2(0, borderOffset / 2f + yOffset));

        temp = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2f));
        borderOffset = leftBorder.GetComponent<SpriteRenderer>().bounds.size.x;
        leftBorder.transform.position = (temp - new Vector2(borderOffset / 2f + xOffset, 0));

        temp = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height / 2f));
        borderOffset = rightBorder.GetComponent<SpriteRenderer>().bounds.size.x;
        rightBorder.transform.position = (temp + new Vector2(borderOffset / 2f + xOffset, 0));
    }
}
