using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public int Points { get; private set; }
    public float PowerPoints { get; private set; }

    public void AddPoints(int amtToAdd) {
        Points += amtToAdd;
    }

    public void AddPowerPoints(float amtToAdd) {
        PowerPoints += amtToAdd;

        if (PowerPoints > 4) {
            PowerPoints = 4;
        }

        Player.Instance.UpdatePowerPoints();
    }
}
