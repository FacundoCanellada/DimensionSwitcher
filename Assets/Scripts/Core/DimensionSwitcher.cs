using UnityEngine;
using System.Collections;

public class DimensionSwitcher : MonoBehaviour
{
    [Header("Configuración de Dimensiones")]
    public string normalMask = "Dim_Normal";
    public string alteredMask = "Dim_Altered";
    
    [Header("Estado del Sistema")]
    public bool desbloqueado = false;
    public bool dimensionActual = false; // false = Normal, true = Alterada

    [Header("Configuración de Cooldown")]
    public float cooldown = 5f;
    public KeyCode toggleKey = KeyCode.Tab;
    
    [Header("Referencias")]
    public QuestManager questManager;
    
    // Variables internas
    private float ultimoCambio = 0f;
    private bool puedeUsarse = true;
    
    // Cache de layers
    private int normalLayerMask;
    private int alteredLayerMask;
    
    void Start()
    {
        // Convertir los nombres de capas a layer masks
        normalLayerMask = LayerMask.NameToLayer(normalMask);
        alteredLayerMask = LayerMask.NameToLayer(alteredMask);
        
        // Verificar que las capas existen
        if (normalLayerMask == -1)
        {
            Debug.LogError($"Capa '{normalMask}' no encontrada! Agrega esta capa en Layer Settings.");
            Debug.LogError("Ve a Edit > Project Settings > Tags and Layers y agrega 'Dim_Normal' en Layer 8");
        }
        else
        {
            Debug.Log($"Capa '{normalMask}' encontrada en Layer {normalLayerMask}");
        }
        
        if (alteredLayerMask == -1)
        {
            Debug.LogError($"Capa '{alteredMask}' no encontrada! Agrega esta capa en Layer Settings.");
            Debug.LogError("Ve a Edit > Project Settings > Tags and Layers y agrega 'Dim_Altered' en Layer 9");
        }
        else
        {
            Debug.Log($"Capa '{alteredMask}' encontrada en Layer {alteredLayerMask}");
        }
        
        // Si no se asigna quest manager, buscar uno
        if (questManager == null)
        {
            questManager = FindFirstObjectByType<QuestManager>();
        }
        
        // Sistema desbloqueado por defecto
        desbloqueado = true;
        Debug.Log("DimensionSwitcher desbloqueado por defecto");
        
        // Inicializar en dimensión NORMAL (como debe ser)
        dimensionActual = false; // false = normal
        SetearDimension(false); // Mostrar estabilizador por defecto
        
        Debug.Log("DimensionSwitcher inicializado - Use Tab para cambiar dimensiones");
    }
    
    void Update()
    {
        // Verificar si se debe desbloquear el sistema
        VerificarDesbloqueo();
        
        // Manejar input del usuario
        if (Input.GetKeyDown(toggleKey))
        {
            Debug.Log("Tab presionado - intentando cambiar dimensión");
            bool exito = Cambiar();
            Debug.Log($"Cambio de dimensión exitoso: {exito}");
        }
        
        // Actualizar cooldown visual
        ActualizarCooldown();
    }
    
    /// <summary>
    /// Cambia entre dimensión normal y alterada
    /// </summary>
    public bool Cambiar()
    {
        // Verificar prerrequisitos
        if (!desbloqueado)
        {
            Debug.Log("Sistema de dimensiones bloqueado. Encuentra el primer componente para desbloquearlo.");
            return false;
        }
        
        if (!puedeUsarse)
        {
            float tiempoRestante = cooldown - (Time.time - ultimoCambio);
            Debug.Log($"Sistema de dimensiones en cooldown. Espera {tiempoRestante:F1} segundos.");
            return false;
        }
        
        // Cambiar dimensión
        dimensionActual = !dimensionActual;
        SetearDimension(dimensionActual);
        
        // Iniciar cooldown
        ultimoCambio = Time.time;
        puedeUsarse = false;
        StartCoroutine(CooldownCoroutine());
        
        string nuevaDimension = dimensionActual ? "ALTERADA" : "NORMAL";
        Debug.Log($"¡Cambiaste a dimensión {nuevaDimension}!");
        
        return true;
    }
    
    /// <summary>
    /// Establece la dimensión activa cambiando las capas de los objetos
    /// </summary>
    private void SetearDimension(bool esAlterada)
    {
        if (esAlterada)
        {
            // Dimensión ALTERADA: Mostrar solo objetos en Dim_Altered
            CambiarVisibilidadPorLayer("Enemy", alteredLayerMask, true);  // Mostrar enemigos de Dim_Altered
            CambiarVisibilidadPorLayer("Enemy", normalLayerMask, false);   // Ocultar enemigos de Dim_Normal
            CambiarVisibilidadDimension("Estabilizador", false);
            CambiarVisibilidadDimension("Item", false);
            Debug.Log("DIMENSIÓN ALTERADA: Enemigos de Dim_Altered visibles, resto oculto");
        }
        else
        {
            // Dimensión NORMAL: Mostrar solo objetos en Dim_Normal
            CambiarVisibilidadPorLayer("Enemy", normalLayerMask, true);    // Mostrar enemigos de Dim_Normal
            CambiarVisibilidadPorLayer("Enemy", alteredLayerMask, false);  // Ocultar enemigos de Dim_Altered
            CambiarVisibilidadDimension("Estabilizador", true);
            CambiarVisibilidadDimension("Item", true);
            Debug.Log("DIMENSIÓN NORMAL: Enemigos de Dim_Normal visibles, resto oculto");
        }
    }
    
