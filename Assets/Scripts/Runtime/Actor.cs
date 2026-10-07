using System;
using UnityEngine;

namespace BaseExpedition {
  public sealed class PlayerState {
    public Vector2 Position; public Vector2 Direction=Vector2.right; public float Health=100, MaxHealth=100, Armor, MoveMultiplier=1, DashRemaining, DashCooldown, AttackCooldown; public WeaponId Weapon=WeaponId.Torch; public int WeaponLevel; public readonly Inventory Inventory=new(); public readonly System.Collections.Generic.HashSet<string> Modules=new();
    public WeaponDefinition Definition => Definitions.Weapons[Weapon];
    public float Damage => Definition.Damage * (WeaponLevel==0 ? 1 : WeaponLevel==1 ? 1.2f : WeaponLevel==2 ? 1.45f : 1.75f);
    public float Range => Definition.Range * (WeaponLevel==0 ? 1 : WeaponLevel==1 ? 1.1f : WeaponLevel==2 ? 1.22f : 1.36f);
    public float Cooldown => Definition.Cooldown * (WeaponLevel==0 ? 1 : WeaponLevel==1 ? .94f : WeaponLevel==2 ? .87f : .8f);
    public bool IsDashing => DashRemaining>0;
    public int TakeDamage(float damage) { if (IsDashing) return 0; int actual=Mathf.Max(1,Mathf.RoundToInt(damage-Armor)); Health=Mathf.Max(0,Health-actual); return actual; }
  }
  public sealed class EnemyState {
    public readonly string Id=Guid.NewGuid().ToString("N"); public readonly EnemyDefinition Definition; public Vector2 Position; public float Health, AttackCooldown, ChargeRemaining, ChargeCooldown; public bool Alive => Health>0; public readonly Vector2 Home;
    public EnemyState(EnemyKind kind, Vector2 position) { Definition=Definitions.Enemies[kind]; Position=position; Home=position; Health=Definition.MaxHealth; }
    public void Damage(float amount) => Health=Mathf.Max(0,Health-amount);
  }
  public sealed class ProjectileState { public Vector2 Position, Direction; public float Speed, Remaining, Damage, Radius; public bool FromPlayer, Homing, Piercing; public int Bounces; public EnemyState Source; public ProjectileState(Vector2 pos,Vector2 dir,float speed,float remaining,float damage,bool fromPlayer) { Position=pos;Direction=dir.normalized;Speed=speed;Remaining=remaining;Damage=damage;FromPlayer=fromPlayer;Radius=9; } }
  public sealed class DropState { public Vector2 Position; public ItemStack Item; public DropState(Vector2 pos,ItemStack item){Position=pos;Item=item;} }
  public sealed class FireWallState { public Vector2 Origin; public float Angle, Arc, Radius, Damage, Remaining=1; public readonly System.Collections.Generic.HashSet<string> Hit=new(); }
}
