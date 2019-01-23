using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShooter : TargettingShooter {

    public override void Shoot() {
        InitBullet(FindShootDirection());
    }
}
