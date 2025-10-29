using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("Referencias")]
    public Image iconoItem;
    public TextMeshProUGUI textoCantidad;
    public Image fondoSlot;
    
    private int slotIndex;
    
    public void InicializarSlot(int index)
    {
        slotIndex = index;
        
        // Buscar referencias si no están asignadas
        if (iconoItem == null)
            iconoItem = transform.Find("IconoItem")?.GetComponent<Image>();
        if (textoCantidad == null)
            textoCantidad = transform.Find("TextoCantidad")?.GetComponent<TextMeshProUGUI>();
        if (fondoSlot == null)
            fondoSlot = GetComponent<Image>();
            
        LimpiarSlot();
    }
    
    public void ConfigurarSlot(Item item, int cantidad)
    {
        if (iconoItem != null)
        {
            iconoItem.sprite = item.icon;
            iconoItem.enabled = true;
        }
        
        if (textoCantidad != null)
        {
            textoCantidad.text = $"x{cantidad}";
            textoCantidad.enabled = true;
        }
    }
    
    public void ConfigurarSlotDesconocido(int itemId, int cantidad)
    {
        if (iconoItem != null)
        {
            iconoItem.sprite = null;
            iconoItem.enabled = false;
        }
        
        if (textoCantidad != null)
        {
            textoCantidad.text = $"x{cantidad}";
            textoCantidad.enabled = true;
        }
    }
    
    public void LimpiarSlot()
    {
        if (iconoItem != null)
        {
            iconoItem.sprite = null;
            iconoItem.enabled = false;
        }
        
        if (textoCantidad != null)
        {
            textoCantidad.text = "";
            textoCantidad.enabled = false;
        }
    }
    
    public void SetSeleccionado(bool seleccionado)
    {
        // El borde visual se maneja desde InventoryUI
        // Aquí podrías agregar un efecto extra si querés
    }
}
