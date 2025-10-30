using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    [Header("Sistema de Inventario")]
    public Dictionary<int, int> slots = new Dictionary<int, int>();
    
    [Header("Configuración")]
    public int maxSlots = 20;
    
    // Cache para optimización
    private List<int> componentesIds = new List<int> { 1, 2, 3 };
    
    /// <summary>
    /// Agrega un item al inventario
    /// </summary>
    public bool AgregarItem(int itemId, int cantidad = 1)
    {
        if (cantidad <= 0) return false;
        
        if (slots.ContainsKey(itemId))
        {
            slots[itemId] += cantidad;
        }
        else
        {
            if (slots.Count >= maxSlots)
            {return false;
            }
            slots[itemId] = cantidad;
        }return true;
    }
    
    /// <summary>
    /// Remueve un item del inventario
    /// </summary>
    public bool RemoverItem(int itemId, int cantidad = 1)
    {
        if (cantidad <= 0) return false;
        
        if (!slots.ContainsKey(itemId) || slots[itemId] < cantidad)
        {
            return false;
        }
        
        slots[itemId] -= cantidad;
        
        // Si queda en 0, remover la entrada
        if (slots[itemId] <= 0)
        {
            slots.Remove(itemId);
        }return true;
    }
    
    /// <summary>
    /// Verifica si el inventario tiene cierta cantidad de un item
    /// </summary>
    public bool CheckItem(int itemId, int cantidad = 1)
    {
        return slots.ContainsKey(itemId) && slots[itemId] >= cantidad;
    }
    
    /// <summary>
    /// Obtiene la cantidad de un item específico
    /// </summary>
    public int GetCantidad(int itemId)
    {
        return slots.ContainsKey(itemId) ? slots[itemId] : 0;
    }
    
    /// <summary>
    /// Cuenta cuántos componentes del estabilizador se tienen
    /// </summary>
    public int ContarComponentes()
    {
        int totalComponentes = 0;
        
        foreach (int componenteId in componentesIds)
        {
            if (CheckItem(componenteId))
            {
                totalComponentes++;
            }
        }
        
        return totalComponentes;
    }
    
    /// <summary>
    /// Obtiene todos los items del inventario
    /// </summary>
    public Dictionary<int, int> GetTodosLosItems()
    {
        return new Dictionary<int, int>(slots);
    }
    
    /// <summary>
    /// Limpia completamente el inventario
    /// </summary>
    public void Limpiar()
    {
        slots.Clear();}
    
    /// <summary>
    /// Verifica si el inventario está vacío
    /// </summary>
    public bool EstaVacio()
    {
        return slots.Count == 0;
    }
    
    /// <summary>
    /// Obtiene el número total de slots ocupados
    /// </summary>
    public int GetSlotsOcupados()
    {
        return slots.Count;
    }
}