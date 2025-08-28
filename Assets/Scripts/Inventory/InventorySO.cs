using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventorySO", menuName = "Inventory/InventorySO")]
public class InventorySO : ScriptableObject
{
    public Dictionary<int, int> slots = new();

    public void AddItem(Item item, int cantidad = 1)
    {
        if (slots.ContainsKey(item.id))
            slots[item.id] += cantidad;
        else
            slots[item.id] = cantidad;
    }

    public bool RemoveItem(Item item, int cantidad = 1)
    {
        if (slots.ContainsKey(item.id) && slots[item.id] >= cantidad)
        {
            slots[item.id] -= cantidad;
            if (slots[item.id] <= 0) slots.Remove(item.id);
            return true;
        }
        return false;
    }

    public int GetItemCount(int itemId)
    {
        return slots.ContainsKey(itemId) ? slots[itemId] : 0;
    }

    public void Clear()
    {
        slots.Clear();
    }
}