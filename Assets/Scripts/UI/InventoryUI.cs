using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias del UI")]
    public GameObject inventoryPanel;
    public Transform slotsParent;
    public GameObject slotPrefab;
    
    [Header("Información del Item")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemQuantityText;
    public Image itemPreviewImage;
    
    [Header("Navegación")]
    public int columnas = 4; // 4 columnas como en tu imagen
    public int filas = 3; // 3 filas
    public Color colorSlotSeleccionado = Color.yellow;
    public Color colorSlotNormal = Color.white;
    public Image bordeSeleccion; // Imagen/borde que resalta el slot seleccionado
    
    [Header("Referencias de Items")]
    public Item[] todosLosItemsDelJuego; // Asignar todos los ScriptableObject de items
    
    // Variables internas
    private Inventory inventory;
    private Cientifico cientifico;
    private List<InventorySlot> slots = new List<InventorySlot>();
    private int slotSeleccionado = 0;
    private bool inventarioAbierto = false;
    
    // Cache para optimización
    private Dictionary<int, Item> itemsCache = new Dictionary<int, Item>();

    void Start()
    {
        Debug.Log("InventoryUI.Start() iniciado");
        
        // Buscar referencias
        cientifico = FindFirstObjectByType<Cientifico>();
        if (cientifico != null)
        {
            Debug.Log("Científico encontrado");
            inventory = cientifico.GetComponent<Inventory>();
            if (inventory != null)
            {
                Debug.Log("Inventory encontrado en el científico");
            }
            else
            {
                Debug.LogError("NO se encontró componente Inventory en el científico");
            }
        }
        else
        {
            Debug.LogError("NO se encontró científico en la escena");
        }
        
        // Verificar referencias del panel
        Debug.Log($"InventoryPanel es null: {inventoryPanel == null}");
        if (inventoryPanel != null) Debug.Log($"InventoryPanel name: {inventoryPanel.name}");
        
        Debug.Log($"SlotsParent es null: {slotsParent == null}");
        if (slotsParent != null) Debug.Log($"SlotsParent name: {slotsParent.name}");
        
        Debug.Log($"SlotPrefab es null: {slotPrefab == null}");
        if (slotPrefab != null) Debug.Log($"SlotPrefab name: {slotPrefab.name}");
        
        // Crear cache de items
        CrearCacheItems();
        
        // Crear slots de inventario
        CrearSlots();
        
        // Auto-asignar InventoryPanel si no está configurado
        if (inventoryPanel == null)
        {
            inventoryPanel = gameObject;
            Debug.Log("InventoryPanel auto-asignado al GameObject actual");
        }
        
        // Inicializar UI cerrado
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("InventoryPanel desactivado inicialmente");
        }
        else
        {
            Debug.LogError("InventoryPanel es NULL - ¡Asignar en Inspector!");
        }
        
        Debug.Log("InventoryUI inicializado");
    }
    
    void Update()
    {
        // El control de apertura/cierre ahora lo maneja UIManager con Tab
        // Solo procesar input si el panel está activo
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            inventarioAbierto = true;
            ManejarNavegacion();
            ManejarAcciones();
        }
        else
        {
            inventarioAbierto = false;
        }
    }
    
    void OnEnable()
    {
        // Cuando se activa el panel, SIEMPRE actualizar el inventario
        ActualizarInventario();
        slotSeleccionado = 0;
        Debug.Log("InventoryUI OnEnable - Inventario actualizado");
    }
    
    /// <summary>
    /// Crea cache de items para acceso rápido por ID
    /// </summary>
    private void CrearCacheItems()
    {
        itemsCache.Clear();
        
        if (todosLosItemsDelJuego != null)
        {
            foreach (Item item in todosLosItemsDelJuego)
            {
                if (item != null)
                {
                    itemsCache[item.id] = item;
                }
            }
        }
        
        Debug.Log($"Cache de items creado con {itemsCache.Count} items");
    }
    
    /// <summary>
    /// Crea los slots visuales del inventario
    /// </summary>
    private void CrearSlots()
    {
        if (slotsParent == null || slotPrefab == null) return;
        
        // Limpiar slots existentes
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();
        
        int totalSlots = columnas * filas;
        
        // Crear nuevos slots
        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot == null)
            {
                // Si no existe el componente, agregarlo
                slot = slotObj.AddComponent<InventorySlot>();
            }
            
            slot.InicializarSlot(i);
            slots.Add(slot);
        }
        
        Debug.Log($"Creados {slots.Count} slots de inventario");
    }
    
    /// <summary>
    /// Maneja la navegación con teclado
    /// </summary>
    private void ManejarNavegacion()
    {
        int totalSlots = columnas * filas;
        
        // Navegación horizontal
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            slotSeleccionado++;
            if (slotSeleccionado >= totalSlots) slotSeleccionado = 0;
            ActualizarSeleccion();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            slotSeleccionado--;
            if (slotSeleccionado < 0) slotSeleccionado = totalSlots - 1;
            ActualizarSeleccion();
        }
        
        // Navegación vertical
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            slotSeleccionado -= columnas;
            if (slotSeleccionado < 0) slotSeleccionado += totalSlots;
            ActualizarSeleccion();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            slotSeleccionado += columnas;
            if (slotSeleccionado >= totalSlots) slotSeleccionado -= totalSlots;
            ActualizarSeleccion();
        }
    }
    
    /// <summary>
    /// Maneja las acciones del inventario
    /// </summary>
    private void ManejarAcciones()
    {
        // Usar/Equipar item con X
        if (Input.GetKeyDown(KeyCode.X))
        {
            UsarItemSeleccionado();
        }
        
        // Tirar item con Delete
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            TirarItemSeleccionado();
        }
    }
    
    /// <summary>
    /// Actualiza toda la visualización del inventario
    /// </summary>
    public void ActualizarInventario()
    {
        if (inventory == null) return;
        
        // Limpiar slots
        foreach (var slot in slots)
        {
            slot.LimpiarSlot();
        }
        
        // Llenar slots con items del inventario
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new List<KeyValuePair<int, int>>(todosLosItems);
        
        for (int i = 0; i < Mathf.Min(itemsArray.Count, slots.Count); i++)
        {
            var itemData = itemsArray[i];
            Item itemSO = ObtenerItemPorId(itemData.Key);
            
            if (itemSO != null)
            {
                slots[i].ConfigurarSlot(itemSO, itemData.Value);
            }
            else
            {
                // Item no encontrado, mostrar como desconocido
                slots[i].ConfigurarSlotDesconocido(itemData.Key, itemData.Value);
            }
        }
        
        ActualizarSeleccion();
    }
    
    /// <summary>
    /// Actualiza la visualización del slot seleccionado
    /// </summary>
    private void ActualizarSeleccion()
    {
        if (inventory == null) return;
        
        // Actualizar colores de slots
        for (int i = 0; i < slots.Count; i++)
        {
            bool esSeleccionado = i == slotSeleccionado;
            slots[i].SetSeleccionado(esSeleccionado);
        }
        
        // Mover el borde de selección si existe
        if (bordeSeleccion != null && slotSeleccionado < slots.Count)
        {
            bordeSeleccion.transform.position = slots[slotSeleccionado].transform.position;
            bordeSeleccion.gameObject.SetActive(true);
        }
        
        // Actualizar información del item seleccionado
        MostrarInformacionItem();
    }
    
    /// <summary>
    /// Muestra la información del item seleccionado
    /// </summary>
    private void MostrarInformacionItem()
    {
        if (inventory == null) return;
        
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new List<KeyValuePair<int, int>>(todosLosItems);
        
        if (slotSeleccionado < itemsArray.Count)
        {
            var itemData = itemsArray[slotSeleccionado];
            Item itemSO = ObtenerItemPorId(itemData.Key);
            
            if (itemSO != null)
            {
                ActualizarInfoPanel(itemSO, itemData.Value);
            }
            else
            {
                ActualizarInfoPanelDesconocido(itemData.Key, itemData.Value);
            }
        }
        else
        {
            LimpiarInfoPanel();
        }
    }
    
    /// <summary>
    /// Actualiza el panel de información con datos del item
    /// </summary>
    private void ActualizarInfoPanel(Item item, int cantidad)
    {
        if (itemNameText != null)
            itemNameText.text = item.nombre;
        
        if (itemDescriptionText != null)
            itemDescriptionText.text = item.descripcion;
        
        if (itemQuantityText != null)
            itemQuantityText.text = $"Cantidad: {cantidad}";
        
       if (itemPreviewImage != null)
        {
            itemPreviewImage.sprite = item.icon;
            itemPreviewImage.enabled = item.icon != null;
        }
    }
    
    /// <summary>
    /// Actualiza el panel para items desconocidos
    /// </summary>
    private void ActualizarInfoPanelDesconocido(int itemId, int cantidad)
    {
        if (itemNameText != null)
            itemNameText.text = $"Item Desconocido (ID: {itemId})";
        
        if (itemDescriptionText != null)
            itemDescriptionText.text = "Item no configurado correctamente";
        
        if (itemQuantityText != null)
            itemQuantityText.text = $"Cantidad: {cantidad}";
        
        if (itemPreviewImage != null)
            itemPreviewImage.enabled = false;
    }
    
    /// <summary>
    /// Limpia el panel de información
    /// </summary>
    private void LimpiarInfoPanel()
    {
        if (itemNameText != null) itemNameText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = "Selecciona un item para ver información";
        if (itemQuantityText != null) itemQuantityText.text = "";
        if (itemPreviewImage != null) itemPreviewImage.enabled = false;
    }
    
    /// <summary>
    /// Usa el item seleccionado
    /// </summary>
    private void UsarItemSeleccionado()
    {
        if (inventory == null || cientifico == null) return;
        
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new List<KeyValuePair<int, int>>(todosLosItems);
        
        if (slotSeleccionado < itemsArray.Count)
        {
            int itemId = itemsArray[slotSeleccionado].Key;
            Item itemSO = ObtenerItemPorId(itemId);
            
            if (itemSO != null)
            {
                bool usado = itemSO.Usar(cientifico);
                if (usado)
                {
                    inventory.RemoverItem(itemId, 1);
                    ActualizarInventario();
                    Debug.Log($"Usado: {itemSO.nombre}");
                }
            }
            else
            {
                Debug.LogWarning($"No se puede usar item desconocido ID: {itemId}");
            }
        }
    }
    
    /// <summary>
    /// Tira el item seleccionado (lo elimina del inventario)
    /// </summary>
    private void TirarItemSeleccionado()
    {
        if (inventory == null) return;
        
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new List<KeyValuePair<int, int>>(todosLosItems);
        
        if (slotSeleccionado < itemsArray.Count)
        {
            int itemId = itemsArray[slotSeleccionado].Key;
            inventory.RemoverItem(itemId, 1);
            ActualizarInventario();
            
            Debug.Log($"Tiraste item ID: {itemId}");
        }
    }
    
    /// <summary>
    /// Obtiene un item por su ID
    /// </summary>
    private Item ObtenerItemPorId(int id)
    {
        return itemsCache.ContainsKey(id) ? itemsCache[id] : null;
    }
}