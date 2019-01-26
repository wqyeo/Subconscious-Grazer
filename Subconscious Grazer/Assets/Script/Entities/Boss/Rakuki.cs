using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rakuki : Boss {

    [Separator("Rakuki Boss Properties", true)]
    [SerializeField, Tooltip("Firerate for the arrow rain.")]
    private float arrowRainFireRate;

    [SerializeField, Tooltip("The max number of arrow rain to produce at once")]
    private int maxArrowRainCount;

    [SerializeField, Tooltip("Prefab for the arrow rain.")]
    private GameObject arrowRainPrefab;

    private float timer;

    private Vector2 screenTopPos;

    private void Start() {
        screenTopPos = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height));
    }

    private void Update() {
        if (currentSpell != null) {
            if (currentSpell.SpellCardName == SpellCardName.Arrow_Storm) {
                timer += Time.deltaTime;

                int arrowToFire = Random.Range(0, maxArrowRainCount);

                while (arrowToFire > 0) {

                }
            }
        }
    }

}
