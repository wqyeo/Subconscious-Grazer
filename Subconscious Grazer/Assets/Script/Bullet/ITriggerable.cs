using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
//  Mainly for bullets that can be triggered when they enter a certain region.
/// </summary>
public interface ITriggerable {

    /// <summary>
    /// Effects to invoke on the bullet when the bullet enters the trigger.
    /// </summary>
    BulletTriggerEffect OnTriggerEnterEff { get; }
    BulletTriggerEffect OnTriggerExitEff { get; }

    // Invoke the trigger action when the bullet enters.
    void InvokeEnter();

    void InvokeExit();
}
