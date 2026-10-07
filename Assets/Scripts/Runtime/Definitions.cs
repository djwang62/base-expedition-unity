using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseExpedition {
  public enum RegionId { Base, Forest, Mountain, Mine, Wasteland }
  public enum EnemyKind { TreeSprite, Seedling, ForestGuardian, StoneGolem, RockThrower, MountainGuardian, IronBrute, OreShooter, MineGuardian, OuterBoss }
  public enum WeaponId { Torch, Slingshot, Spear, Bow }

  [Serializable] public sealed class ItemStack {
    public string Id; public string Label; public int Quantity;
    public ItemStack(string id, string label, int quantity) { Id = id; Label = label; Quantity = quantity; }
  }
  [Serializable] public sealed class WeaponDefinition {
    public WeaponId Id; public string Label; public bool Ranged; public bool Line; public float Damage; public float Cooldown; public float Range; public float Arc; public float ProjectileSpeed; public float ThrustWidth; public string UpgradeMaterial;
  }
  [Serializable] public sealed class EnemyDefinition {
    public EnemyKind Id; public string Label; public RegionId Region; public float MaxHealth; public float Speed; public float ContactRange; public float Damage; public float Cooldown; public float Radius; public bool Ranged; public bool Charger; public string Material; public string MaterialLabel; public bool Guardian;
  }
  public static class Definitions {
    public const float BaseRadius = 560f, BiomeWidth = 2800f, WastelandWidth = 2100f, WorldRadius = 11060f;
    public static readonly Dictionary<WeaponId, WeaponDefinition> Weapons = new() {
      { WeaponId.Torch, new WeaponDefinition { Id=WeaponId.Torch, Label="火把", Damage=18, Cooldown=.55f, Range=88, Arc=100, UpgradeMaterial="wood" } },
      { WeaponId.Slingshot, new WeaponDefinition { Id=WeaponId.Slingshot, Label="彈弓", Ranged=true, Damage=15, Cooldown=.7f, Range=330, ProjectileSpeed=520, UpgradeMaterial="wood" } },
      { WeaponId.Spear, new WeaponDefinition { Id=WeaponId.Spear, Label="長槍", Line=true, Damage=30, Cooldown=.8f, Range=138, Arc=24, ThrustWidth=22, UpgradeMaterial="stone" } },
      { WeaponId.Bow, new WeaponDefinition { Id=WeaponId.Bow, Label="弓箭", Ranged=true, Damage=22, Cooldown=.55f, Range=440, ProjectileSpeed=1440, UpgradeMaterial="stone" } }
    };
    public static readonly Dictionary<EnemyKind, EnemyDefinition> Enemies = new() {
      { EnemyKind.TreeSprite, E(EnemyKind.TreeSprite,"樹妖",RegionId.Forest,56,72,30,9,1.1f,20,"wood","木材") },
      { EnemyKind.Seedling, ER(EnemyKind.Seedling,"種子怪",RegionId.Forest,38,56,210,6,1.45f,15,"wood","木材") },
      { EnemyKind.ForestGuardian, EG(EnemyKind.ForestGuardian,"森林守衛",RegionId.Forest,280,82,34,16,.95f,31,"wood","木材") },
      { EnemyKind.StoneGolem, E(EnemyKind.StoneGolem,"石像",RegionId.Mountain,110,46,36,19,1.15f,24,"stone","石材") },
      { EnemyKind.RockThrower, ER(EnemyKind.RockThrower,"投石怪",RegionId.Mountain,74,42,260,12,1.55f,18,"stone","石材") },
      { EnemyKind.MountainGuardian, EG(EnemyKind.MountainGuardian,"山脈守衛",RegionId.Mountain,490,64,38,26,.9f,36,"stone","石材") },
      { EnemyKind.IronBrute, EC(EnemyKind.IronBrute,"鐵礦怪",RegionId.Mine,180,72,38,29,1.05f,27,"metal","金屬") },
      { EnemyKind.OreShooter, ER(EnemyKind.OreShooter,"礦石射手",RegionId.Mine,120,58,300,17,1.2f,20,"metal","金屬") },
      { EnemyKind.MineGuardian, EG(EnemyKind.MineGuardian,"礦場守衛",RegionId.Mine,760,88,42,36,.82f,41,"metal","金屬") },
      { EnemyKind.OuterBoss, EG(EnemyKind.OuterBoss,"外圈守衛者",RegionId.Wasteland,1800,78,46,30,1.1f,52,"metal","金屬") }
    };
    static EnemyDefinition E(EnemyKind id,string label,RegionId region,float hp,float speed,float range,float damage,float cooldown,float radius,string mat,string materialLabel) => new EnemyDefinition {Id=id,Label=label,Region=region,MaxHealth=hp,Speed=speed,ContactRange=range,Damage=damage,Cooldown=cooldown,Radius=radius,Material=mat,MaterialLabel=materialLabel};
    static EnemyDefinition ER(EnemyKind id,string label,RegionId region,float hp,float speed,float range,float damage,float cooldown,float radius,string mat,string materialLabel) { var e=E(id,label,region,hp,speed,range,damage,cooldown,radius,mat,materialLabel); e.Ranged=true; return e; }
    static EnemyDefinition EC(EnemyKind id,string label,RegionId region,float hp,float speed,float range,float damage,float cooldown,float radius,string mat,string materialLabel) { var e=E(id,label,region,hp,speed,range,damage,cooldown,radius,mat,materialLabel); e.Charger=true; return e; }
    static EnemyDefinition EG(EnemyKind id,string label,RegionId region,float hp,float speed,float range,float damage,float cooldown,float radius,string mat,string materialLabel) { var e=E(id,label,region,hp,speed,range,damage,cooldown,radius,mat,materialLabel); e.Guardian=true; return e; }
    public static RegionId RegionAt(Vector2 position) { float d=position.magnitude; if(d<=BaseRadius)return RegionId.Base; if(d<=BaseRadius+BiomeWidth)return RegionId.Forest; if(d<=BaseRadius+BiomeWidth*2)return RegionId.Mountain; if(d<=BaseRadius+BiomeWidth*3)return RegionId.Mine; return RegionId.Wasteland; }
    public static float RadiusFor(RegionId id) => id switch { RegionId.Forest=>BaseRadius+BiomeWidth, RegionId.Mountain=>BaseRadius+BiomeWidth*2, RegionId.Mine=>BaseRadius+BiomeWidth*3, RegionId.Wasteland=>WorldRadius, _=>BaseRadius };
  }
}
