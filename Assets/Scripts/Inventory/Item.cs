using UnityEngine;

public enum ItemType { Food, Water, Components, Weapon }

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public int id;
    public ItemType type;
    public Sprite icon;
    public int weaponDamage; // Solo se usa si type == Weapon
}