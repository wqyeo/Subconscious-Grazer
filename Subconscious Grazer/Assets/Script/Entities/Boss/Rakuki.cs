using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rakuki : Boss {

    [Separator("Rakuki Boss Properties", true)]
    [SerializeField, Tooltip("Firerate for the arrow rain.")]
    private float arrowRainFireRate;

    [SerializeField, Tooltip("Prefab for the arrow rain.")]
    private GameObject arrowRainPrefab;

    private float timer;

    private void Update() {
        if (currentSpell != null) {
            if (currentSpell.SpellCardName == SpellCardName.Arrow_Storm) {
                timer += Time.deltaTime;

                if (timer >= arrowRainFireRate) {
                    FireArrowRain();
                    timer = 0f;
                }
            }
        }
    }

    private void FireArrowRain() {

        // Find a random x position to fire
        float xFirePos = Random.Range(0f, Screen.width);

        // Fetch or create bullet
        GameObject newBullet = ObjPoolManager.Instance.BulletPool.FetchObjByType(BulletType.arrow);
        if (newBullet == null) {
            newBullet = Instantiate(arrowRainPrefab);
            ObjPoolManager.Instance.BulletPool.AddToObjectPool(BulletType.arrow, newBullet);
        }

        newBullet.SetActive(true);

        // Rotate the bullet to the offset.
        var tempRotation = new Vector3();
        tempRotation.z += 180;
        newBullet.transform.eulerAngles = tempRotation;

        newBullet.GetComponent<SpriteRenderer>().sprite = arrowRainPrefab.GetComponent<SpriteRenderer>().sprite;

        newBullet.GetComponent<Bullet>().Initalize(Vector2.zero, -1f, 1, bulletType: BulletType.arrow, rotateBulletToDirection: true, rotationalOffset: 180);
        newBullet.GetComponent<Bullet>().GravityAffected = true;

        newBullet.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(xFirePos, Screen.height + 1f));
    }

}
