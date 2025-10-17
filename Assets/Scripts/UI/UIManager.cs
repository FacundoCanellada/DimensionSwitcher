using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public Slider salud;
    public Slider sed;
    public Slider hambre;
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

    [Header("Referencias")]
    public GameManager gameManager;

    [Header("Suavizado de Barras")]
    [Tooltip("Velocidad de interpolación de las barras (mayor = más rápido)")]
    public float velocidadLerp = 8f;

    // Valores suavizados internos
    private float saludVisual;
    private float sedVisual;
    private float hambreVisual;
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

        // Mostrar solo el menú principal al inicio
        MostrarMenuPrincipal();
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
    /// Actualiza el HUD con información del jugador, estabilizador y dimensión
    /// </summary>
    public void Actualizar(Cientifico cientifico, EstabilizadorCuantico estabilizador, DimensionSwitcher dimensionSwitcher = null)
    {
        // Inicializar una sola vez para evitar salto brusco
        if (!inicializadoBarras)
        {
            saludVisual = cientifico.salud;
            sedVisual = cientifico.sed;
            hambreVisual = cientifico.hambre;
            staminaVisual = cientifico.stamina;
            if (salud != null) salud.value = saludVisual;
            if (sed != null) sed.value = sedVisual;
            if (hambre != null) hambre.value = hambreVisual;
            if (stamina != null) stamina.value = staminaVisual;
            inicializadoBarras = true;
        }

        float t = Time.unscaledDeltaTime * velocidadLerp;
        // Lerp de valores
        saludVisual = Mathf.Lerp(saludVisual, cientifico.salud, t);
        sedVisual = Mathf.Lerp(sedVisual, cientifico.sed, t);
        hambreVisual = Mathf.Lerp(hambreVisual, cientifico.hambre, t);
        staminaVisual = Mathf.Lerp(staminaVisual, cientifico.stamina, t);

        // Aplicar a sliders si existen
        if (salud != null)
        {
            salud.value = saludVisual;
            if (salud.fillRect != null)
            {
                Image fillImage = salud.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.salud > 30 ? Color.green :
                                       cientifico.salud > 15 ? Color.yellow : Color.red;
            }
        }
        if (sed != null)
        {
            sed.value = sedVisual;
            if (sed.fillRect != null)
            {
                Image fillImage = sed.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.sed > 30 ? Color.cyan :
                                       cientifico.sed > 15 ? Color.yellow : Color.red;
            }
        }
        if (hambre != null)
        {
            hambre.value = hambreVisual;
            if (hambre.fillRect != null)
            {
                Image fillImage = hambre.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.hambre > 30 ? Color.green :
                                       cientifico.hambre > 15 ? Color.yellow : Color.red;
            }
        }
        if (stamina != null)
        {
            stamina.value = staminaVisual;
            if (stamina.fillRect != null)
            {
                Image fillImage = stamina.fillRect.GetComponent<Image>();
                if (fillImage != null)
                    fillImage.color = cientifico.stamina > 30 ? Color.blue :
                                       cientifico.stamina > 15 ? Color.yellow : Color.red;
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