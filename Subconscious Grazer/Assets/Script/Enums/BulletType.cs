using UnityEngine;

[System.Serializable]
public enum BulletType {
    Undefined,

    #region Player_Bullets
    PlayerBullet_default,
    PlayerBullet_needle,
    #endregion

    #region Enemy_Bullets

    star_Small,
    ball_Small,
    arrow,
    ringBall,
    fadeBall,

    arrow_Effect,
    ringBall_Effect
    #endregion
}
