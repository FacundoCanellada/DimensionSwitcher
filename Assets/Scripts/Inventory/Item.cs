using UnityEngine;

public enum ItemType { Food, Water, Components, Weapon }

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Header("Propiedades Básicas")]
    public int id;
    public ItemType type;
    public Sprite icon;
    
    [Header("Valores de Efectos")]
    public int valor; // Nutrición, hidratación, etc.
    public int weaponDamage; // Solo se usa si type == Weapon
    
    [Header("Información Visual")]
    public string nombre;
    public string descripcion;
    
    /// <summary>
    /// Usa el item en el jugador aplicando sus efectos
    /// SISTEMA SIMPLIFICADO: Food/Water curan salud según ID
    /// </summary>
    public bool Usar(Cientifico cientifico)
    {
        if (cientifico == null) return false;
        
        switch (type)
        {
            case ItemType.Weapon:
                cientifico.arma = this;
                Debug.Log($"Equipaste {nombre}. Daño: {weaponDamage}");
                return true;
                
            case ItemType.Components:
                // Los componentes no se "usan" directamente, se insertan en el estabilizador
                Debug.Log($"Componente {nombre} listo para insertar en el estabilizador");
                return false;
                
            case ItemType.Food:
            case ItemType.Water:
                // Consumir comida o agua - curan salud según ID
                // El inventario se encarga de eliminarlo, aquí solo aplicamos efectos
                cientifico.AplicarEfectoItem(id);
                Debug.Log($"Consumiste {nombre}");
                return true;
                
            default:
                Debug.LogWarning($"Tipo de item no reconocido: {type}");
                return false;
        }
    }
}