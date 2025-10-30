using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Estado de la Quest")]
    public List<int> componentesRecolectados = new List<int>();
    
    [Header("Referencias")]
    public Inventory inventory;
    
    // IDs de los componentes necesarios
    private readonly List<int> componentesNecesarios = new List<int> { 1, 2, 3 };
    
    void Start()
    {
        // Si no se asigna inventario, buscar en el mismo GameObject
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }
    
    /// <summary>
    /// Llamado cuando el jugador recoge un item
    /// </summary>
    public void OnItemRecogido(int itemId)
    {
        // Verificar si es un componente del estabilizador
        if (componentesNecesarios.Contains(itemId))
        {
            if (!componentesRecolectados.Contains(itemId))
            {
                componentesRecolectados.Add(itemId);
                Debug.Log($"¡Componente {itemId} recolectado! ({componentesRecolectados.Count}/3)");
                
                // Mostrar progreso
                MostrarProgreso();
            }
        }
    }
    
    /// <summary>
    /// Llamado cuando el jugador coloca un componente en el estabilizador
    /// Esto permite que el mismo componente se pueda recoger de nuevo
    /// </summary>
    public void OnComponenteColocado(int itemId)
    {
        if (componentesRecolectados.Contains(itemId))
        {
            componentesRecolectados.Remove(itemId);
            Debug.Log($"Componente {itemId} colocado en estabilizador. Ahora puedes recogerlo de nuevo. ({componentesRecolectados.Count}/3)");
        }
    }
    
    /// <summary>
    /// Verifica si el jugador tiene los 3 componentes necesarios
    /// </summary>
    public bool TieneLos3Componentes()
    {
        foreach (int componenteId in componentesNecesarios)
        {
            if (!componentesRecolectados.Contains(componenteId))
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Obtiene el progreso actual de la quest
    /// </summary>
    public float GetProgreso()
    {
        return (float)componentesRecolectados.Count / componentesNecesarios.Count;
    }
    
    /// <summary>
    /// Obtiene el número de componentes recolectados
    /// </summary>
    public int GetComponentesRecolectados()
    {
        return componentesRecolectados.Count;
    }
    
    /// <summary>
    /// Verifica si un componente específico ha sido recolectado
    /// </summary>
    public bool TieneComponente(int componenteId)
    {
        return componentesRecolectados.Contains(componenteId);
    }
    
    /// <summary>
    /// Resetea el progreso de la quest
    /// </summary>
    public void Resetear()
    {
        componentesRecolectados.Clear();
        Debug.Log("QuestManager reseteado - Componentes eliminados");
    }
    
    /// <summary>
    /// Muestra el progreso actual en consola
    /// </summary>
    private void MostrarProgreso()
    {
        string progreso = "Componentes encontrados: ";
        
        foreach (int componenteId in componentesNecesarios)
        {
            if (componentesRecolectados.Contains(componenteId))
            {
                progreso += $"[{componenteId}✓] ";
            }
            else
            {
                progreso += $"[{componenteId}✗] ";
            }
        }if (TieneLos3Componentes())
        {}
    }
    
    /// <summary>
    /// Obtiene una lista con los componentes que faltan
    /// </summary>
    public List<int> GetComponentesFaltantes()
    {
        List<int> faltantes = new List<int>();
        
        foreach (int componenteId in componentesNecesarios)
        {
            if (!componentesRecolectados.Contains(componenteId))
            {
                faltantes.Add(componenteId);
            }
        }
        
        return faltantes;
    }
}