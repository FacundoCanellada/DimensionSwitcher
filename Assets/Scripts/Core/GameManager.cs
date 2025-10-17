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
    public GameObject enemigoPrefab; // UN SOLO PREFAB
    public int cantidadEnemigos = 3; // CANTIDAD A CREAR
    public List<Item> componentesDropEnemigos;

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
        
        // No iniciar automáticamente el juego, esperar por UIManager
        // Si no hay UIManager, usar comportamiento legacy
        if (uiManager == null)
        {
            Debug.Log("No se encontró UIManager, usando comportamiento legacy");
            // Comportamiento legacy: iniciar directamente
            RespawnearEnemigos();
            juegoIniciado = true;
        }
        else
        {
            Debug.Log("UIManager encontrado, esperando inicio manual");
            // Nuevo comportamiento: esperar por el UIManager
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
        RespawnearEnemigos();
        
        // Habilitar controles del jugador
        if (cientifico != null) cientifico.HabilitarControl(true);
        
        // Actualizar UI si existe
        if (uiManager != null)
            uiManager.Actualizar(cientifico, estabilizador, dimensionSwitcher);

        Debug.Log("¡Juego iniciado!");
    }

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
        if (cientifico != null) cientifico.HabilitarControl(false);
        
        Debug.Log("¡Juego reseteado completamente!");
    }

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
        
        // Condición nueva: QuestManager tiene los 3 componentes Y estabilizador reparado
        if (questManager != null && estabilizador != null)
        {
            condicionVictoria = questManager.TieneLos3Componentes() && estabilizador.reparado;
        }
        // Condición legacy: solo estabilizador reparado
        else if (estabilizador != null)
        {
            condicionVictoria = estabilizador.reparado;
        }
        
        if (!juegoTerminado && condicionVictoria)
        {
            // Solo verificar si el juego ha iniciado (para compatibilidad)
            if (uiManager == null || juegoIniciado)
            {
                TerminarJuego(false); // false = victoria
                Debug.Log("¡VICTORIA! Todos los componentes encontrados y estabilizador reparado.");
            }
        }
    }

    void TerminarJuego(bool esDerrota)
    {
        juegoTerminado = true;
        
        // Deshabilitar controles del jugador
        if (cientifico != null) cientifico.HabilitarControl(false);
        
        // Usar UIManager si está disponible, sino usar sistema legacy
        if (uiManager != null)
        {
            if (esDerrota)
                uiManager.MostrarPantallaDerrota();
            else
                uiManager.MostrarPantallaVictoria();
        }
        else
        {
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
        Debug.Log("=== REINICIAR NIVEL INICIADO ===");
        
        // Reactiva el tiempo y los inputs
        Time.timeScale = 1;
        juegoTerminado = false;
        
        // Usar UIManager para ocultar pantallas si está disponible
        if (uiManager != null)
        {
            Debug.Log("Usando UIManager para reiniciar");
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
                cientifico.ResetearPosicion();
                Debug.Log("Científico reposicionado sin resetear stats");
            }
        }
        
        if (estabilizador != null) 
        {
            estabilizador.Resetear();
            Debug.Log("Estabilizador reseteado");
        }
        
        if (questManager != null) 
        {
            questManager.Resetear();
            Debug.Log("QuestManager reseteado");
        }
        
        if (dimensionSwitcher != null) 
        {
            // ARREGLADO: Forzar dimensión normal para que los enemigos se vean
            dimensionSwitcher.desbloqueado = true;
            dimensionSwitcher.dimensionActual = false; // Normal
            // Aplicar inmediatamente la dimensión normal
            dimensionSwitcher.GetType().GetMethod("SetearDimension", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(dimensionSwitcher, new object[] { false });
            Debug.Log("DimensionSwitcher forzado a DIMENSIÓN NORMAL - enemigos visibles");
        }
        
        // CRÍTICO: Respawnear enemigos
        Debug.Log("=== LLAMANDO A RESPAWNEAR ENEMIGOS ===");
        RespawnearEnemigos();
        
        // Verificar inmediatamente cuántos enemigos hay
        GameObject[] enemigosVerificar = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"=== DESPUÉS DEL RESPAWN HAY {enemigosVerificar.Length} ENEMIGOS ===");
        
        Debug.Log("Enemigos respawneados");

        // Habilitar controles
        if (cientifico != null) cientifico.HabilitarControl(true);

        // Actualizar UI
        if (uiManager != null)
        {
            uiManager.Actualizar(cientifico, estabilizador, dimensionSwitcher);
        }

        Debug.Log("¡Nivel reiniciado completamente!");
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
        Debug.Log("Usando posiciones fijas predefinidas");
    }

    public void RespawnearEnemigos()
    {
        Debug.Log("=== RESPAWN ENEMIGOS INICIADO ===");
        
        // Elimina todos los enemigos actuales
        GameObject[] enemigosActuales = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"Destruyendo {enemigosActuales.Length} enemigos existentes");
        
        foreach (var enemigo in enemigosActuales)
        {
            if (enemigo != null)
            {
                Destroy(enemigo);
            }
        }

        // Verificar que tenemos prefab
        if (enemigoPrefab == null)
        {
            Debug.LogError("=== NO HAY PREFAB DE ENEMIGO CONFIGURADO ===");
            return;
        }
        
        // Posiciones fijas para los enemigos
        Vector2[] posicionesFijas = {
            new Vector2(5f, 5f),
            new Vector2(-5f, 5f), 
            new Vector2(5f, -5f),
            new Vector2(-5f, -5f),
            new Vector2(0f, 8f)
        };
        
        Debug.Log($"=== CREANDO {cantidadEnemigos} ENEMIGOS ===");
        
        // Crear enemigos
        for (int i = 0; i < cantidadEnemigos && i < posicionesFijas.Length; i++)
        {
            Vector2 pos = posicionesFijas[i];
            Item drop = (componentesDropEnemigos != null && i < componentesDropEnemigos.Count) 
                        ? componentesDropEnemigos[i] : null;

            Debug.Log($"Creando enemigo {i} en posición {pos}");
            GameObject enemigo = Instantiate(enemigoPrefab, pos, Quaternion.identity);
            
            if (enemigo == null)
            {
                Debug.LogError($"ERROR: No se pudo crear enemigo {i}");
                continue;
            }
            
            // Configurar enemigo
            enemigo.name = $"Enemy_{i}";
            enemigo.tag = "Enemy";
            enemigo.layer = LayerMask.NameToLayer("Dim_Altered");
            
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enabled = true;
                enemyScript.ResetearSalud();
                
                // Configurar drops
                if (drop != null && enemyScript.posibleDrops != null && enemyScript.posibleDrops.Length > 0)
                {
                    enemyScript.posibleDrops[0] = drop;
                }
                
                Debug.Log($"Enemigo {i} creado: Vida={enemyScript.saludEnemy}");
            }

            // Activar colliders
            Collider2D[] colliders = enemigo.GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }
        }
        
        Debug.Log("=== RESPAWN ENEMIGOS COMPLETADO ===");
    }
    
    /// <summary>
    /// Método público para forzar respawn desde inspector o console
    /// </summary>
    [ContextMenu("Forzar Respawn Enemigos")]
    public void ForzarRespawnEnemigos()
    {
        Debug.Log(">>> FORZANDO RESPAWN DE ENEMIGOS DESDE INSPECTOR <<<");
        RespawnearEnemigos();
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
        if (cientifico == null)
            Debug.LogWarning("GameManager: Falta referencia a Cientifico");
        
        if (estabilizador == null)
            Debug.LogWarning("GameManager: Falta referencia a EstabilizadorCuantico");
        
        if (enemigoPrefab == null)
            Debug.LogWarning("GameManager: Falta prefab de enemigo");
            
        if (cantidadEnemigos <= 0)
            cantidadEnemigos = 3;
    }

    #endregion
}