    /// <summary>
    /// Cambia la visibilidad de objetos con un tag Y layer específicos
    /// </summary>
    private void CambiarVisibilidadPorLayer(string tag, int layer, bool visible)
    {
        GameObject[] objetos = GameObject.FindGameObjectsWithTag(tag);
        int contador = 0;
        
        foreach (GameObject obj in objetos)
        {
            // Solo afectar objetos en el layer especificado
            if (obj.layer != layer) continue;
            
            contador++;
            
            // Cambiar la visibilidad del objeto manteniendo el GameObject activo
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
            
            // Para sprites usar SpriteRenderer
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }
            
            // Desactiva colliders para que no interactúen cuando están invisibles
            Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = visible;
            }
            
            // Desactiva componentes Enemy para que no ataquen cuando están invisibles
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.enabled = visible;
            }
        }
        
        Debug.Log($"Configurados {contador} objetos con tag '{tag}' en layer {LayerMask.LayerToName(layer)} como {(visible ? "VISIBLES" : "INVISIBLES")}");
    }
    
    /// <summary>
    /// Cambia la visibilidad de objetos con un tag específico
    /// </summary>
    private void CambiarVisibilidadDimension(string tag, bool visible)
    {
        GameObject[] objetos = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log($"Encontrados {objetos.Length} objetos con tag '{tag}', configurándolos como {(visible ? "VISIBLES" : "INVISIBLES")}");
        
        foreach (GameObject obj in objetos)
        {
            // Cambiar la visibilidad del objeto manteniendo el GameObject activo
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = visible;
                Debug.Log($"Renderer de {obj.name} configurado como {visible}");
            }
            
            // Para sprites usar SpriteRenderer
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
                Debug.Log($"SpriteRenderer de {obj.name} configurado como {visible}");
            }
            
            // Desactiva colliders para que no interactúen cuando están invisibles
            Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = visible;
            }
            
            // Desactiva componentes Enemy para que no ataquen cuando están invisibles
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.enabled = visible;
                Debug.Log($"Enemy script de {obj.name} configurado como {visible}");
            }
            
            Debug.Log($"Objeto {obj.name} configurado: visible={visible}");
        }
    }
    
    /// <summary>
    /// Verifica si el sistema debe desbloquearse
    /// </summary>
    private void VerificarDesbloqueo()
    {
        if (!desbloqueado && questManager != null)
        {
            // Desbloquear cuando se encuentra el primer componente
            if (questManager.GetComponentesRecolectados() > 0)
            {
                desbloqueado = true;
                Debug.Log("¡Sistema de dimensiones DESBLOQUEADO! Usa Tab para cambiar dimensiones.");
            }
        }
    }
    
    /// <summary>
    /// Actualiza el estado del cooldown
    /// </summary>
    private void ActualizarCooldown()
    {
        if (!puedeUsarse && Time.time - ultimoCambio >= cooldown)
        {
            puedeUsarse = true;
        }
    }
    
    /// <summary>
    /// Corrutina para manejar el cooldown
    /// </summary>
    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldown);
        puedeUsarse = true;
        Debug.Log("Sistema de dimensiones listo para usar nuevamente.");
    }
    
    /// <summary>
    /// Obtiene el tiempo restante de cooldown
    /// </summary>
    public float GetTiempoRestanteCooldown()
    {
        if (puedeUsarse) return 0f;
        return Mathf.Max(0f, cooldown - (Time.time - ultimoCambio));
    }
    
    /// <summary>
    /// Obtiene si el sistema está desbloqueado
    /// </summary>
    public bool EstaDesbloqueado()
    {
        return desbloqueado;
    }
    
    /// <summary>
    /// Obtiene el nombre de la dimensión actual
    /// </summary>
    public string GetDimensionActual()
    {
        return dimensionActual ? "ALTERADA" : "NORMAL";
    }
    
    /// <summary>
    /// Resetea el sistema al estado inicial
    /// </summary>
    public void Resetear()
    {
        desbloqueado = true; // Sistema desbloqueado por defecto
        dimensionActual = false; // INICIA EN NORMAL (false = normal, true = alterada)
        puedeUsarse = true;
        ultimoCambio = 0f;
        
        SetearDimension(false); // Mostrar estabilizador e items (dimensión normal)
        
        Debug.Log("DimensionSwitcher reseteado - INICIANDO EN DIMENSIÓN NORMAL");
    }
}