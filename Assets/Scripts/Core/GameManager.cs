using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Referencias Principales")]
    public Cientifico cientifico;
    public EstabilizadorCuantico estabilizador;
    public UIManager uiManager; // Nueva referencia al UIManager
    public QuestManager questManager;
    public DimensionSwitcher dimensionSwitcher;
    
    [Header("Pantallas de Juego (Legacy - para compatibilidad)")]
    public GameObject pantallaVictoria;
    public GameObject pantallaDerrota;

    [Header("Configuración de Enemigos")]
    public bool usarEnemigosPreColocados = true; // Si true, usa enemigos de la escena; si false, spawneará desde prefab
    public GameObject enemigoPrefab; // Prefab base (solo se usa si usarEnemigosPreColocados = false)
    public int cantidadEnemigos = 3;
    public List<Item> componentesDropEnemigos;
    
    [Header("Posiciones de Spawn de Enemigos")]
    [Tooltip("Define aquí las posiciones exactas donde aparecerán los enemigos. Si está vacío, usa posiciones por defecto.")]
    public Vector2[] posicionesSpawnEnemigos = new Vector2[]
    {
        new Vector2(10f, 5f),   // Enemigo 1
        new Vector2(-10f, 5f),  // Enemigo 2
        new Vector2(0f, 10f)    // Enemigo 3
    };
    
    [Header("Tipos de Enemigos")]
    public EnemyTypeData[] tiposEnemigos; // Array con los 3 tipos
    public bool usarTiposAleatorios = true; // Si usar tipos random o en orden
    
    // Información de enemigos pre-colocados guardada al inicio
    private List<EnemySpawnData> enemigosIniciales = new List<EnemySpawnData>();
    
    [System.Serializable]
    private class EnemySpawnData
    {
        public Vector3 posicion;
        public Quaternion rotacion;
        public int layer;
        public EnemyTypeData tipoData;
        public EnemyType tipo;
        public int spriteVariant;
        public Item[] drops;
    }

    [Header("Estado del Juego")]
    private bool juegoTerminado = false;
    private bool juegoIniciado = false; // Para controlar si el juego ha comenzado

    void Awake()
    {
        // Buscar referencias automáticamente si no están asignadas
        BuscarReferenciasAutomaticas();
    }

    void Start()
    {
        Debug.Log("GameManager.Start() iniciado");
        
        // Buscar referencias automáticamente
        BuscarReferenciasAutomaticas();
        
        // Guardar estado inicial de enemigos si están pre-colocados
        if (usarEnemigosPreColocados)
        {
            GuardarEstadoInicialEnemigos();}
        
        // No iniciar automáticamente el juego, esperar por UIManager
        // Si no hay UIManager, usar comportamiento legacy
        if (uiManager == null)
        {// Comportamiento legacy: iniciar directamente
            if (!usarEnemigosPreColocados)
            {
                RespawnearEnemigos();
            }
            juegoIniciado = true;
        }
        else
        {// Nuevo comportamiento: esperar por el UIManager
            juegoIniciado = false;
        }
        
        Debug.Log("GameManager.Start() completado");
    }

    #region Nuevos Métodos para UI System

    /// <summary>
    /// Inicia el juego desde el menú principal
    /// </summary>
    public void IniciarJuego()
    {
        juegoIniciado = true;
        juegoTerminado = false;
        
        // Resetear todo al estado inicial
        if (cientifico != null) cientifico.Resetear();
        if (estabilizador != null) estabilizador.Resetear();
        if (questManager != null) questManager.Resetear();
        if (dimensionSwitcher != null) dimensionSwitcher.Resetear();
        
        // Solo llamar RespawnearEnemigos si NO estamos usando pre-colocados
        // Si usamos pre-colocados, los enemigos ya están en la escena
        if (!usarEnemigosPreColocados)
        {
            RespawnearEnemigos();
        }
        else
        {
            // Si hay enemigos pre-colocados, solo resetearlos
            GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemigo in enemigos)
            {
                Enemy enemyScript = enemigo.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.ResetearSalud();
                    enemyScript.Resetear();
                }
            }}
        
        // Habilitar controles del jugador
        if (cientifico != null) cientifico.HabilitarControl(true);
        
        // Actualizar UI si existe
        if (uiManager != null)
            uiManager.Actualizar(cientifico, estabilizador, dimensionSwitcher);}

    /// <summary>
    /// Resetea completamente el juego para volver al menú
    /// </summary>
    public void ResetearJuegoCompleto()
    {
        juegoIniciado = false;
        juegoTerminado = false;
        
        // Resetear todos los componentes
        if (cientifico != null) cientifico.Resetear();
        if (estabilizador != null) estabilizador.Resetear();
        RespawnearEnemigos();
        
        // Deshabilitar controles hasta que se inicie nuevamente
        if (cientifico != null) cientifico.HabilitarControl(false);}

    #endregion

    #region Métodos Originales (Mejorados)

    public void ComprobarDerrota()
    {
        if (!juegoTerminado && cientifico != null && cientifico.salud <= 0)
        {
            // Solo verificar si el juego ha iniciado (para compatibilidad)
            if (uiManager == null || juegoIniciado)
            {
                TerminarJuego(true); // true = derrota
            }
        }
    }

    public void ComprobarVictoria()
    {
        bool condicionVictoria = false;
        
        // Condición de victoria: estabilizador reparado (tiene los 3 componentes colocados)
        if (estabilizador != null)
        {
            condicionVictoria = estabilizador.reparado;
            
            if (condicionVictoria)
            {
                Debug.Log("¡Condición de victoria cumplida! Estabilizador reparado.");
            }
        }
        
        if (!juegoTerminado && condicionVictoria)
        {
            // Solo verificar si el juego ha iniciado (para compatibilidad)
            if (uiManager == null || juegoIniciado)
            {
                TerminarJuego(false); // false = victoria
            }
        }
    }

    void TerminarJuego(bool esDerrota)
    {
        Debug.Log($"=== TERMINAR JUEGO LLAMADO === esDerrota: {esDerrota}");
        
        juegoTerminado = true;
        
        // Deshabilitar controles del jugador
        if (cientifico != null) cientifico.HabilitarControl(false);
        
        // Usar UIManager si está disponible, sino usar sistema legacy
        if (uiManager != null)
        {
            Debug.Log($"Usando UIManager para mostrar pantalla de {(esDerrota ? "DERROTA" : "VICTORIA")}");
            if (esDerrota)
                uiManager.MostrarPantallaDerrota();
            else
                uiManager.MostrarPantallaVictoria();
        }
        else
        {
            Debug.LogWarning("UIManager es NULL, usando sistema legacy");
            // Sistema legacy
            if (pantallaVictoria != null && pantallaDerrota != null)
            {
                pantallaVictoria.SetActive(!esDerrota);
                pantallaDerrota.SetActive(esDerrota);
            }
            Time.timeScale = 0;
        }
    }

    public void ReiniciarNivel()
    {
        // Reactiva el tiempo y los inputs
        Time.timeScale = 1;
        juegoTerminado = false;
        
        // Usar UIManager para ocultar pantallas si está disponible
        if (uiManager != null)
        {
            uiManager.IniciarJuego(); // Esto oculta pantallas y muestra HUD
        }
        else
        {
            // Solo ocultar pantallas legacy si existen y no hay UIManager
            if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
            if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        }

        // Resetear SOLO posición del jugador (no stats, no inventario) para no perder progreso
        if (cientifico != null) 
        {
            if (cientifico.salud <= 0)
            {
                // Si estaba muerto, restaurar salud pero NO hambre/sed/sed
                cientifico.ResetearPosicion();
                Debug.Log("Científico respawneado (posición + salud si estaba muerto)");
            }
            else
            {
                cientifico.ResetearPosicion();}
        }
        
        if (estabilizador != null) 
        {
            estabilizador.Resetear();
            Debug.Log("=== ESTABILIZADOR RESETEADO ===");
        }
        
        if (questManager != null) 
        {
            questManager.Resetear();
            Debug.Log("=== QUEST MANAGER RESETEADO ===");
        }
        
        // CRÍTICO: Respawnear enemigos ANTES de resetear dimensiónRespawnearEnemigos();
        
        // Verificar inmediatamente cuántos enemigos hay
        GameObject[] enemigosVerificar = GameObject.FindGameObjectsWithTag("Enemy");// ARREGLO: Resetear dimensión DESPUÉS de que los enemigos existan
        if (dimensionSwitcher != null) 
        {
            dimensionSwitcher.Resetear();}// Habilitar controles
        if (cientifico != null) cientifico.HabilitarControl(true);

        // Actualizar UI
        if (uiManager != null)
        {
            uiManager.Actualizar(cientifico, estabilizador, dimensionSwitcher);
        }}

    #endregion

    #region Gestión de Enemigos Pre-colocados

    /// <summary>
    /// Guarda el estado inicial de todos los enemigos en la escena
    /// </summary>
    private void GuardarEstadoInicialEnemigos()
    {
        enemigosIniciales.Clear();
        
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        
        Debug.Log($"=== GUARDANDO ESTADO INICIAL DE ENEMIGOS ===");
        Debug.Log($"Enemigos encontrados con tag 'Enemy': {enemigos.Length}");
        
        if (enemigos.Length == 0)
        {
            Debug.LogWarning("¡NO SE ENCONTRARON ENEMIGOS EN LA ESCENA! Asegúrate de que los enemigos tengan el tag 'Enemy' y estén activos.");
            return;
        }
        
        foreach (GameObject enemigo in enemigos)
        {
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript == null) 
            {
                Debug.LogWarning($"El objeto {enemigo.name} tiene tag 'Enemy' pero no tiene el componente Enemy.cs");
                continue;
            }
            
            EnemySpawnData data = new EnemySpawnData
            {
                posicion = enemigo.transform.position,
                rotacion = enemigo.transform.rotation,
                layer = enemigo.layer,
                tipoData = enemyScript.enemyTypeData,
                tipo = enemyScript.enemyType,
                spriteVariant = enemyScript.spriteVariant,
                drops = enemyScript.posibleDrops != null ? (Item[])enemyScript.posibleDrops.Clone() : null
            };
            
            enemigosIniciales.Add(data);
            Debug.Log($"✓ Guardado enemigo {enemigosIniciales.Count}: {enemigo.name} en posición {data.posicion}, drops: {(data.drops != null && data.drops.Length > 0 ? data.drops[0]?.nombre : "ninguno")}");
        }
        
        Debug.Log($"=== TOTAL ENEMIGOS GUARDADOS: {enemigosIniciales.Count} ===");
    }

    #endregion

    #region Update y Utilidades

    void Update()
    {
        // Actualización continua de UI solo si el juego está activo
        if (juegoIniciado && !juegoTerminado && uiManager != null)
        {
            uiManager.Actualizar(cientifico, estabilizador, dimensionSwitcher);
        }
        
        // Permite reiniciar con R si se terminó el juego (compatibilidad legacy)
        if (Input.GetKeyDown(KeyCode.R))
        {
            bool pantallaLegacyActiva = false;
            if (pantallaDerrota != null && pantallaDerrota.activeSelf) pantallaLegacyActiva = true;
            if (pantallaVictoria != null && pantallaVictoria.activeSelf) pantallaLegacyActiva = true;
            
            if (pantallaLegacyActiva || juegoTerminado)
            {
                ReiniciarNivel();
            }
        }
    }

    void GuardarPosicionesInicialesEnemigos()
    {
        // Ya no es necesario, usamos posiciones fijas
    }

    public void RespawnearEnemigos()
    {
        if (usarEnemigosPreColocados)
        {
            RespawnearEnemigosPreColocados();
        }
        else
        {
            RespawnearEnemigosDesdePrefab();
        }}
    
    /// <summary>
    /// Restaura enemigos desde sus posiciones guardadas al inicio
    /// </summary>
    private void RespawnearEnemigosPreColocados()
    {
        Debug.Log("=== RESPAWNEANDO ENEMIGOS PRE-COLOCADOS ===");
        
        // Verificar que tenemos datos guardados PRIMERO
        if (enemigosIniciales.Count == 0)
        {
            Debug.LogError("¡CRÍTICO! La lista enemigosIniciales está VACÍA. Posibles causas:");
            Debug.LogError("1. No hay enemigos en la escena con tag 'Enemy'");
            Debug.LogError("2. Los enemigos no tienen el script Enemy.cs");
            Debug.LogError("3. GuardarEstadoInicialEnemigos() no se ejecutó correctamente en Start()");
            Debug.LogError("SOLUCIÓN: Coloca enemigos en la escena o cambia 'usarEnemigosPreColocados' a FALSE en el Inspector del GameManager");
            return;
        }
        
        // SIEMPRE destruir todos los enemigos existentes primero
        GameObject[] enemigosExistentes = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"Enemigos existentes antes de respawn: {enemigosExistentes.Length}");
        
        foreach (var enemigo in enemigosExistentes)
        {
            if (enemigo != null)
            {
                Debug.Log($"Destruyendo enemigo: {enemigo.name}");
                Destroy(enemigo);
            }
        }
        
        // Verificar que tenemos prefab
        if (enemigoPrefab == null)
        {
            Debug.LogError("enemigoPrefab es NULL! No se pueden recrear enemigos.");
            return;
        }
        
        Debug.Log($"Recreando {enemigosIniciales.Count} enemigos...");
        
        // Recrear cada enemigo desde su configuración guardada
        for (int i = 0; i < enemigosIniciales.Count; i++)
        {
            EnemySpawnData data = enemigosIniciales[i];
            
            Debug.Log($"Recreando enemigo {i} en posición {data.posicion}, capa {LayerMask.LayerToName(data.layer)}");
            GameObject enemigo = Instantiate(enemigoPrefab, data.posicion, data.rotacion);
            
            if (enemigo == null)
            {
                Debug.LogError($"Fallo al instanciar enemigo {i}");
                continue;
            }
            
            // Configurar propiedades básicas
            enemigo.name = $"Enemy_{i}";
            enemigo.tag = "Enemy";
            enemigo.layer = data.layer;
            
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Restaurar configuración original
                enemyScript.enemyTypeData = data.tipoData;
                enemyScript.enemyType = data.tipo;
                enemyScript.spriteVariant = data.spriteVariant;
                enemyScript.posibleDrops = data.drops;
                
                // Configurar tipo desde data si existe
                if (data.tipoData != null)
                {
                    enemyScript.ConfigurarTipoEnemigo(data.tipoData);
                }
                
                enemyScript.enabled = true;
                enemyScript.ResetearSalud();
                
                Debug.Log($"Enemigo {i} recreado: Tipo={enemyScript.enemyType}, Vida={enemyScript.saludEnemy}, Capa={LayerMask.LayerToName(data.layer)}");
            }

            // Activar colliders
            Collider2D[] colliders = enemigo.GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
        }
        
        // Verificar cuántos enemigos hay ahora
        GameObject[] enemigosFinales = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"=== RESPAWN COMPLETO === Enemigos ahora: {enemigosFinales.Length}/{enemigosIniciales.Count}");
    }
    
    /// <summary>
    /// Crea enemigos desde prefab en posiciones fijas (modo original)
    /// </summary>
    private void RespawnearEnemigosDesdePrefab()
    {
        Debug.Log("=== RESPAWNEANDO ENEMIGOS DESDE PREFAB ===");
        
        // Elimina todos los enemigos actuales
        GameObject[] enemigosActuales = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"Enemigos existentes antes de respawn: {enemigosActuales.Length}");
        
        foreach (var enemigo in enemigosActuales)
        {
            if (enemigo != null)
            {
                Debug.Log($"Destruyendo enemigo: {enemigo.name}");
                Destroy(enemigo);
            }
        }

        // Verificar que tenemos prefab
        if (enemigoPrefab == null)
        {
            Debug.LogError("enemigoPrefab es NULL! No se pueden crear enemigos.");
            return;
        }
        
        Debug.Log($"Creando {cantidadEnemigos} enemigos...");
        
        // Usar las posiciones configuradas en el Inspector, o posiciones por defecto si está vacío
        Vector2[] posicionesAUsar = (posicionesSpawnEnemigos != null && posicionesSpawnEnemigos.Length > 0) 
            ? posicionesSpawnEnemigos 
            : new Vector2[] {
                new Vector2(10f, 5f),
                new Vector2(-10f, 5f), 
                new Vector2(5f, 10f),
                new Vector2(-5f, -5f),
                new Vector2(0f, 8f)
            };
        
        Debug.Log($"Usando {posicionesAUsar.Length} posiciones de spawn configuradas");
        
        // Crear enemigos
        for (int i = 0; i < cantidadEnemigos && i < posicionesAUsar.Length; i++)
        {
            Vector2 pos = posicionesAUsar[i];
            Debug.Log($"Spawneando enemigo {i + 1} en posición: ({pos.x}, {pos.y})");
            
            Item drop = (componentesDropEnemigos != null && i < componentesDropEnemigos.Count) 
                        ? componentesDropEnemigos[i] : null;
            
            GameObject enemigo = Instantiate(enemigoPrefab, pos, Quaternion.identity);
            
            if (enemigo == null)
            {continue;
            }
            
            // Configurar enemigo básico
            enemigo.name = $"Enemy_{i}";
            enemigo.tag = "Enemy";
            enemigo.layer = LayerMask.NameToLayer("Dim_Altered");
            
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // Configurar tipo de enemigo
                ConfigurarTipoEnemigo(enemyScript, i);
                
                enemyScript.enabled = true;
                enemyScript.ResetearSalud();
                
                // Configurar drops
                if (drop != null && enemyScript.posibleDrops != null && enemyScript.posibleDrops.Length > 0)
                {
                    enemyScript.posibleDrops[0] = drop;
                }}

            // Activar colliders
            Collider2D[] colliders = enemigo.GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
        }
        
        // Verificar cuántos enemigos hay ahora
        GameObject[] enemigosFinales = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"=== RESPAWN COMPLETO === Enemigos ahora: {enemigosFinales.Length}/{cantidadEnemigos}");
    }
    
    /// <summary>
    /// Configura el tipo de enemigo según el índice
    /// </summary>
    private void ConfigurarTipoEnemigo(Enemy enemyScript, int index)
    {
        if (tiposEnemigos == null || tiposEnemigos.Length == 0)
        {
            // Sin configuración específica, usar tipos por defecto
            EnemyType[] tiposDefault = { EnemyType.Slime, EnemyType.Orc, EnemyType.Predator };
            enemyScript.enemyType = tiposDefault[index % tiposDefault.Length];
            enemyScript.spriteVariant = Random.Range(0, 3); // Variante aleatoria (0-2)
            return;
        }
        
        if (usarTiposAleatorios)
        {
            // Tipo aleatorio
            int tipoIndex = Random.Range(0, tiposEnemigos.Length);
            enemyScript.enemyTypeData = tiposEnemigos[tipoIndex];
            enemyScript.enemyType = tiposEnemigos[tipoIndex].enemyType;
        }
        else
        {
            // Tipo en orden
            int tipoIndex = index % tiposEnemigos.Length;
            enemyScript.enemyTypeData = tiposEnemigos[tipoIndex];
            enemyScript.enemyType = tiposEnemigos[tipoIndex].enemyType;
        }
        
        // Variante de sprite aleatoria (0-2)
        enemyScript.spriteVariant = Random.Range(0, 3);
    }
    
    /// <summary>
    /// Método público para forzar respawn desde inspector o console
    /// </summary>
    [ContextMenu("Forzar Respawn Enemigos")]
    public void ForzarRespawnEnemigos()
    {RespawnearEnemigos();
    }
    


    #endregion

    #region Métodos de Validación y Debug
    
    /// <summary>
    /// Busca automáticamente las referencias si no están asignadas
    /// </summary>
    private void BuscarReferenciasAutomaticas()
    {
        if (cientifico == null)
            cientifico = FindFirstObjectByType<Cientifico>();
        
        if (estabilizador == null)
            estabilizador = FindFirstObjectByType<EstabilizadorCuantico>();
        
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>();
        
        if (questManager == null)
            questManager = FindFirstObjectByType<QuestManager>();
        
        if (dimensionSwitcher == null)
            dimensionSwitcher = FindFirstObjectByType<DimensionSwitcher>();
    }

    void OnValidate()
    {
        // Verificar referencias básicas
        if (cientifico == null)if (estabilizador == null)if (enemigoPrefab == null)if (cantidadEnemigos <= 0)
            cantidadEnemigos = 3;
    }

    #endregion
}
