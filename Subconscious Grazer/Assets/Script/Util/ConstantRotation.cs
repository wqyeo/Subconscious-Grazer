using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ConstantRotation : MonoBehaviour {

    [SerializeField, Tooltip("The speed which this object rotates at.")]
    private float rotationSpeed;

	void Update () {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
