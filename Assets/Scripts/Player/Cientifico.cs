using UnityEngine;
using System.Collections;

public class Cientifico : MonoBehaviour
{
    [Header("Estadísticas del Jugador")]
    public int salud = 100;
    public int stamina = 100;
    private float staminaReal = 100f; // valor interno para regeneración suave
    public int hambre = 100;
    public int sed = 100;
    
    [Header("Sistema de Inventario")]
    public Inventory inventory;
    public Item arma;
    
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f;
    public float rangoAtaque = 2f;
    
    [Header("Referencias Visuales")]
    public SpriteRenderer armaRenderer;
    
    [Header("Sistema de Animaciones")]
    public PlayerAnimationControllerSimple animationController;
    
    [Header("Sistema de Necesidades")]
    public float tiempoReduccionNecesidades = 30f; // Cada 30 segundos
    public int cantidadReduccionHambre = 5;
    public int cantidadReduccionSed = 8;
    public int cantidadReduccionStamina = 3;
    
    [Header("Degradación Gradual")]
    public float velocidadDegradacionHambre = 0.5f; // Hambre por segundo (más lenta y gradual)
    public float velocidadDegradacionSed = 0.8f; // Sed por segundo (un poco más rápido que hambre)
    [Tooltip("Intervalo (segundos) entre aplicación de daño por inanición/sed")]
    public float intervaloDanioNecesidades = 2.5f;
    [Tooltip("Daño por tick cuando hambre o sed son 0" )]
    public int danioPorNecesidades = 3;
    
    [Header("Recuperación de Stamina")]
    public float velocidadRecuperacionStamina = 25f; // Stamina por segundo en reposo
    public float velocidadRecuperacionStaminaMovimiento = 10f; // Stamina por segundo moviéndose
    
    // Variables internas
    private bool controlHabilitado = true;
    private Vector3 posicionInicial;
    private Item armaInicial;
    
    // Sistema de necesidades
    private float ultimaReduccionNecesidades = 0f;
    // Acumuladores flotantes reales para degradación gradual
    private float acumuladorHambre = 0f;
    private float acumuladorSed = 0f;
    private float timerDanioNecesidades = 0f;
    
    // Referencias a otros sistemas
    private QuestManager questManager;
    
    // Configuración de inventario UI
    private bool inventarioAbierto = false;
    private int slotSeleccionado = 0;

    void Start()
    {
        // Configuración inicial
        posicionInicial = transform.position;
        armaInicial = arma;
        
        // Buscar componentes necesarios
        if (inventory == null)
            inventory = GetComponent<Inventory>();
        
        if (questManager == null)
            questManager = FindFirstObjectByType<QuestManager>();
            
        // Buscar controlador de animaciones
        if (animationController == null)
            animationController = GetComponent<PlayerAnimationControllerSimple>();
            
        if (animationController != null)
        {
            // Suscribirse al evento de finalización de ataque
            animationController.OnAttackComplete += OnAttackAnimationComplete;
        }
        
        // Inicializar sistema de necesidades
        ultimaReduccionNecesidades = Time.time;
        
        Debug.Log("Científico inicializado");
    }

    void Update()
    {
        if (!controlHabilitado) return;
        
        // Manejar movimiento
        ManejarMovimiento();
        
        // Manejar combate
        ManejarCombate();
        
        // Manejar inventario
        ManejarInventario();
        
        // Sistema de necesidades (solo chequeamos efectos de salud, no reducimos)
        // ActualizarNecesidades(); // DESACTIVADO - causaba doble degradación
        
        // Solo degradación gradual de hambre y sed (más realista)
        ActualizarDegradacionGradual();
        
        // Comprobar efectos de hambre/sed baja en la salud
        ComprobarEfectosSalud();
        
        // Actualizar visualización del arma
        ActualizarVisualizacionArma();
    }
    
    /// <summary>
    /// Maneja el movimiento del jugador
    /// </summary>
    private void ManejarMovimiento()
    {
        // No permitir movimiento si está atacando
        if (animationController != null && animationController.IsAttacking())
            return;
            
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3(h, v, 0).normalized;
        Vector2 movimiento2D = new Vector2(h, v);
        
        // Actualizar animaciones - SÚPER SIMPLE
        if (animationController != null)
        {
            animationController.UpdateMovement(movimiento2D);
        }
        
        // Aplicar movimiento
        transform.position += direccion * velocidad * Time.deltaTime;
        
        // Sistema de stamina suave usando float interno
        if (direccion.magnitude > 0.1f)
        {
            // Consumo de stamina al moverse
            staminaReal -= 2f * Time.deltaTime; // 2 por segundo
            if (staminaReal < 0f) staminaReal = 0f;
            // Recuperación parcial mientras se mueve
            staminaReal += velocidadRecuperacionStaminaMovimiento * Time.deltaTime;
        }
        else
        {
            // Recuperación completa cuando está quieto
            staminaReal += velocidadRecuperacionStamina * Time.deltaTime;
        }
        if (staminaReal > 100f) staminaReal = 100f;
        stamina = Mathf.RoundToInt(staminaReal);
    }
    
