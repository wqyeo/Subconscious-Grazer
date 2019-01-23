using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines this entity can do shooting.
/// </summary>
public interface IShooting {
    /// <summary>
    /// How long to wait for before this entity can shoot again.
    /// </summary>
    float ShootCooldown { get; set; }

    void Shoot();
}
