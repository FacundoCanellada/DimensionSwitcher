using UnityEngine;

/// <summary>
/// Tipos de enemigos disponibles
/// </summary>
public enum EnemyType
{
    Slime,
    Orc,
    Predator
}

public class Enemy : MonoBehaviour
{
    [Header("Configuración del Tipo")]
    public EnemyTypeData enemyTypeData;
    public EnemyType enemyType = EnemyType.Slime;
    [Range(0, 2)]
    public int spriteVariant = 0; // 0, 1, 2 para los 3 estilos diferentes
    
    [Header("Estadísticas del Enemigo (Auto-configuradas)")]
    public int saludEnemy = 100;
    [SerializeField] private int saludMaxima;
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
    
    [Header("Sistema de Animaciones")]
    public EnemyAnimationController animationController;
    
    // Control de ataques
    private float tiempoUltimoAtaque = 0f;
    public float tiempoEntreAtaques = 1f;
    
    // Estado del enemigo
    private Vector3 posicionInicial;
    private bool estaPatrullando = true;

    void Start()
    {
        // Guardar la salud máxima del inspector ANTES de configurar el tipo
        // (para que no se sobrescriba con el valor del EnemyTypeData)
        saludMaxima = saludEnemy;
        
        // Configurar enemigo según su tipo
        ConfigurarTipoEnemigo();
        
        // Buscar referencias
        gameManager = FindFirstObjectByType<GameManager>();
        target = FindFirstObjectByType<Cientifico>();
        questManager = FindFirstObjectByType<QuestManager>();
        
        // Buscar controlador de animaciones
        if (animationController == null)
            animationController = GetComponent<EnemyAnimationController>();
            
        if (animationController != null)
        {
            animationController.OnAttackComplete += OnAttackAnimationComplete;
            animationController.OnDeathComplete += OnDeathAnimationComplete;
        }
        
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
        
        // Actualizar animaciones de movimiento
        if (animationController != null)
        {
            animationController.UpdateMovement(new Vector2(direccion.x, direccion.y));
        }
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
            
            // Actualizar animaciones de movimiento
            if (animationController != null)
            {
                animationController.UpdateMovement(new Vector2(direccion.x, direccion.y));
            }
        }
        else
        {
            estaPatrullando = true;
            // Animación idle
            if (animationController != null)
            {
                animationController.UpdateMovement(Vector2.zero);
            }
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
                // Calcular dirección hacia el jugador para el ataque
                Vector3 direccionAtaque = (target.transform.position - transform.position).normalized;
                
                // Iniciar animación de ataque
                if (animationController != null && !animationController.IsAttacking())
                {
                    animationController.SetDirection(new Vector2(direccionAtaque.x, direccionAtaque.y));
                    animationController.StartAttack();
                }
                
                // Aplicar daño inmediatamente
                target.RecibirDanio(daño, gameManager);
                tiempoUltimoAtaque = Time.time;}
        }
    }

    /// <summary>
    /// Recibe daño y maneja la muerte
    /// </summary>
    public void RecibirDanio(int cantidad)
    {
        saludEnemy -= cantidad;if (saludEnemy <= 0)
        {
            Morir();
        }
    }
    
    /// <summary>
    /// Maneja la muerte del enemigo y el drop de items
    /// </summary>
    private void Morir()
    {// Iniciar animación de muerte
        if (animationController != null && !animationController.IsDead())
        {
            animationController.StartDeath();
        }
        else
        {
            // Si no hay animación, morir inmediatamente
            CompletarMuerte();
        }
    }
    
    /// <summary>
    /// Completa la muerte del enemigo (llamado cuando termina la animación)
    /// </summary>
    private void CompletarMuerte()
    {
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
        {return;
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
            {// Notificar al QuestManager si es un componente
                if (itemADropear.type == ItemType.Components && questManager != null)
                {
                    questManager.OnItemRecogido(itemADropear.id);
                }
            }
            else
            {}
        }
    }
    
    /// <summary>
    /// Selecciona qué item dropear basado en el tipo configurado
    /// </summary>
    private Item SeleccionarItemDrop()
    {
        if (posibleDrops == null || posibleDrops.Length == 0)
        {return null;
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
        saludEnemy = saludMaxima; // Usar la salud configurada en el inspector
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
        saludEnemy = saludMaxima;}
    
    /// <summary>
    /// Obtiene la salud máxima configurada en el prefab
    /// </summary>
    public int GetSaludMaxima()
    {
        return saludMaxima;
    }
    
    /// <summary>
    /// Verifica si el enemigo está en la misma dimensión que el jugador (YA NO APLICA - SISTEMA SIMPLIFICADO)
    /// </summary>
    private bool EstaEnMismaDimensionQueJugador()
    {
        // Sistema simplificado: todos los enemigos están siempre en la misma dimensión
        return true;
    }
    
    /// <summary>
    /// Verifica si el enemigo puede interactuar basado en dimensión (YA NO APLICA - SISTEMA SIMPLIFICADO)
    /// </summary>
    private bool PuedeInteractuar()
    {
        // Sistema simplificado: todos los enemigos pueden interactuar siempre
        return true;
    }
    
    /// <summary>
    /// Configura el enemigo según su tipo y variante de sprite
    /// </summary>
    /// <summary>
    /// Configura el enemigo con un tipo específico desde EnemyTypeData
    /// </summary>
    public void ConfigurarTipoEnemigo(EnemyTypeData data)
    {
        enemyTypeData = data;
        if (data != null)
        {
            enemyType = data.enemyType;
        }
        ConfigurarTipoEnemigo();
    }
    
    private void ConfigurarTipoEnemigo()
    {
        // Si hay datos de tipo específico, usarlos
        if (enemyTypeData != null)
        {
            saludEnemy = enemyTypeData.health;
            daño = enemyTypeData.damage;
            velocidad = enemyTypeData.speed;
            rangoDeteccion = enemyTypeData.detectionRange;
            tiempoEntreAtaques = enemyTypeData.attackCooldown;
            
            // Aplicar tinte de color si está configurado
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && enemyTypeData.tintColor != Color.white)
            {
                spriteRenderer.color = enemyTypeData.tintColor;
            }
            
            // Configurar animator si está especificado
            Animator animator = GetComponent<Animator>();
            if (animator != null && enemyTypeData.animatorController != null)
            {
                animator.runtimeAnimatorController = enemyTypeData.animatorController;
            }
            
            // Configurar drops
            if (enemyTypeData.possibleDrops != null && enemyTypeData.possibleDrops.Length > 0)
            {
                posibleDrops = enemyTypeData.possibleDrops;
                dropChance = enemyTypeData.dropChance;
                dropMin = enemyTypeData.dropMinAmount;
                dropMax = enemyTypeData.dropMaxAmount;
            }
            
            Debug.Log($"Enemigo configurado como {enemyTypeData.enemyName} (Tipo: {enemyType}, Variante: {spriteVariant})");
        }
        else
        {
            // Configuración por defecto según el tipo
            switch (enemyType)
            {
                case EnemyType.Slime:
                    saludEnemy = 5;
                    daño = 3;
                    velocidad = 2f;
                    break;
                case EnemyType.Orc:
                    saludEnemy = 15;
                    daño = 8;
                    velocidad = 3f;
                    break;
                case EnemyType.Predator:
                    saludEnemy = 20;
                    daño = 12;
                    velocidad = 4f;
                    break;
            }
            Debug.Log($"Enemigo configurado con valores por defecto (Tipo: {enemyType})");
        }
        
        // Actualizar nombre del GameObject
        gameObject.name = $"{enemyType}_{spriteVariant}";
    }
    
    /// <summary>
    /// Callback cuando termina la animación de ataque
    /// </summary>
    private void OnAttackAnimationComplete()
    {
        // Aquí se puede agregar lógica adicional después del ataque
    }
    
    /// <summary>
    /// Callback cuando termina la animación de muerte
    /// </summary>
    private void OnDeathAnimationComplete()
    {
        CompletarMuerte();
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos para evitar errores
        if (animationController != null)
        {
            animationController.OnAttackComplete -= OnAttackAnimationComplete;
            animationController.OnDeathComplete -= OnDeathAnimationComplete;
        }
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