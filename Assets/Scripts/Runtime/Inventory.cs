using System.Collections.Generic;
using System.Linq;

namespace BaseExpedition {
  public sealed class Inventory {
    public int SlotCount { get; private set; } = 5;
    public const int StackSize = 10;
    public readonly List<ItemStack> Stacks = new();
    public int UsedSlots => Stacks.Count;
    public int Count(string id) => Stacks.Where(s => s.Id == id).Sum(s => s.Quantity);
    public int Add(ItemStack item) {
      int remaining = item.Quantity;
      foreach (var stack in Stacks.Where(s => s.Id == item.Id && s.Quantity < StackSize)) { int added = System.Math.Min(StackSize-stack.Quantity, remaining); stack.Quantity += added; remaining -= added; if (remaining == 0) return 0; }
      while (remaining > 0 && Stacks.Count < SlotCount) { int added = System.Math.Min(StackSize, remaining); Stacks.Add(new ItemStack(item.Id,item.Label,added)); remaining -= added; }
      return remaining;
    }
    public bool Remove(string id, int quantity) { if (Count(id)<quantity)return false; for(int i=Stacks.Count-1; i>=0 && quantity>0; i--) if(Stacks[i].Id==id) { int used=System.Math.Min(quantity,Stacks[i].Quantity); Stacks[i].Quantity-=used; quantity-=used; if(Stacks[i].Quantity==0)Stacks.RemoveAt(i); } return true; }
    public List<ItemStack> TakeAll() { var all=Stacks.Select(s=>new ItemStack(s.Id,s.Label,s.Quantity)).ToList(); Stacks.Clear(); return all; }
    public void UpgradeSlots(int amount) => SlotCount = System.Math.Max(SlotCount, amount);
  }
}