    /// <summary>
    /// Maneja el sistema de combate
    /// </summary>
    private void ManejarCombate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && arma != null && arma.type == ItemType.Weapon && staminaReal >= 10f)
        {
            // Verificar que no esté ya atacando
            if (animationController == null || !animationController.IsAttacking())
            {
                IniciarAtaque();
            }
        }
    }
    
    /// <summary>
    /// Maneja la navegación del inventario
    /// </summary>
    private void ManejarInventario()
    {
        // Abrir/cerrar inventario
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventarioAbierto = !inventarioAbierto;
            Debug.Log($"Inventario {(inventarioAbierto ? "abierto" : "cerrado")}");
        }
        
        if (inventarioAbierto && inventory != null)
        {
            var todosLosItems = inventory.GetTodosLosItems();
            int totalItems = todosLosItems.Count;
            
            if (totalItems > 0)
            {
                // Navegar entre items con flechas
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    slotSeleccionado = (slotSeleccionado - 1 + totalItems) % totalItems;
                    MostrarItemSeleccionado();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    slotSeleccionado = (slotSeleccionado + 1) % totalItems;
                    MostrarItemSeleccionado();
                }
                
                // Usar item con Enter
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    UsarItemSeleccionado();
                }
            }
        }
        
        // Usar items rápidamente con números (1-9)
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                ConsumirItemRapido(i);
            }
        }
    }
    
    /// <summary>
    /// Ataque mejorado con consumo de stamina
    /// </summary>
    private void Atacar()
    {
        if (staminaReal < 10f)
        {
            Debug.Log("No tienes suficiente stamina para atacar!");
            return;
        }
        staminaReal -= 10f;
        if (staminaReal < 0f) staminaReal = 0f;
        stamina = Mathf.RoundToInt(staminaReal);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoAtaque);
        bool ataqueExitoso = false;
        
        foreach (var hit in hits)
        {
            Enemy enemigo = hit.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(arma.weaponDamage);
                ataqueExitoso = true;
                Debug.Log($"Atacaste al enemigo por {arma.weaponDamage} de daño!");
            }
        }
        
        if (!ataqueExitoso)
        {
            Debug.Log("No hay enemigos en rango de ataque.");
        }
    }
    
    /// <summary>
    /// Inicia la secuencia de ataque con animaciones
    /// </summary>
    private void IniciarAtaque()
    {
        if (staminaReal < 10f)
        {
            Debug.Log("No tienes suficiente stamina para atacar!");
            return;
        }
        
        // Consumir stamina
        staminaReal -= 10f;
        if (staminaReal < 0f) staminaReal = 0f;
        stamina = Mathf.RoundToInt(staminaReal);
        
        // Ejecutar el daño inmediatamente
        EjecutarAtaque();
        
        // Iniciar animación de ataque
        if (animationController != null)
        {
            animationController.StartAttack();
        }
    }
    
    /// <summary>
    /// Ejecuta el daño del ataque (llamado cuando la animación alcanza el frame adecuado)
    /// </summary>
    private void EjecutarAtaque()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoAtaque);
        bool ataqueExitoso = false;
        
        foreach (var hit in hits)
        {
            Enemy enemigo = hit.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(arma.weaponDamage);
                ataqueExitoso = true;
                Debug.Log($"Atacaste al enemigo por {arma.weaponDamage} de daño!");
            }
        }
        
        if (!ataqueExitoso)
        {
            Debug.Log("No hay enemigos en rango de ataque.");
        }
    }
    
    /// <summary>
    /// Llamado cuando la animación de ataque se completa
    /// </summary>
    private void OnAttackAnimationComplete()
    {
        // Aquí puedes agregar efectos adicionales cuando termine el ataque
        Debug.Log("Ataque completado");
    }
    
    /// <summary>
    /// Actualiza el sistema de necesidades básicas (sistema original mantenido)
    /// </summary>
    private void ActualizarNecesidades()
    {
        if (Time.time - ultimaReduccionNecesidades >= tiempoReduccionNecesidades)
        {
            // Reducir hambre y sed con el tiempo
            hambre = Mathf.Max(0, hambre - cantidadReduccionHambre);
            sed = Mathf.Max(0, sed - cantidadReduccionSed);
            
            ultimaReduccionNecesidades = Time.time;
            
            // Avisos cuando las necesidades están bajas
            if (hambre <= 20)
                Debug.Log("¡Tienes mucha hambre! Busca comida.");
            
            if (sed <= 20)
                Debug.Log("¡Tienes mucha sed! Busca agua.");
            
            // Efectos de hambre/sed baja en la salud
            if (hambre <= 0 || sed <= 0)
            {
                salud = Mathf.Max(0, salud - 5);
                Debug.Log("¡Tu salud disminuye por falta de cuidados básicos!");
            }
        }
    }
    
    /// <summary>
    /// Degradación gradual adicional de hambre y sed
    /// </summary>
    private void ActualizarDegradacionGradual()
    {
        // Usar acumuladores flotantes para no perder precisión
        acumuladorHambre += velocidadDegradacionHambre * Time.deltaTime;
        acumuladorSed += velocidadDegradacionSed * Time.deltaTime;

        if (acumuladorHambre >= 1f)
        {
            int puntos = Mathf.FloorToInt(acumuladorHambre);
            if (puntos > 0)
            {
                hambre = Mathf.Max(0, hambre - puntos);
                acumuladorHambre -= puntos;
            }
        }

        if (acumuladorSed >= 1f)
        {
            int puntos = Mathf.FloorToInt(acumuladorSed);
            if (puntos > 0)
            {
                sed = Mathf.Max(0, sed - puntos);
                acumuladorSed -= puntos;
            }
        }
    }
    
    /// <summary>
    /// Comprueba efectos de hambre y sed en la salud
    /// </summary>
    private void ComprobarEfectosSalud()
    {
        // Avisos periódicos cada 5s usando timerDanioNecesidades como referencia separada
        // (No usar Time.time % n para evitar saltos al pausar/reanudar)
        // Daño acumulado cuando alguna necesidad está a 0
        if (hambre <= 20 || sed <= 20)
        {
            // Mensajes simples (podríamos throttlear si hace falta)
            if (hambre == 20 || sed == 20)
                Debug.Log("Advertencia: Necesidades bajas (hambre o sed < 20)");
        }

        if (hambre <= 0 || sed <= 0)
        {
            timerDanioNecesidades += Time.unscaledDeltaTime; // Independiente de pausas por inventario
            if (timerDanioNecesidades >= intervaloDanioNecesidades)
            {
                salud = Mathf.Max(0, salud - danioPorNecesidades);
                timerDanioNecesidades = 0f;
                Debug.Log($"Daño por necesidades críticas: -{danioPorNecesidades}. Salud:{salud}");
            }
        }
        else
        {
            // Resetear timer si ya no estamos en estado crítico
            timerDanioNecesidades = 0f;
        }
    }
    
    /// <summary>
    /// Actualiza la visualización del arma
    /// </summary>
    private void ActualizarVisualizacionArma()
    {
        if (armaRenderer != null)
        {
            if (arma != null && arma.icon != null)
            {
                armaRenderer.sprite = arma.icon;
                armaRenderer.enabled = true;
            }
            else
            {
                armaRenderer.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Muestra información del item seleccionado en el inventario
    /// </summary>
    private void MostrarItemSeleccionado()
    {
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>(todosLosItems);
        
        if (slotSeleccionado < itemsArray.Count)
        {
            var itemActual = itemsArray[slotSeleccionado];
            Debug.Log($"Item seleccionado: ID {itemActual.Key} x{itemActual.Value}");
        }
    }
    
    /// <summary>
    /// Usa el item seleccionado actualmente
    /// </summary>
    private void UsarItemSeleccionado()
    {
        var todosLosItems = inventory.GetTodosLosItems();
        var itemsArray = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>(todosLosItems);
        
        if (slotSeleccionado < itemsArray.Count)
        {
            int itemId = itemsArray[slotSeleccionado].Key;
            ConsumirItem(itemId);
        }
    }
    
    /// <summary>
    /// Consume un item específico por ID
    /// </summary>
    public bool ConsumirItem(int itemId)
    {
        if (inventory != null && inventory.CheckItem(itemId))
        {
            // Aquí necesitaríamos una referencia a los ScriptableObjects de items
            // Por ahora, aplicamos efectos básicos basados en el ID
            AplicarEfectoItem(itemId);
            
            inventory.RemoverItem(itemId, 1);
            Debug.Log($"Consumiste item {itemId}");
            return true;
        }
        
        Debug.Log($"No tienes item {itemId} en el inventario");
        return false;
    }
    
    /// <summary>
    /// Consumo rápido de items con teclas numéricas
    /// </summary>
    private void ConsumirItemRapido(int slot)
    {
        // IDs rápidos para consumibles comunes
        int[] idsRapidos = { 10, 11, 12, 13, 14, 15, 16, 17, 18 }; // Comida y agua
        
        if (slot <= idsRapidos.Length)
        {
            ConsumirItem(idsRapidos[slot - 1]);
        }
    }
    
    /// <summary>
    /// Aplica efectos básicos de items (versión simplificada)
    /// </summary>
    private void AplicarEfectoItem(int itemId)
    {
        Debug.Log($"🍎 AplicarEfectoItem llamado con ID: {itemId}");
        Debug.Log($"📊 ANTES: Salud:{salud}, Hambre:{hambre}, Sed:{sed}");
        
        // Items de comida (IDs 10-15) - Curan HAMBRE + SALUD
        if (itemId >= 10 && itemId <= 15)
        {
            int nuevaHambre = Mathf.Min(100, hambre + 25);
            int nuevaSalud = Mathf.Min(100, salud + 15);
            hambre = nuevaHambre;
            salud = nuevaSalud;
            Debug.Log($"🍞 Comida consumida: Hambre {hambre} (+25), Salud {salud} (+15)");
        }
        // Items de agua (IDs 16-20) - Curan SED + SALUD
        else if (itemId >= 16 && itemId <= 20)
        {
            int nuevaSed = Mathf.Min(100, sed + 30);
            int nuevaSalud = Mathf.Min(100, salud + 10);
            sed = nuevaSed;
            salud = nuevaSalud;
            Debug.Log($"💧 Agua consumida: Sed {sed} (+30), Salud {salud} (+10)");
        }
        // Items de salud (IDs 21-25) - Curan SALUD + un poco de HAMBRE y SED
        else if (itemId >= 21 && itemId <= 25)
        {
            int nuevaSalud = Mathf.Min(100, salud + 30);
            int nuevaHambre = Mathf.Min(100, hambre + 10);
            int nuevaSed = Mathf.Min(100, sed + 10);
            salud = nuevaSalud;
            hambre = nuevaHambre;
            sed = nuevaSed;
            Debug.Log($"💊 Medicina usada: Salud {salud} (+30), Hambre {hambre} (+10), Sed {sed} (+10)");
        }
        else
        {
            Debug.LogWarning($"⚠️ Item ID {itemId} no reconocido - sin efectos aplicados");
        }
        
        Debug.Log($"📊 DESPUÉS: Salud:{salud}, Hambre:{hambre}, Sed:{sed}");
    }

    /// <summary>
    /// Recibe daño y maneja efectos
    /// </summary>
    public void RecibirDanio(int cantidad, GameManager gameManager)
    {
        salud -= cantidad;
        if (salud < 0) salud = 0;
        
        Debug.Log($"Recibiste {cantidad} de daño. Salud actual: {salud}");
        
        if (gameManager != null)
            gameManager.ComprobarDerrota();
    }
    
    /// <summary>
    /// Maneja la interacción con objetos (legacy)
    /// </summary>
    public void AgarrarItem(Item item)
    {
        if (inventory != null)
        {
            inventory.AgregarItem(item.id, 1);
            
            // Notificar al quest manager si es un componente
            if (item.type == ItemType.Components && questManager != null)
            {
                questManager.OnItemRecogido(item.id);
            }
        }
    }

    /// <summary>
    /// Resetea el científico al estado inicial COMPLETO (solo al iniciar partida nueva)
    /// </summary>
    public void Resetear()
    {
        // Solo resetear stats al inicio del juego
        salud = 100;
        stamina = 100;
        hambre = 100;
        sed = 100;
        
        if (inventory != null)
        {
            inventory.Limpiar();
            
            // TEMPORAL: Agregar items de prueba para testing
            inventory.AgregarItem(1, 1); // Componente 1
            inventory.AgregarItem(10, 3); // Pan x3
            inventory.AgregarItem(20, 2); // Agua x2
            Debug.Log("Items de prueba agregados al inventario");
        }
        
        arma = armaInicial;
        transform.position = posicionInicial;
        controlHabilitado = true;
        inventarioAbierto = false;
        
        Debug.Log("Científico COMPLETAMENTE reseteado (stats + inventario + posición)");
    }
    
    /// <summary>
    /// Resetea solo la posición sin tocar stats ni inventario (para respawn tras muerte)
    /// </summary>
    public void ResetearPosicion()
    {
        transform.position = posicionInicial;
        controlHabilitado = true;
        inventarioAbierto = false;
        
        // Solo resetear salud en caso de muerte
        if (salud <= 0)
        {
            salud = 100;
            Debug.Log("Salud restaurada tras muerte");
        }
        
        Debug.Log("Posición reseteada (manteniendo stats e inventario)");
    }

    /// <summary>
    /// Habilita o deshabilita el control del jugador
    /// </summary>
    public void HabilitarControl(bool habilitado)
    {
        controlHabilitado = habilitado;
        
        if (!habilitado)
        {
            inventarioAbierto = false;
            if (armaRenderer != null)
                armaRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// Dibuja gizmos para debug
    /// </summary>
    void OnDestroy()
    {
        // Desuscribirse de eventos para evitar errores
        if (animationController != null)
        {
            animationController.OnAttackComplete -= OnAttackAnimationComplete;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}