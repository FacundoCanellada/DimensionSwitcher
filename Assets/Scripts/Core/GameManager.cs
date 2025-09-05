using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Referencias Principales")]
    public Cientifico cientifico;
    public EstabilizadorCuantico estabilizador;
    public UIManager uiManager; // Nueva referencia al UIManager
    
    [Header("Pantallas de Juego (Legacy - para compatibilidad)")]
    public GameObject pantallaVictoria;
    public GameObject pantallaDerrota;

    [Header("Configuración de Enemigos")]
    public List<GameObject> enemigoPrefab;
    public List<Vector2> posicionesEnemigosIniciales;
    public List<Item> componentesDropEnemigos;

    [Header("Estado del Juego")]
    private bool juegoTerminado = false;
    private bool juegoIniciado = false; // Para controlar si el juego ha comenzado

    void Awake()
    {
        // Solo guarda las posiciones si la lista está vacía
        if (posicionesEnemigosIniciales == null || posicionesEnemigosIniciales.Count == 0)
            GuardarPosicionesInicialesEnemigos();
    }

    void Start()
    {
        // Guarda posiciones iniciales de enemigos
        GuardarPosicionesInicialesEnemigos();
        
        // No iniciar automáticamente el juego, esperar por UIManager
        // Si no hay UIManager, usar comportamiento legacy
        if (uiManager == null)
        {
            // Comportamiento legacy: iniciar directamente
            RespawnearEnemigos();
            juegoIniciado = true;
        }
        else
        {
            // Nuevo comportamiento: esperar por el UIManager
            juegoIniciado = false;
        }
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
        RespawnearEnemigos();
        
        // Habilitar controles del jugador
        if (cientifico != null) cientifico.HabilitarControl(true);
        
        // Actualizar UI si existe
        if (uiManager != null)
            uiManager.Actualizar(cientifico, estabilizador);

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
        if (!juegoTerminado && estabilizador != null && estabilizador.reparado)
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
        // Reactiva el tiempo y los inputs
        Time.timeScale = 1;
        juegoTerminado = false;
        
        // Solo ocultar pantallas legacy si existen y no hay UIManager
        if (uiManager == null)
        {
            if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
            if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        }

        // Resetear componentes
        if (cientifico != null) cientifico.Resetear();
        if (estabilizador != null) estabilizador.Resetear();
        RespawnearEnemigos();

        // Habilitar controles
        if (cientifico != null) cientifico.HabilitarControl(true);

        // Actualizar UI
        if (uiManager != null)
        {
            uiManager.Actualizar(cientifico, estabilizador);
        }

        Debug.Log("¡Nivel reiniciado!");
    }

    #endregion

    #region Update y Utilidades

    void Update()
    {
        // Actualización continua de UI solo si el juego está activo
        if (juegoIniciado && !juegoTerminado && uiManager != null)
        {
            uiManager.Actualizar(cientifico, estabilizador);
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
        if (posicionesEnemigosIniciales == null)
            posicionesEnemigosIniciales = new List<Vector2>();
        
        posicionesEnemigosIniciales.Clear();
        
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemigo in enemigos)
        {
            if (enemigo != null)
                posicionesEnemigosIniciales.Add(enemigo.transform.position);
        }
    }

    public void RespawnearEnemigos()
    {
        // Elimina todos los enemigos actuales
        GameObject[] enemigosActuales = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemigo in enemigosActuales)
        {
            if (enemigo != null)
                Destroy(enemigo);
        }

        // Verificar que tenemos prefabs y posiciones
        if (enemigoPrefab == null || posicionesEnemigosIniciales == null) return;
        
        // Respawnea cada enemigo en la posición y le asigna su componente
        for (int i = 0; i < enemigoPrefab.Count && i < posicionesEnemigosIniciales.Count; i++)
        {
            GameObject prefab = enemigoPrefab[i];
            if (prefab == null) continue;
            
            Vector2 pos = posicionesEnemigosIniciales[i];
            Item drop = (componentesDropEnemigos != null && i < componentesDropEnemigos.Count) 
                        ? componentesDropEnemigos[i] : null;

            GameObject enemigo = Instantiate(prefab, pos, Quaternion.identity);
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enabled = true;
                enemyScript.dropItem = drop;
                enemyScript.saludEnemy = 100; // Reinicia la vida
            }

            // Fuerza el CapsuleCollider2D a estar activo y habilitado
            CapsuleCollider2D capsula = enemigo.GetComponent<CapsuleCollider2D>();
            if (capsula != null)
            {
                capsula.enabled = true;
                capsula.gameObject.SetActive(true);
            }
        }
    }

    #endregion

    #region Métodos de Validación y Debug

    void OnValidate()
    {
        // Verificar referencias básicas
        if (cientifico == null)
            Debug.LogWarning("GameManager: Falta referencia a Cientifico");
        
        if (estabilizador == null)
            Debug.LogWarning("GameManager: Falta referencia a EstabilizadorCuantico");
        
        // Verificar consistencia de listas de enemigos
        if (enemigoPrefab != null && posicionesEnemigosIniciales != null && 
            enemigoPrefab.Count != posicionesEnemigosIniciales.Count)
        {
            Debug.LogWarning("GameManager: El número de prefabs de enemigos no coincide con el número de posiciones iniciales");
        }
        
        if (enemigoPrefab != null && componentesDropEnemigos != null && 
            enemigoPrefab.Count != componentesDropEnemigos.Count)
        {
            Debug.LogWarning("GameManager: El número de prefabs de enemigos no coincide con el número de componentes drop");
        }
    }

    #endregion
}
