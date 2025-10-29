using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sistema de tutorial con múltiples imágenes que se muestra solo la primera vez
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("Panel del Tutorial")]
    public GameObject panelTutorial;
    
    [Header("Imágenes del Tutorial")]
    public Image[] imagenesTutorial; // Array con las 5 imágenes
    
    [Header("Botones de Navegación")]
    public Button botonAnterior;
    public Button botonSiguiente;
    public Button botonSaltar;
    public Button botonComenzar; // Aparece en la última página
    
    [Header("Indicador de Página")]
    public TextMeshProUGUI textoPagina; // Ej: "1/5"
    
    [Header("Referencias")]
    public UIManager uiManager; // Para iniciar el juego después del tutorial
    
    private int paginaActual = 0;
    private const string CLAVE_TUTORIAL_VISTO = "TutorialVisto";
    private bool inicializado = false;
    
    void Awake()
    {
        // Configurar botones
        ConfigurarBotones();
        
        // Panel oculto al inicio
        if (panelTutorial != null)
            panelTutorial.SetActive(false);
    }
    
    /// <summary>
    /// Muestra el tutorial SIEMPRE (sin verificar PlayerPrefs)
    /// </summary>
    public void MostrarTutorial()
    {
        // Activar panel
        if (panelTutorial != null)
            panelTutorial.SetActive(true);
        
        // Mostrar primera página
        MostrarPagina(0);
        
        // Pausar juego
        Time.timeScale = 0;
    }
    
    void ConfigurarBotones()
    {
        if (botonAnterior != null)
            botonAnterior.onClick.AddListener(PaginaAnterior);
        
        if (botonSiguiente != null)
            botonSiguiente.onClick.AddListener(PaginaSiguiente);
        
        if (botonSaltar != null)
            botonSaltar.onClick.AddListener(SaltarTutorial);
        
        if (botonComenzar != null)
        {
            botonComenzar.onClick.AddListener(TerminarTutorial);
            botonComenzar.gameObject.SetActive(false); // Oculto al inicio
        }
    }
    
    void MostrarPagina(int indicePagina)
    {
        paginaActual = Mathf.Clamp(indicePagina, 0, imagenesTutorial.Length - 1);
        
        // Ocultar todas las imágenes
        for (int i = 0; i < imagenesTutorial.Length; i++)
        {
            if (imagenesTutorial[i] != null)
                imagenesTutorial[i].gameObject.SetActive(i == paginaActual);
        }
        
        // Actualizar texto de página
        if (textoPagina != null)
            textoPagina.text = $"{paginaActual + 1}/{imagenesTutorial.Length}";
        
        // Actualizar botones
        ActualizarBotones();
        
        Debug.Log($"Tutorial - Página {paginaActual + 1}/{imagenesTutorial.Length}");
    }
    
    void ActualizarBotones()
    {
        // Botón Anterior: Deshabilitado en la primera página
        if (botonAnterior != null)
            botonAnterior.interactable = paginaActual > 0;
        
        // Última página: Ocultar "Siguiente" y mostrar "Comenzar"
        bool esUltimaPagina = paginaActual >= imagenesTutorial.Length - 1;
        
        if (botonSiguiente != null)
            botonSiguiente.gameObject.SetActive(!esUltimaPagina);
        
        if (botonComenzar != null)
            botonComenzar.gameObject.SetActive(esUltimaPagina);
    }
    
    void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            MostrarPagina(paginaActual - 1);
        }
    }
    
    void PaginaSiguiente()
    {
        if (paginaActual < imagenesTutorial.Length - 1)
        {
            MostrarPagina(paginaActual + 1);
        }
    }
    
    void SaltarTutorial()
    {
        TerminarTutorial();
    }
    
    void TerminarTutorial()
    {
        // Ocultar panel
        if (panelTutorial != null)
            panelTutorial.SetActive(false);
        
        // Iniciar el juego
        if (uiManager != null)
            uiManager.IniciarJuegoDirectamente();
    }
    
    /// <summary>
    /// Método público para resetear el tutorial (útil para testing)
    /// </summary>
    public void ResetearTutorial()
    {
        PlayerPrefs.DeleteKey(CLAVE_TUTORIAL_VISTO);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reseteado - Se mostrará la próxima vez");
    }
}
