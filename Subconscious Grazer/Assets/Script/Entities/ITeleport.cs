using UnityEngine;

/// <summary>
/// Defines this entity can do teleporting.
/// </summary>
public interface ITeleport {
    /// <summary>
    /// The set distance between each teleportation.
    /// </summary>
    float TeleDistance { get; set; }

    /// <summary>
    /// The duration before each teleport can occur.
    /// </summary>
    float TeleDuration { get; set; }

    void Teleport(Vector2 direction);
}
