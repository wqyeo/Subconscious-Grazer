using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour, IDisposableObj {

    public event EventHandler OnObjectDisposedEvent;

    [Separator("Base Item properties", true)]

    [SearchableEnum, SerializeField, Tooltip("The type of item.")]
    private ItemType typeOfItem;

    [SerializeField, Tooltip("The speed of this item when absorbed by the player.")]
    private float absorbingSpeed;

    [SerializeField, Tooltip("The speed this item falls at.")]
    private float fallSpeed;

    [SerializeField, Tooltip("The amount of score this item rewards.")]
    private int scoreReward;

    [SerializeField, Tooltip("The amount of power this item rewards.")]
    private float powerReward;

    [SerializeField, Tooltip("Falling on start")]
    private bool fallingOnStart;

    [ConditionalField("fallingOnStart", false), SerializeField, Tooltip("The delay before this item gets absorbed by the player")]
    private float absorbDelay;

    #region Properties

    public bool IsFalling { get; set; }

    public ItemType TypeOfItem {
        get {
            return typeOfItem;
        }
    }

    #endregion

    private void Update() {
        if (IsFalling) {
            HandleFalling(Time.deltaTime);
        } else {
            SeekToPlayer(Time.deltaTime);
        }
    }

    public void InitalizeItem() {
        IsFalling = true;

        if (!fallingOnStart) {
            StartCoroutine(WaitBeforeSeekToPlayer());
        }
    }

    private IEnumerator WaitBeforeSeekToPlayer() {
        yield return new WaitForSeconds(absorbDelay);
        SeekToPlayer();
        yield return null;
    }

    public void SeekToPlayer() {
        IsFalling = false;
    }

    public void CollectItem() {

        UpdatePointsToGame();

        Dispose();
    }

    private void HandleFalling(float deltaTime) {
        transform.position += Vector3.down * (fallSpeed * deltaTime);
    }

    private void SeekToPlayer(float deltaTime) {
        var playerPos = Player.Instance.transform.position;
        transform.position += transform.position.DirectionToVector(playerPos) * (absorbingSpeed * deltaTime);
    }

    private void UpdatePointsToGame() {
        GameManager.Instance.AddPoints(scoreReward);
        GameManager.Instance.AddPowerPoints(powerReward);
    }

    public void Dispose() {

        if (OnObjectDisposedEvent != null) {
            OnObjectDisposedEvent(this, null);
        }

        gameObject.SetActive(false);
    }
}
