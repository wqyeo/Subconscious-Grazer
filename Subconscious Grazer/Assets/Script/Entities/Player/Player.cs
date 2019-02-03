using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator)), DisallowMultipleComponent]
public class Player : Singleton<Player> {

    [SerializeField, Tooltip("True if this player is invicible at the start.")]
    private bool Invulnerable;

    [MustBeAssigned, Separator("Player Input Keycodes", true)]

    [SerializeField, SearchableEnum, Tooltip("The respective keycodes for the player's input.")]
    private KeyCode shootKey;

    [MustBeAssigned, SerializeField, SearchableEnum, Tooltip("Keycode for screenshot capture")]
    private KeyCode screenshotKey;

    [MustBeAssigned, SerializeField, SearchableEnum, Tooltip("The respective keycodes for the player's input.")]
    private KeyCode focusKey, moveUpKey, moveDownKey, moveRightKey, moveLeftKey;

    [Separator("Player movement properties", true)]

    [SerializeField, Tooltip("The respective move speed of this player")]
    private Vector2 moveSpeed;

    [SerializeField, Tooltip("The movement multiplier applied when the player does focusing.")]
    private float focusSpeedMultiplier;

    [SerializeField, Tooltip("The sprite renderers for focus indicators to show to player when focused.")]
    private SpriteRenderer[] focusIndicators;

    [Separator("Player shooter properties", true)]

    [SerializeField, Tooltip("The cooldown time before the player can shoot again.")]
    private float shootCooldown;

    [MustBeAssigned, SerializeField, Tooltip("The respective bullet sprites.")]
    private Sprite blueBullet, redBullet;

    [Separator("Player shooter properties (Rose Shooters)", true)]

    [Range(0, 360), SerializeField, Tooltip("The wideness of the shot when focused")]
    private float focusedSpreadWideness;

    [Range(0, 360), SerializeField, Tooltip("The wideness of the shot when not focused")]
    private float defocusedSpreadWideness;

    [MustBeAssigned, SerializeField, Tooltip("The respective bullet sprites.")]
    private Sprite redNeedle, blueNeedle;

    [MustBeAssigned, SerializeField, Tooltip("The respective rose sprite")]
    private Sprite redRose, blueRose;

    [SerializeField, Tooltip("How far the rose shooter should be away from the player.")]
    private float shooterDistance;

    private float cooldownTimer;

    // The default shooter player has.
    private LinearShooter[] defaultShooter;
    // The roses following the player (Shoots out needles).
    private SpreadShooter[] needleShooters;

    private Rigidbody2D playerRB;

    private Animator playerAnim;

    private bool isFocused;

    private int currPowerPoint;

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
    void Start() {
        currPowerPoint = 0;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();

        defaultShooter = GetComponentsInChildren<LinearShooter>(true);
        needleShooters = GetComponentsInChildren<SpreadShooter>(true);

        SetFocused(false);
    }

    private void Update() {
        HandleShooting(Time.deltaTime);

        if (Input.GetKeyDown(screenshotKey)) {
            TakeScreenshot();
        }
    }

    void FixedUpdate() {
        HandleMovement(Time.fixedTime);
    }

    private void TakeScreenshot() {
        int screenShotNum = PlayerPrefs.GetInt("ScreenShotNum", 0);
        ScreenCapture.CaptureScreenshot("screenshots/Screenshot_" + screenShotNum.ToString());
        screenShotNum += 1;
        PlayerPrefs.SetInt("ScreenShotNum", screenShotNum);
    }

    #region Input_Handling

