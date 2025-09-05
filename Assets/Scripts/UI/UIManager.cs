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

    public void Actualizar(Cientifico cientifico, EstabilizadorCuantico estabilizador)
    {
        if (salud != null) salud.value = cientifico.salud;
        if (sed != null) sed.value = cientifico.sed;
        if (hambre != null) hambre.value = cientifico.hambre;
        if (stamina != null) stamina.value = cientifico.stamina;
        
        // Actualizar iconos de componentes
        if (icon1 != null) icon1.enabled = estabilizador.componentesInsertados.Contains(1);
        if (icon2 != null) icon2.enabled = estabilizador.componentesInsertados.Contains(2);
        if (icon3 != null) icon3.enabled = estabilizador.componentesInsertados.Contains(3);
        
        // Si usas DimensionSwitcher, aquí puedes actualizar el texto de dimensión
        // if (dimensionText != null) dimensionText.text = "Dimensión: " + dimensionActual;
    }
}