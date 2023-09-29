using UnityEngine;

public abstract class BoatBulletShooter : BoatWeapon
{
    [SerializeField]
    protected BoatBullet bulletToShootWith;
    
    public virtual Transform Target { get => target; set => target = value; }

    protected Transform target;
}