    private void HandleShooting(float time) {
        // If the player gives an input to shoot and the player can shoot.
        if (Input.GetKey(shootKey) && CanShoot) {
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

        #region Movement_Input

        // If the user presses the left arrow key.
        if (Input.GetKey(moveLeftKey)) {
            // Move to the left.
            moveDirection.x += -1f;
        }
        // If the user presses the right arrow key.
        if (Input.GetKey(moveRightKey)) {
            // Move to the right.
            moveDirection.x += 1f;
        }

        // If the user presses the up arrow key.
        if (Input.GetKey(moveUpKey)) {
            // Move upwards.
            moveDirection.y += 1f;
        }
        // If the user presses the down arrow key.
        if (Input.GetKey(moveDownKey)) {
            // Move downwards
            moveDirection.y += -1f;
        }

        #endregion

        HandleAnimator(moveDirection.x);

        // Determines the speed the player would move by.
        Vector2 finalSpeed = moveSpeed;

        // If the player decides to focus
        if (Input.GetKey(focusKey)) {
            // Player is focused and slow movement
            focusState = true;
            finalSpeed *= focusSpeedMultiplier;
        }

        SetFocused(focusState);

        // Move the player respective.
        playerRB.velocity = moveDirection.normalized * finalSpeed;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D other) {
        // If the player touched the enemy's bullet.
        if (other.CompareTag("EnemyBullet")) {
            HandleBulletCollision(other.gameObject.GetComponent<Bullet>());
        } else if (other.CompareTag("Collectable")) {
            HandleCollectableCollision(other.gameObject.GetComponent<Item>());
        }
    }

    private void HandleBulletCollision(Bullet collidedBullet) {
        if (Invulnerable) { return; }

        StartCoroutine(ShowPlayerHitAnimation());
        GameManager.Instance.PenalizePlayer();

        collidedBullet.Dispose();
    }

    private void HandleCollectableCollision(Item collidedCollectable) {
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.PlayerItemCollect);
        collidedCollectable.CollectItem();
    }

    private void Shoot() {
        // Reset shoot timer
        cooldownTimer = 0f;

        AudioManager.Instance.PlayAudioClipIfExists(AudioType.PlayerShoot);

        ShootAllActiveDefaultShooters();

        ShootAllActiveNeedleShooters();
    }

    private void ShootAllActiveDefaultShooters() {
        foreach (var shooter in defaultShooter) {
            if (shooter.IsActive) {
                shooter.Shoot();
            }
        }
    }

