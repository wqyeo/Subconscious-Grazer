using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeSnapper : MonoBehaviour {

    [System.Serializable]
    private enum UpdateTypes {
        Update,
        FixedUpdate,
        LateUpdate
    }

    [SerializeField, Tooltip("The relative axis this object should snap by.")]
    private bool x, y, z;

    [MustBeAssigned, SerializeField, Tooltip("The transform to snap to.")]
    private Transform targetTransform;

    [SerializeField, Tooltip("The update type to use")]
    private UpdateTypes updateType;

    [SerializeField, Tooltip("How intensively this object will follow to the target object.")]
    private float snapIntensity = 1f;

    private Vector3 lastSnapObjPos;

    private void Start() {
        lastSnapObjPos = targetTransform.position;
    }

    // Update is called once per frame
    void Update () {
        if (updateType == UpdateTypes.Update) {
            UpdateSnapping();
        }
	}

    private void FixedUpdate() {
        if (updateType == UpdateTypes.FixedUpdate) {
            UpdateSnapping();
        }
    }

    private void LateUpdate() {
        if (updateType == UpdateTypes.LateUpdate) {
            UpdateSnapping();
        }
    }

    private void UpdateSnapping() {
        // Determines how much this object should move by.
        Vector3 moveBy = new Vector3(0, 0, 0);

        // If we need to snap to the target object by x axis.
        if (x) {
            // Calculate how much to move by x axis.
            moveBy.x = targetTransform.position.x - lastSnapObjPos.x;
        }

        if (y) {
            moveBy.y = targetTransform.position.y - lastSnapObjPos.y;
        }

        if (z) {
            moveBy.z = targetTransform.position.z - lastSnapObjPos.z;
        }

        lastSnapObjPos = targetTransform.position;
        // Move this object
        transform.position += (moveBy * snapIntensity);
    }
}
