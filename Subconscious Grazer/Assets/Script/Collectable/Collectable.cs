using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour {

    [Separator("Base Collectable properties", true)]

    [SearchableEnum, SerializeField, Tooltip("The type of collectable.")]
    private CollectableType typeOfCollectable;

    [SerializeField, Tooltip("The speed of this collectable when absorbed by the player.")]
    private float absorbingSpeed;

    [SerializeField, Tooltip("The speed this collectable falls at.")]
    private float fallSpeed;

    [SerializeField, Tooltip("The amount of score this collectable rewards.")]
    private int scoreReward;

    [SerializeField, Tooltip("The amount of power this collectable rewards.")]
    private float powerReward;

    [SerializeField, Tooltip("Falling on start")]
    private bool fallingOnStart;

    [ConditionalField("fallingOnStart", false), SerializeField, Tooltip("The delay before this collectable gets absorbed by the player")]
    private float absorbDelay;

    public delegate void CollectedDelegate();

    public CollectedDelegate OnCollectableCollected;

    #region Properties

    public bool IsFalling { get; set; }

    public CollectableType TypeOfCollectable {
        get {
            return typeOfCollectable;
        }
    }

    #endregion

    private void Start() {
        IsFalling = true;

        if (!fallingOnStart) {
            StartCoroutine(AbsorbDelay());
        }
    }

    private void Update() {
        if (IsFalling) {
            HandleFalling(Time.deltaTime);
        } else {
            SeekToPlayer(Time.deltaTime);
        }
    }

    private IEnumerator AbsorbDelay() {
        yield return new WaitForSeconds(absorbDelay);
        IsFalling = false;
        yield return null;
    }

    public void AbsorbCollectable() {
        IsFalling = false;
    }

    public void CollectCollectable() {
        if (OnCollectableCollected != null) {
            OnCollectableCollected();
        }

        GameManager.Instance.AddPoints(scoreReward);
        GameManager.Instance.AddPowerPoints(powerReward);

        gameObject.SetActive(false);
    }

    private void HandleFalling(float deltaTime) {
        transform.position += Vector3.down * (fallSpeed * deltaTime);
    }

    private void SeekToPlayer(float deltaTime) {
        var playerPos = Player.Instance.transform.position;
        transform.position += transform.position.DirectionToVector(playerPos) * (absorbingSpeed * deltaTime);
    }
}
