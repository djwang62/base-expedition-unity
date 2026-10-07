using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseExpedition {
  public sealed class BaseState {
    public readonly HashSet<string> Built = new() { "mainFire" }; public readonly Dictionary<string,ItemStack> Storage=new(); public int HealingLevel=1;
    public bool Warehouse => Built.Contains("warehouse");
    public int Available(string id, Inventory inventory) => (Storage.TryGetValue(id,out var item)?item.Quantity:0)+inventory.Count(id);
    public bool Affords(Dictionary<string,int> cost, Inventory inv) => cost.All(p=>Available(p.Key,inv)>=p.Value);
    public bool Consume(Dictionary<string,int> cost, Inventory inv) { if(!Affords(cost,inv))return false; foreach(var pair in cost) { int stored=Storage.TryGetValue(pair.Key,out var stack)?stack.Quantity:0; int fromStore=Mathf.Min(stored,pair.Value); if(fromStore>0) {stack.Quantity-=fromStore;if(stack.Quantity==0)Storage.Remove(pair.Key);} if(pair.Value>fromStore)inv.Remove(pair.Key,pair.Value-fromStore); } return true; }
    public void Deposit(Inventory inv) { if(!Warehouse)return; foreach(var item in inv.TakeAll()) { if(!Storage.TryGetValue(item.Id,out var stored))Storage[item.Id]=stored=new ItemStack(item.Id,item.Label,0); stored.Quantity+=item.Quantity; } }
  }
  public sealed class ArmoryState {
    public readonly HashSet<WeaponId> Owned = new() { WeaponId.Torch }; public readonly Dictionary<WeaponId,int> Slots=new();
    public bool Craft(WeaponId id, BaseState baseState, Inventory inv) { if(Owned.Contains(id)) return false; var cost=id switch { WeaponId.Slingshot=>new Dictionary<string,int>{{"wood",25},{"magicShard",2}}, WeaponId.Spear=>new Dictionary<string,int>{{"wood",10},{"stone",10},{"magicShard",1}}, WeaponId.Bow=>new Dictionary<string,int>{{"wood",10},{"stone",20},{"magicShard",3}}, _=>null}; if(cost==null || !baseState.Consume(cost,inv))return false; Owned.Add(id); return true; }
  }
}
