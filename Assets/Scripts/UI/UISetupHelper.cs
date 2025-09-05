using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script de ejemplo para configurar fácilmente todos los elementos de UI en Unity.
/// Adjunta este script a un GameObject vacío en tu escena y configura las referencias desde el Inspector.
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("INSTRUCCIONES")]
    [TextArea(5, 10)]
    public string instrucciones = 
        "1. Crea un Canvas en tu escena\n" +
        "2. Dentro del Canvas, crea estos GameObjects:\n" +
        "   - MenuPrincipal (Panel)\n" +
        "   - HUDContainer (Panel)\n" +
        "   - PantallaVictoria (Panel)\n" +
        "   - PantallaDerrota (Panel)\n" +
        "3. Configura los elementos según las indicaciones abajo\n" +
        "4. Arrastra las referencias al UIManager\n" +
        "5. Elimina este GameObject cuando termines";

    [Header("ESTRUCTURA RECOMENDADA")]
    [TextArea(10, 15)]
    public string estructuraUI = 
        "Canvas\n" +
        "├── MenuPrincipal\n" +
        "│   ├── Titulo (Text)\n" +
        "│   └── BotonIniciar (Button)\n" +
        "├── HUDContainer\n" +
        "│   ├── BarraSalud (Slider)\n" +
        "│   ├── BarraSed (Slider)\n" +
        "│   ├── BarraHambre (Slider)\n" +
        "│   ├── BarraStamina (Slider)\n" +
        "│   ├── IconoComponente1 (Image)\n" +
        "│   ├── IconoComponente2 (Image)\n" +
        "│   └── IconoComponente3 (Image)\n" +
        "├── PantallaVictoria\n" +
        "│   ├── TextoVictoria (Text)\n" +
        "│   └── BotonMenu (Button)\n" +
        "└── PantallaDerrota\n" +
        "    ├── TextoDerrota (Text)\n" +
        "    └── BotonReiniciar (Button)";

    [Header("CONFIGURACIÓN AUTOMÁTICA")]
    [Space(10)]
    public bool crearUIAutomaticamente = false;
    
    [Header("Referencias del GameManager")]
    public GameManager gameManager;

    void Start()
    {
        if (crearUIAutomaticamente)
        {
            CrearUICompleta();
        }
    }

    [ContextMenu("Crear UI Completa")]
    public void CrearUICompleta()
    {
        // Buscar o crear el Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Crear Menu Principal
        GameObject menuPrincipal = CrearPanel(canvas.transform, "MenuPrincipal");
        CrearTexto(menuPrincipal.transform, "Titulo", "DIMENSION SWITCHER", 36);
        CrearBoton(menuPrincipal.transform, "BotonIniciar", "INICIAR JUEGO");

        // Crear HUD Container
        GameObject hudContainer = CrearPanel(canvas.transform, "HUDContainer");
        CrearSlider(hudContainer.transform, "BarraSalud", "Salud", Color.red);
        CrearSlider(hudContainer.transform, "BarraSed", "Sed", Color.blue);
        CrearSlider(hudContainer.transform, "BarraHambre", "Hambre", Color.yellow);
        CrearSlider(hudContainer.transform, "BarraStamina", "Stamina", Color.green);
        CrearIcono(hudContainer.transform, "IconoComponente1");
        CrearIcono(hudContainer.transform, "IconoComponente2");
        CrearIcono(hudContainer.transform, "IconoComponente3");

        // Crear Pantalla Victoria
        GameObject pantallaVictoria = CrearPanel(canvas.transform, "PantallaVictoria");
        CrearTexto(pantallaVictoria.transform, "TextoVictoria", "¡VICTORIA!", 48);
        CrearBoton(pantallaVictoria.transform, "BotonMenu", "VOLVER AL MENU");

        // Crear Pantalla Derrota
        GameObject pantallaDerrota = CrearPanel(canvas.transform, "PantallaDerrota");
        CrearTexto(pantallaDerrota.transform, "TextoDerrota", "DERROTA", 48);
        CrearBoton(pantallaDerrota.transform, "BotonReiniciar", "REINTENTAR");

        // Configurar UIManager automáticamente si existe
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            ConfigurarUIManager(uiManager, menuPrincipal, hudContainer, pantallaVictoria, pantallaDerrota);
        }

        Debug.Log("UI creada automáticamente. Revisa el Canvas y configura las posiciones según tus necesidades.");
    }

    GameObject CrearPanel(Transform parent, string nombre)
    {
        GameObject panel = new GameObject(nombre);
        panel.transform.SetParent(parent);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        return panel;
    }

    GameObject CrearTexto(Transform parent, string nombre, string texto, int fontSize = 24)
    {
        GameObject textGO = new GameObject(nombre);
        textGO.transform.SetParent(parent);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
        
        TextMeshProUGUI textComponent = textGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = texto;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        return textGO;
    }

    GameObject CrearBoton(Transform parent, string nombre, string texto)
    {
        GameObject buttonGO = new GameObject(nombre);
        buttonGO.transform.SetParent(parent);
        
        RectTransform rect = buttonGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        Image image = buttonGO.AddComponent<Image>();
        image.color = Color.white;
        
        Button button = buttonGO.AddComponent<Button>();
        
        // Crear texto del botón
        GameObject textGO = CrearTexto(buttonGO.transform, "Text", texto, 18);
        
        return buttonGO;
    }

    GameObject CrearSlider(Transform parent, string nombre, string label, Color color)
    {
        GameObject sliderGO = new GameObject(nombre);
        sliderGO.transform.SetParent(parent);
        
        RectTransform rect = sliderGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 20);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        
        // Crear background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderGO.transform);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = Color.gray;
        
        // Crear fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderGO.transform);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = color;
        
        slider.fillRect = fillRect;
        
        return sliderGO;
    }

    GameObject CrearIcono(Transform parent, string nombre)
    {
        GameObject iconGO = new GameObject(nombre);
        iconGO.transform.SetParent(parent);
        
        RectTransform rect = iconGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(32, 32);
        
        Image image = iconGO.AddComponent<Image>();
        image.color = Color.white;
        image.enabled = false; // Inicialmente deshabilitado
        
        return iconGO;
    }

    void ConfigurarUIManager(UIManager uiManager, GameObject menuPrincipal, GameObject hudContainer, 
                           GameObject pantallaVictoria, GameObject pantallaDerrota)
    {
        // Configurar referencias principales
        uiManager.menuPrincipal = menuPrincipal;
        uiManager.hudContainer = hudContainer;
        uiManager.pantallaVictoria = pantallaVictoria;
        uiManager.pantallaDerrota = pantallaDerrota;
        
        // Configurar botones
        uiManager.botonIniciar = menuPrincipal.transform.Find("BotonIniciar")?.GetComponent<Button>();
        uiManager.botonMenuVictoria = pantallaVictoria.transform.Find("BotonMenu")?.GetComponent<Button>();
        uiManager.botonReiniciarDerrota = pantallaDerrota.transform.Find("BotonReiniciar")?.GetComponent<Button>();
        
        // Configurar sliders del HUD
        uiManager.salud = hudContainer.transform.Find("BarraSalud")?.GetComponent<Slider>();
        uiManager.sed = hudContainer.transform.Find("BarraSed")?.GetComponent<Slider>();
        uiManager.hambre = hudContainer.transform.Find("BarraHambre")?.GetComponent<Slider>();
        uiManager.stamina = hudContainer.transform.Find("BarraStamina")?.GetComponent<Slider>();
        
        // Configurar iconos
        uiManager.icon1 = hudContainer.transform.Find("IconoComponente1")?.GetComponent<Image>();
        uiManager.icon2 = hudContainer.transform.Find("IconoComponente2")?.GetComponent<Image>();
        uiManager.icon3 = hudContainer.transform.Find("IconoComponente3")?.GetComponent<Image>();
        
        // Configurar GameManager
        if (gameManager != null)
        {
            uiManager.gameManager = gameManager;
            gameManager.uiManager = uiManager;
        }
        
        Debug.Log("UIManager configurado automáticamente!");
    }
}
