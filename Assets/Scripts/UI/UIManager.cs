using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements - SISTEMA SIMPLIFICADO")]
    public Slider salud;
    public Slider stamina;
    public TextMeshProUGUI dimensionText;
    public Image icon1;
    public Image icon2;
    public Image icon3;
    public GameObject hudContainer; // Panel que contiene todos los elementos del HUD

    [Header("Menu Principal")]
    public GameObject menuPrincipal;
    public Button botonIniciar;

    [Header("Pantalla Victoria")]
    public GameObject pantallaVictoria;
    public Button botonMenuVictoria;

    [Header("Pantalla Derrota")]
    public GameObject pantallaDerrota;
    public Button botonReiniciarDerrota;

    [Header("Menu de Pausa")]
    public GameObject menuPausa; // Panel del menú de pausa
    public GameObject panelInventario; // Pestaña de inventario
    public GameObject panelOpciones; // Pestaña de opciones
    public GameObject panelControles; // Pestaña de controles
    public GameObject panelEstadisticas; // Pestaña de estadísticas del jugador
    
    [Header("Referencias")]
    public GameManager gameManager;

    [Header("Suavizado de Barras")]
    [Tooltip("Velocidad de interpolación de las barras (mayor = más rápido)")]
    public float velocidadLerp = 8f;
    
    // Variables de menú de pausa
    private bool menuPausaAbierto = false;
    private enum PestanaActiva { Inventario, Opciones, Controles, Estadisticas }
    private PestanaActiva pestanaActual = PestanaActiva.Controles; // Controles por defecto

    // Valores suavizados internos - SISTEMA SIMPLIFICADO
    private float saludVisual;
    private float staminaVisual;

    private bool inicializadoBarras = false;

    private void Start()
    {
        // Configurar botones
        if (botonIniciar != null)
            botonIniciar.onClick.AddListener(IniciarJuego);
        
        if (botonMenuVictoria != null)
            botonMenuVictoria.onClick.AddListener(VolverAlMenu);
        
        if (botonReiniciarDerrota != null)
            botonReiniciarDerrota.onClick.AddListener(ReiniciarJuego);

        // Inicializar menú de pausa cerrado
        if (menuPausa != null) menuPausa.SetActive(false);

        // Mostrar solo el menú principal al inicio
        MostrarMenuPrincipal();
    }
    
    private void Update()
    {
        // Solo manejar menú de pausa si el juego está en curso
        if (menuPrincipal != null && menuPrincipal.activeSelf) return;
        if (pantallaVictoria != null && pantallaVictoria.activeSelf) return;
        if (pantallaDerrota != null && pantallaDerrota.activeSelf) return;
        
        // Abrir/cerrar menú de pausa con Escape o P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenuPausa();
        }
        
        // Si el menú está abierto, manejar navegación entre pestañas
        if (menuPausaAbierto)
        {
            // E - Siguiente pestaña
            if (Input.GetKeyDown(KeyCode.E))
            {
                CambiarPestanaSiguiente();
            }
            // Q - Pestaña anterior
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                CambiarPestanaAnterior();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                CerrarMenuPausa();
            }
        }
    }
    
    private void CambiarPestanaSiguiente()
    {
        int siguiente = ((int)pestanaActual + 1) % 4; // 4 pestañas totales
        pestanaActual = (PestanaActiva)siguiente;
        ActualizarPestanas();
    }
    
    private void CambiarPestanaAnterior()
    {
        int anterior = ((int)pestanaActual - 1);
        if (anterior < 0) anterior = 3; // 4 pestañas, índice 0-3
        pestanaActual = (PestanaActiva)anterior;
        ActualizarPestanas();
    }
    
    private void ToggleMenuPausa()
    {
        if (menuPausaAbierto)
        {
            CerrarMenuPausa();
        }
        else
        {
            AbrirMenuPausa();
        }
    }
    
    private void AbrirMenuPausa()
    {
        menuPausaAbierto = true;
        if (menuPausa != null) menuPausa.SetActive(true);
        
        // Mostrar pestaña de CONTROLES por defecto (para que el jugador sepa cómo jugar)
        pestanaActual = PestanaActiva.Controles;
        ActualizarPestanas();
        
        Time.timeScale = 0f; // Pausar el juego
        
        Debug.Log("Menú de pausa abierto - Mostrando Controles por defecto");
    }
    
    private void CerrarMenuPausa()
    {
        menuPausaAbierto = false;
        if (menuPausa != null) menuPausa.SetActive(false);
        
        Time.timeScale = 1f; // Reanudar el juego
    }
    
    private void CambiarPestana(PestanaActiva nuevaPestana)
    {
        pestanaActual = nuevaPestana;
        ActualizarPestanas();
    }
    
    private void ActualizarPestanas()
    {
        // Activar/desactivar paneles según la pestaña actual
        if (panelInventario != null)
            panelInventario.SetActive(pestanaActual == PestanaActiva.Inventario);
        
        if (panelOpciones != null)
            panelOpciones.SetActive(pestanaActual == PestanaActiva.Opciones);
        
        if (panelControles != null)
            panelControles.SetActive(pestanaActual == PestanaActiva.Controles);
        
        if (panelEstadisticas != null)
            panelEstadisticas.SetActive(pestanaActual == PestanaActiva.Estadisticas);
    }

    public void MostrarMenuPrincipal()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (hudContainer != null) hudContainer.SetActive(false);
        if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
        if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        
        // Pausar el juego
        Time.timeScale = 0;
    }

    public void IniciarJuego()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (hudContainer != null) hudContainer.SetActive(true);
        if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
        if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        
        // Reanudar el juego
        Time.timeScale = 1;
        
        // Reiniciar el estado del juego cuando se inicia
        if (gameManager != null)
            gameManager.IniciarJuego();
    }

    public void MostrarPantallaVictoria()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (hudContainer != null) hudContainer.SetActive(false);
        if (pantallaVictoria != null) pantallaVictoria.SetActive(true);
        if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        
        // Pausar el juego
        Time.timeScale = 0;
    }

    public void MostrarPantallaDerrota()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (hudContainer != null) hudContainer.SetActive(false);
        if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
        if (pantallaDerrota != null) pantallaDerrota.SetActive(true);
        
        // Pausar el juego
        Time.timeScale = 0;
    }

    public void VolverAlMenu()
    {
        // Resetear completamente el juego antes de volver al menú
        if (gameManager != null)
            gameManager.ResetearJuegoCompleto();
        
        MostrarMenuPrincipal();
    }

    public void ReiniciarJuego()
    {
        // Reiniciar nivel manteniendo el HUD activo
        if (hudContainer != null) hudContainer.SetActive(true);
        if (pantallaVictoria != null) pantallaVictoria.SetActive(false);
        if (pantallaDerrota != null) pantallaDerrota.SetActive(false);
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        
        // Reanudar el juego
        Time.timeScale = 1;
        
        // Reiniciar el nivel
        if (gameManager != null)
            gameManager.ReiniciarNivel();
    }

    /// <summary>
    /// Actualiza el HUD con información del jugador, estabilizador y dimensión - SISTEMA SIMPLIFICADO
    /// </summary>
    public void Actualizar(Cientifico cientifico, EstabilizadorCuantico estabilizador, DimensionSwitcher dimensionSwitcher = null)
    {
        // Inicializar una sola vez para evitar salto brusco
        if (!inicializadoBarras)
        {
            saludVisual = cientifico.salud;
            staminaVisual = cientifico.stamina;
            if (salud != null) salud.value = saludVisual;
            if (stamina != null) stamina.value = staminaVisual;
            inicializadoBarras = true;
        }

        float t = Time.unscaledDeltaTime * velocidadLerp;
        // Lerp de valores
        saludVisual = Mathf.Lerp(saludVisual, cientifico.salud, t);
        staminaVisual = Mathf.Lerp(staminaVisual, cientifico.stamina, t);

        // Aplicar a sliders si existen
        if (salud != null)
        {
            salud.value = saludVisual;
            if (salud.fillRect != null)
            {
                Image fillImage = salud.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.salud > 50 ? Color.green :
                                       cientifico.salud > 25 ? Color.yellow : Color.red;
            }
        }
        if (stamina != null)
        {
            stamina.value = staminaVisual;
            if (stamina.fillRect != null)
            {
                Image fillImage = stamina.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.stamina > 50 ? Color.blue :
                                       cientifico.stamina > 25 ? Color.yellow : Color.red;
            }
        }
        
        // Actualizar iconos de componentes del estabilizador
        if (estabilizador != null)
        {
            if (icon1 != null) 
            {
                bool tieneComponente1 = estabilizador.componentesInsertados.Contains(1);
                icon1.enabled = tieneComponente1;
                if (tieneComponente1) icon1.color = Color.green;
            }
            
            if (icon2 != null) 
            {
                bool tieneComponente2 = estabilizador.componentesInsertados.Contains(2);
                icon2.enabled = tieneComponente2;
                if (tieneComponente2) icon2.color = Color.green;
            }
            
            if (icon3 != null) 
            {
                bool tieneComponente3 = estabilizador.componentesInsertados.Contains(3);
                icon3.enabled = tieneComponente3;
                if (tieneComponente3) icon3.color = Color.green;
            }
        }
        
        // Actualizar información de dimensión
        if (dimensionSwitcher != null && dimensionText != null)
        {
            string estadoDimension = dimensionSwitcher.GetDimensionActual();
            float cooldown = dimensionSwitcher.GetTiempoRestanteCooldown();
            bool desbloqueado = dimensionSwitcher.EstaDesbloqueado();
            
            if (!desbloqueado)
            {
                dimensionText.text = "Dimensión: BLOQUEADA";
                dimensionText.color = Color.gray;
            }
            else if (cooldown > 0)
            {
                dimensionText.text = $"Dimensión: {estadoDimension} (Cooldown: {cooldown:F1}s)";
                dimensionText.color = Color.yellow;
            }
            else
            {
                dimensionText.text = $"Dimensión: {estadoDimension} [Tab para cambiar]";
                dimensionText.color = estadoDimension == "NORMAL" ? Color.blue : Color.red;
            }
        }
    }
    
    /// <summary>
    /// Versión legacy para compatibilidad
    /// </summary>
    public void Actualizar(Cientifico cientifico, EstabilizadorCuantico estabilizador)
    {
        Actualizar(cientifico, estabilizador, null);
    }
}