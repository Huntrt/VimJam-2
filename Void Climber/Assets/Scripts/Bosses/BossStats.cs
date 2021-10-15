using System; using UnityEngine;

///An bullet that move with speed in certain direction until reach range
[Serializable] public class BulletStats {public float range, speed;}

///An bomb the move toward it destination with speed to explode with scale of radius
[Serializable] public class BombStats 
{public float speed, radius; public Vector2 destination; public Transform explosion;}

///An laser has an size that can reach an range with an wind up time to create it toward target
[Serializable] public class LaserStats {public float size, range, windUp; public Vector2 target;}

///Projectile attack stats
[Serializable] public class ProjectileAttack {public GameObject attack;
public float rate, burst, delay, spread, focus;}