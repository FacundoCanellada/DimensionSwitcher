using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Estadísticas del Enemigo")]
    public int saludEnemy = 100;
    [SerializeField] private int saludMaxima; // Guardamos la salud original del prefab
    public int daño = 10;
    public float rangoDeteccion = 10f;
    public float velocidad = 5f;
    
    [Header("Sistema de Drops")]
    public ItemType dropType = ItemType.Food;
    [Range(0f, 1f)]
    public float dropChance = 1f;
    public int dropMin = 1;
    public int dropMax = 1;
    public Item[] posibleDrops; // Array de items posibles que puede dropear
    
    // Referencias internas
    private Cientifico target;
    private GameManager gameManager;
    private QuestManager questManager;
    
    // Control de ataques
    private float tiempoUltimoAtaque = 0f;
    public float tiempoEntreAtaques = 1f;
    
    // Estado del enemigo
    private Vector3 posicionInicial;
    private bool estaPatrullando = true;

    void Start()
    {
        // Guardar la salud máxima del prefab al inicio
        saludMaxima = saludEnemy;
        
        // Buscar referencias
        gameManager = FindFirstObjectByType<GameManager>();
        target = FindFirstObjectByType<Cientifico>();
        questManager = FindFirstObjectByType<QuestManager>();
        
        // Guardar posición inicial para patrullaje
        posicionInicial = transform.position;
        
        // Asegurar que tiene el tag correcto para el sistema de dimensiones
        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }
        
        Debug.Log($"Enemigo iniciado con {saludEnemy} de vida (máxima: {saludMaxima})");
    }

    void Update()
    {
        if (target == null) return;
        
        // ARREGLADO: Solo actuar si está en la misma dimensión que el jugador
        if (!EstaEnMismaDimensionQueJugador()) return;
        
        float distanciaAlJugador = Vector3.Distance(transform.position, target.transform.position);
        
        // Comportamiento basado en distancia
        if (distanciaAlJugador < rangoDeteccion)
        {
            // Perseguir al jugador
            Perseguir();
            estaPatrullando = false;
        }
        else
        {
            // Patrullar o volver a posición inicial
            if (!estaPatrullando)
            {
                Patrullar();
            }
        }
        
        // Atacar si está cerca
        if (distanciaAlJugador < 2f)
        {
            Atacar();
        }
    }
    
    /// <summary>
    /// Persigue al jugador
    /// </summary>
    private void Perseguir()
    {
        Vector3 direccion = (target.transform.position - transform.position).normalized;
        transform.position += direccion * velocidad * Time.deltaTime;
    }
    
    /// <summary>
    /// Patrulla o vuelve a la posición inicial
    /// </summary>
    private void Patrullar()
    {
        float distanciaAInicio = Vector3.Distance(transform.position, posicionInicial);
        
        if (distanciaAInicio > 0.5f)
        {
            // Volver a posición inicial
            Vector3 direccion = (posicionInicial - transform.position).normalized;
            transform.position += direccion * (velocidad * 0.5f) * Time.deltaTime;
        }
        else
        {
            estaPatrullando = true;
        }
    }
    
    /// <summary>
    /// Ataca al jugador
    /// </summary>
    private void Atacar()
    {
        if (Time.time - tiempoUltimoAtaque > tiempoEntreAtaques)
        {
            if (target != null && gameManager != null)
            {
                target.RecibirDanio(daño, gameManager);
                tiempoUltimoAtaque = Time.time;
                
                Debug.Log($"Enemigo atacó por {daño} de daño!");
            }
        }
    }

    /// <summary>
    /// Recibe daño y maneja la muerte
    /// </summary>
    public void RecibirDanio(int cantidad)
    {
        saludEnemy -= cantidad;
        Debug.Log($"Enemigo recibió {cantidad} de daño. Salud restante: {saludEnemy}");
        
        if (saludEnemy <= 0)
        {
            Morir();
        }
    }
    
    /// <summary>
    /// Maneja la muerte del enemigo y el drop de items
    /// </summary>
    private void Morir()
    {
        Debug.Log("Enemigo eliminado!");
        
        // Intentar dropear item
        DropearItem();
        
        // Destruir el enemigo
        Destroy(gameObject);
    }

    /// <summary>
    /// Sistema de drops mejorado
    /// </summary>
    private void DropearItem()
    {
        // Verificar probabilidad de drop
        if (Random.Range(0f, 1f) > dropChance)
        {
            Debug.Log("No se dropeó ningún item.");
            return;
        }
        
        // Seleccionar item para dropear
        Item itemADropear = SeleccionarItemDrop();
        if (itemADropear == null) return;
        
        // Calcular cantidad a dropear
        int cantidad = Random.Range(dropMin, dropMax + 1);
        
        // Agregar al inventario del jugador directamente (sistema simplificado)
        if (target != null && target.GetComponent<Inventory>() != null)
        {
            Inventory inventarioJugador = target.GetComponent<Inventory>();
            bool agregado = inventarioJugador.AgregarItem(itemADropear.id, cantidad);
            
            if (agregado)
            {
                Debug.Log($"Dropeado: {itemADropear.nombre} x{cantidad}");
                
                // Notificar al QuestManager si es un componente
                if (itemADropear.type == ItemType.Components && questManager != null)
                {
                    questManager.OnItemRecogido(itemADropear.id);
                }
            }
            else
            {
                Debug.Log("Inventario lleno! Item perdido.");
            }
        }
    }
    
    /// <summary>
    /// Selecciona qué item dropear basado en el tipo configurado
    /// </summary>
    private Item SeleccionarItemDrop()
    {
        if (posibleDrops == null || posibleDrops.Length == 0)
        {
            Debug.LogWarning("No hay items configurados para dropear en este enemigo.");
            return null;
        }
        
        // Filtrar items por tipo si está especificado
        var itemsDelTipo = System.Array.FindAll(posibleDrops, item => 
            item != null && item.type == dropType);
        
        if (itemsDelTipo.Length == 0)
        {
            // Si no hay items del tipo específico, usar cualquier item válido
            var itemsValidos = System.Array.FindAll(posibleDrops, item => item != null);
            if (itemsValidos.Length > 0)
            {
                return itemsValidos[Random.Range(0, itemsValidos.Length)];
            }
            return null;
        }
        
        // Seleccionar aleatoriamente entre los items del tipo correcto
        return itemsDelTipo[Random.Range(0, itemsDelTipo.Length)];
    }
    
    /// <summary>
    /// Resetea el enemigo a su estado inicial
    /// </summary>
    public void Resetear()
    {
        saludEnemy = 100;
        transform.position = posicionInicial;
        estaPatrullando = true;
        tiempoUltimoAtaque = 0f;
        
        // Re-habilitar componentes si fueron desactivados
        enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }
    
    /// <summary>
    /// Resetea la salud del enemigo a su valor original del prefab
    /// </summary>
    public void ResetearSalud()
    {
        saludEnemy = saludMaxima;
        Debug.Log($"Salud del enemigo reseteada a {saludEnemy}");
    }
    
    /// <summary>
    /// Obtiene la salud máxima configurada en el prefab
    /// </summary>
    public int GetSaludMaxima()
    {
        return saludMaxima;
    }
    
    /// <summary>
    /// Verifica si el enemigo está en la misma dimensión que el jugador
    /// </summary>
    private bool EstaEnMismaDimensionQueJugador()
    {
        // El enemigo está en Dim_Altered, el jugador puede estar en Dim_Normal o Dim_Altered
        // Solo pueden interactuar si ambos están en Dim_Altered
        if (gameObject.layer == LayerMask.NameToLayer("Dim_Altered"))
        {
            // El jugador debe estar también en Dim_Altered para que puedan interactuar
            DimensionSwitcher dimensionSwitcher = FindFirstObjectByType<DimensionSwitcher>();
            if (dimensionSwitcher != null)
            {
                return dimensionSwitcher.dimensionActual == true; // true = dimensión alterada
            }
        }
        return false; // Por defecto no interactúan
    }
    
    /// <summary>
    /// Dibuja el rango de detección en el editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}