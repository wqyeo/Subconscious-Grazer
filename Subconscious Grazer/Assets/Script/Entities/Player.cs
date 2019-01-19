using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class Player : MonoBehaviour {

    [Header("Player movement properties")]

    [SerializeField, Tooltip("The respective move speed of this player")]
    private Vector2 moveSpeed;

    [SerializeField, Tooltip("The movement multiplier applied when the player does focusing.")]
    private float focusSpeedMultiplier;

    [SerializeField, Tooltip("The sprite renderers for focus indicators to show to player when focused.")]
    private SpriteRenderer[] focusIndicators;

    [Header("Player shooter properties")]

    [SerializeField, Tooltip("The cooldown time before the player can shoot again.")]
    private float shootCooldown;

    [SerializeField, Tooltip("The respective bullet sprites.")]
    private Sprite blueBullet, redBullet;

    [Header("Player shooter properties (Rose Shooters)")]

    [Range(0, 360), SerializeField, Tooltip("The wideness of the shot when focused")]
    private float focusedSpreadWideness;

    [Range(0, 360), SerializeField, Tooltip("The wideness of the shot when not focused")]
    private float defocusedSpreadWideness;

    [SerializeField, Tooltip("The respective bullet sprites.")]
    private Sprite redNeedle, blueNeedle;

    [SerializeField, Tooltip("The respective rose sprite")]
    private Sprite redRose, blueRose;


    private float cooldownTimer;

    // The default shooter player has.
    private LinearShooter[] defaultShooter;
    // The roses following the player (Shoots out needles).
    private SpreadShooter[] needleShooters;

    private Rigidbody2D playerRB;

    private Animator playerAnim;

    private bool isFocused;

    private bool CanShoot {
        get {
            return cooldownTimer >= shootCooldown;
        }
    }

    #region Inspector_Validation

    private void OnValidate() {
        // If the move speed is set to negative in the inspector.
        if (moveSpeed.x < 0) {
            // Set to positive.
            moveSpeed.x = 4;
            // Warn
            Debug.LogWarning("Player.cs :: moveSpeed's value must be positive!");
        }

        if (moveSpeed.y < 0) {
            moveSpeed.y = 3.75f;
            Debug.LogWarning("Player.cs :: moveSpeed's value must be positive!");
        }

        if (shootCooldown < 0) {
            shootCooldown = 0.2f;
            Debug.LogWarning("Player.cs :: shootCooldown's value must be positive!");
        }

        if (focusSpeedMultiplier > 1) {
            focusSpeedMultiplier = 0.5f;
            Debug.LogWarning("Player.cs :: focusSpeedMultiplier must be a value below 1!");
        }
    }

    #endregion

    private void Awake() {
        isFocused = true;
        cooldownTimer = 1f;
    }

    // Use this for initialization
    void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();

        defaultShooter = GetComponentsInChildren<LinearShooter>();
        needleShooters = GetComponentsInChildren<SpreadShooter>();

        SetFocused(false);
    }

    private void Update() {
        HandleShooting(Time.deltaTime);
    }

    void FixedUpdate() {
        HandleMovement(Time.fixedTime);
    }

    private void HandleShooting(float time) {
        // If the player gives an input to shoot and the player can shoot.
        if (Input.GetKey(KeyCode.Z) && CanShoot) {
            Shoot();
        }

        // If the player cant shoot.
        if (!CanShoot) {
            // Increase timer.
            cooldownTimer += time;
        }
    }

    private void HandleMovement(float time) {
        // Determines where this player would move to.
        Vector2 moveDirection = new Vector2(0, 0);

        bool focusState = false;

        #region Horizontal Movement

        // If the user presses the left arrow key.
        if (Input.GetKey(KeyCode.LeftArrow)) {
            // Move to the left.
            moveDirection.x += -1f;
        }
        // If the user presses the right arrow key.
        if (Input.GetKey(KeyCode.RightArrow)) {
            // Move to the right.
            moveDirection.x += 1f;
        }

        #endregion

        #region Verticle Movement

        // If the user presses the up arrow key.
        if (Input.GetKey(KeyCode.UpArrow)) {
            // Move upwards.
            moveDirection.y += 1f;
        }
        // If the user presses the down arrow key.
        if (Input.GetKey(KeyCode.DownArrow)) {
            // Move downwards
            moveDirection.y += -1f;
        }

        #endregion

        #region Diagonal Movement

        // If the player moves diagonally.
        if (moveDirection.x != 0 && moveDirection.y != 0) {
            // Fix the movement (To prevent the player from moving faster diagonally)
            moveDirection.x /= 1.75f;
            moveDirection.y /= 1.75f;
        }

        #endregion

        HandleAnimator(moveDirection.x);

        // Determines the speed the player would move by.
        Vector2 finalSpeed = moveSpeed;

        // If the player decides to focus
        if (Input.GetKey(KeyCode.LeftShift)) {
            // Player is focused and slow movement
            focusState = true;
            finalSpeed *= focusSpeedMultiplier;
        }

        SetFocused(focusState);

        // Move the player respective.
        playerRB.velocity = moveDirection * finalSpeed;
    }

    private void Shoot() {
        // Reset shoot timer
        cooldownTimer = 0f;

        // For each shooter the player has
        foreach (var shooter in defaultShooter) {
            // If the shooter is active
            if (shooter.enabled) {
                // shoot
                shooter.Shoot();
            }
        }

        // For each shooter the player has
        foreach (var shooter in needleShooters) {
            // If the shooter is active
            if (shooter.enabled) {
                // shoot
                shooter.Shoot();
            }
        }
    }

    private void HandleAnimator(float xMovement) {
        // True if the player is moving towards the left.
        bool left = (xMovement < 0) ? true : false;
        // True if the player is moving towards the right.
        bool right = (xMovement > 0) ? true : false;

        playerAnim.SetBool("left", left);
        playerAnim.SetBool("right", right);
    }


    private void SetFocused(bool focusedState) {

        // If the focused state given is the same as the current focus state
        if (isFocused == focusedState) {
            // Exit (Prevent unncessary invoking to line of codes.)
            return;
        }

        isFocused = focusedState;

        foreach (var indicator in focusIndicators) {
            indicator.enabled = isFocused;
        }

        // Use the red sprites if the played changed to focused state.
        Sprite defBulletSprite = isFocused ? redBullet : blueBullet;
        Sprite roseBulletSprite = isFocused ? redNeedle : blueNeedle;
        Sprite roseSprite = isFocused ? redRose : blueRose;

        // Change the spread shot wideness when focused.
        float spreadWidness = isFocused ? focusedSpreadWideness : defocusedSpreadWideness;

        // Set the respective default sprite for the shooters.
        foreach (var defShooter in defaultShooter) {
            defShooter.BulletDefaultSprites = defBulletSprite;
        }

        foreach (var needleShooter in needleShooters) {
            needleShooter.BulletDefaultSprites = roseBulletSprite;

            needleShooter.gameObject.GetComponent<SpriteRenderer>().sprite = roseSprite;

            needleShooter.ShotWideness = spreadWidness;
        }
    }
}
