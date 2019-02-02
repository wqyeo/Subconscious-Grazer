using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShooter : TargettingShooter {

    protected override void InvokeShooting() {
        CreateBulletObject(GetShootDirection());
    }
}
