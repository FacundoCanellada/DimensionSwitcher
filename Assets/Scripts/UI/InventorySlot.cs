using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("Referencias UI del Slot")]
    public Image backgroundImage;
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    
    [Header("Configuración Visual")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = Color.yellow;
    public Color colorVacio = Color.gray;
    
    // Estado del slot
    private int slotIndex;
    private Item itemActual;
    private int cantidadActual;
    private bool estaVacio = true;
    private bool estaSeleccionado = false;

    void Awake()
    {
        // Buscar componentes si no están asignados
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        
        if (iconImage == null)
            iconImage = transform.Find("Icon")?.GetComponent<Image>();
        
        if (quantityText == null)
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Configurar estado inicial
        LimpiarSlot();
    }
    
    /// <summary>
    /// Inicializa el slot con su índice
    /// </summary>
    public void InicializarSlot(int index)
    {
        slotIndex = index;
        name = $"InventorySlot_{index}";
        LimpiarSlot();
    }
    
    /// <summary>
    /// Configura el slot con un item
    /// </summary>
    public void ConfigurarSlot(Item item, int cantidad)
    {
        itemActual = item;
        cantidadActual = cantidad;
        estaVacio = false;
        
        // Configurar icono
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
            iconImage.color = Color.white;
        }
        
        // Configurar texto de cantidad
        if (quantityText != null)
        {
            if (cantidad > 1)
            {
                quantityText.text = cantidad.ToString();
                quantityText.enabled = true;
            }
            else
            {
                quantityText.enabled = false;
            }
        }
        
        // Actualizar fondo
        ActualizarApariencia();
    }
    
    /// <summary>
    /// Configura el slot para un item desconocido
    /// </summary>
    public void ConfigurarSlotDesconocido(int itemId, int cantidad)
    {
        itemActual = null;
        cantidadActual = cantidad;
        estaVacio = false;
        
        // Configurar icono como signo de pregunta o similar
        if (iconImage != null)
        {
            iconImage.sprite = null; // O asignar un sprite de "desconocido"
            iconImage.enabled = true;
            iconImage.color = Color.red; // Color para indicar error
        }
        
        // Configurar texto de cantidad
        if (quantityText != null)
        {
            quantityText.text = $"?{cantidad}";
            quantityText.enabled = true;
            quantityText.color = Color.red;
        }
        
        ActualizarApariencia();
    }
    
    /// <summary>
    /// Limpia el slot dejándolo vacío
    /// </summary>
    public void LimpiarSlot()
    {
        itemActual = null;
        cantidadActual = 0;
        estaVacio = true;
        
        // Limpiar icono
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        
        // Limpiar texto
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
            quantityText.color = Color.white;
        }
        
        ActualizarApariencia();
    }
    
    /// <summary>
    /// Establece si el slot está seleccionado
    /// </summary>
    public void SetSeleccionado(bool seleccionado)
    {
        estaSeleccionado = seleccionado;
        ActualizarApariencia();
    }
    
    /// <summary>
    /// Actualiza la apariencia visual del slot
    /// </summary>
    private void ActualizarApariencia()
    {
        if (backgroundImage == null) return;
        
        Color colorFondo;
        
        if (estaVacio)
        {
            colorFondo = colorVacio;
        }
        else if (estaSeleccionado)
        {
            colorFondo = colorSeleccionado;
        }
        else
        {
            colorFondo = colorNormal;
        }
        
        backgroundImage.color = colorFondo;
        
        // Añadir efecto de brillo si está seleccionado
        if (estaSeleccionado && !estaVacio)
        {
            // Opcional: agregar outline o glow effect
            Outline outline = GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.effectColor = colorSeleccionado;
            }
        }
        else
        {
            Outline outline = GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Obtiene información del item en el slot
    /// </summary>
    public Item GetItem()
    {
        return itemActual;
    }
    
    /// <summary>
    /// Obtiene la cantidad en el slot
    /// </summary>
    public int GetCantidad()
    {
        return cantidadActual;
    }
    
    /// <summary>
    /// Verifica si el slot está vacío
    /// </summary>
    public bool EstaVacio()
    {
        return estaVacio;
    }
    
    /// <summary>
    /// Verifica si el slot está seleccionado
    /// </summary>
    public bool EstaSeleccionado()
    {
        return estaSeleccionado;
    }
    
    /// <summary>
    /// Obtiene el índice del slot
    /// </summary>
    public int GetIndex()
    {
        return slotIndex;
    }
    
    /// <summary>
    /// Obtiene información de debug del slot
    /// </summary>
    public string GetDebugInfo()
    {
        if (estaVacio)
        {
            return $"Slot {slotIndex}: Vacío";
        }
        else if (itemActual != null)
        {
            return $"Slot {slotIndex}: {itemActual.nombre} x{cantidadActual}";
        }
        else
        {
            return $"Slot {slotIndex}: Item desconocido x{cantidadActual}";
        }
    }
}