    private void ShootAllActiveNeedleShooters() {
        foreach (var shooter in needleShooters) {
            if (shooter.IsActive) {
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

    #region Focused_State_Handling

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

        HandleSpriteState();
        HandleRosePosition();
    }

    /// <summary>
    /// Handle the position of the needle shooters.
    /// </summary>
    private void HandleRosePosition() {
        // Represents the number of active needle shooter.
        int activeShooterCount = 0;

        // Go through all needle shooters.
        foreach (var needleShooter in needleShooters) {
            // If this needle shooter is active.
            if (needleShooter.IsActive) {
                // Add to active count.
                activeShooterCount += 1;
            }
        }
        // Exit function if there are no active shooter. (Handling position of needle shooters is not needed.)
        if (activeShooterCount <= 0) { return; }

        // Determines which angle from the player the shooter will be at.
        float angle;
        // Determine how much to rotate the angle by (relative to the player) before placing the next shooter.
        float angleStep;

        GetAngleToPlaceShooter(activeShooterCount, out angle, out angleStep);
        angle = angle.GetNormalizedAngle();
        angleStep = angleStep.GetNormalizedAngle();

        // Where to start placing the shooter before rotating.
        Vector2 startPos = new Vector2(0, shooterDistance);

        foreach (var shooter in needleShooters) {
            // If this needle shooter is active
            if (shooter.IsActive) {
                // Set the needle position.
                shooter.gameObject.transform.localPosition = startPos;
                // Rotate the shooter around the player with the given angle.
                shooter.gameObject.transform.RotateAround(transform.position, Vector3.forward, angle);

                // If we are not focused
                if (!isFocused) {
                    // Rotate the shot angle too.
                    shooter.ShotAngle = -angle;
                } else {
                    // Shoot front.
                    shooter.ShotAngle = 0f;
                }

                // Rotate respectively.
                angle += angleStep;
            }
        }
    }

    private void GetAngleToPlaceShooter(int activeShooterCount, out float angle, out float angleStep) {
        angle = 0f;
        angleStep = 0f;
        if (isFocused) {
            float offSet;
            switch (activeShooterCount) {
                // If there are 4 shooters.
                case 4:
                    offSet = 130f / 2f;
                    angleStep = (130f / (activeShooterCount + 1));
                    angle = (angleStep - offSet);
                    break;
                case 3:
                    angle = -30f;
                    angleStep = 30f;
                    break;
                case 2:
                    offSet = 130f / 2f;
                    angleStep = (130f / (activeShooterCount + 1));
                    angle = (angleStep - offSet);
                    break;
                default:
                    break;
            }
        } else {
            switch (activeShooterCount) {
                // If there are 4 shooters.
                case 4:
                    // Place at top-left, top-right, bottom-left, bottom-right
                    angle = -45f;
                    angleStep = 360f / activeShooterCount;
                    break;
                case 3:
                    // Triangle formation, the first point being at the top of the player.
                    angleStep = 360f / activeShooterCount;
                    break;
                case 2:
                    // Somewhere near top-left and top-right.
                    angle = -45f;
                    angleStep = 90f;
                    break;
                // If there is only 1 shooter.
                default:
                    // Use the initalized default.
                    break;
            }
        }
    }

    private void HandleSpriteState() {
        // Use the red sprites if the played changed to focused state.
        Sprite defBulletSprite = isFocused ? redBullet : blueBullet;
        Sprite roseBulletSprite = isFocused ? redNeedle : blueNeedle;
        Sprite roseSprite = isFocused ? redRose : blueRose;

        // Change the spread shot wideness when focused.
        float spreadWidness = isFocused ? focusedSpreadWideness : defocusedSpreadWideness;

        // Set the respective default sprite for the shooters.
        foreach (var defShooter in defaultShooter) {
            defShooter.BulletDefaultSprite = defBulletSprite;
        }

        foreach (var needleShooter in needleShooters) {
            needleShooter.BulletDefaultSprite = roseBulletSprite;

            needleShooter.gameObject.GetComponent<SpriteRenderer>().sprite = roseSprite;

            needleShooter.ShotWideness = spreadWidness;
        }
    }

    #endregion

    #region Power_Point_Handling

    public void UpdatePowerPoints() {
        int gamePowerPoint = Mathf.FloorToInt(GameManager.Instance.PowerPoints);
        // Update the player's power point if needed.
        if (currPowerPoint != gamePowerPoint) {
            AudioManager.Instance.PlayAudioClipIfExists(AudioType.PlayerPowerUp);
            currPowerPoint = gamePowerPoint;
            UpdateNeedleShootersByPower(currPowerPoint);
            UpdateDefaultShootersByPower(currPowerPoint);
        }
    }

    private void UpdateDefaultShootersByPower(int powerPoint) {
        foreach (var shooter in defaultShooter) {
            // Reduce the default shooter damage as the player gets more power.
            // (Needles will take over the damage)
            shooter.Damage = Mathf.Clamp((8 / powerPoint), 2, 6);
        }
    }

    private void UpdateNeedleShootersByPower(int powerPoint) {
        foreach (var shooter in needleShooters) {
            // Activate this shooter if we have enough power to.
            shooter.IsActive = (powerPoint > 0);
            UpdateNeedleShooterSprite(shooter);
            --powerPoint;
        }
        HandleRosePosition();
    }

    private void UpdateNeedleShooterSprite(BaseShooter needleShooter) {
        needleShooter.gameObject.GetComponent<SpriteRenderer>().enabled = needleShooter.IsActive;
    }

    #endregion

    private IEnumerator ShowPlayerHitAnimation() {

        Invulnerable = true;

        float timeTaken = 0f;

        Color32 temp = GetComponent<SpriteRenderer>().color;

        while (timeTaken < 1.25f) {

            // Change the color of the sprite from red back to normal, vice versa.
            temp.g = (byte)((temp.g == 255) ? 0 : 255);
            temp.b = (byte)((temp.b == 255) ? 0 : 255);

            GetComponent<SpriteRenderer>().color = temp;

            yield return new WaitForSeconds(0.125f);
            timeTaken += 0.125f;
        }

        // Change the color back to normal at the end.
        temp.g = 255;
        temp.b = 255;

        GetComponent<SpriteRenderer>().color = temp;

        Invulnerable = false;

        yield return null;
    }
}
