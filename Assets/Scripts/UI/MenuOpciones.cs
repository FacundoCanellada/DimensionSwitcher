using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuOpciones : MonoBehaviour
{
    [Header("Configuración de Video")]
    public TextMeshProUGUI textoResolucion;
    public TextMeshProUGUI textoCalidad;
    public Slider sliderBrillo;
    
    [Header("Navegación")]
    public Color colorOpcionSeleccionada = Color.yellow;
    public Color colorOpcionNormal = Color.white;
    
    private Resolution[] resoluciones;
    private int indiceResolucionActual = 0;
    private int indiceCalidadActual = 0;
    
    private int opcionSeleccionada = 0;
    private int totalOpciones = 3; // Resolución, Calidad, Brillo
    
    private List<TextMeshProUGUI> textosOpciones = new List<TextMeshProUGUI>();
    private bool modificandoBrillo = false;

    void Start()
    {
        ConfigurarResoluciones();
        ConfigurarCalidad();
        ConfigurarBrillo();
        
        // Guardar referencias a los textos
        if (textoResolucion != null) textosOpciones.Add(textoResolucion);
        if (textoCalidad != null) textosOpciones.Add(textoCalidad);
        
        totalOpciones = textosOpciones.Count + 1; // +1 para el slider
    }
    
    void OnEnable()
    {
        // Al abrir el menú, resetear selección
        opcionSeleccionada = 0;
        ActualizarSeleccion();
    }
    
    void Update()
    {
        // Solo procesar input si el menú está activo
        if (!gameObject.activeSelf) return;
        
        ManejarNavegacion();
    }
    
    private void ManejarNavegacion()
    {
        // Navegar entre opciones con W/S o flechas arriba/abajo
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            opcionSeleccionada--;
            if (opcionSeleccionada < 0) opcionSeleccionada = totalOpciones - 1;
            ActualizarSeleccion();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            opcionSeleccionada++;
            if (opcionSeleccionada >= totalOpciones) opcionSeleccionada = 0;
            ActualizarSeleccion();
        }
        
        // Cambiar valores con A/D o flechas izquierda/derecha
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ModificarOpcion(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ModificarOpcion(1);
        }
    }
    
    private void ActualizarSeleccion()
    {
        // Resaltar la opción seleccionada
        for (int i = 0; i < textosOpciones.Count; i++)
        {
            if (textosOpciones[i] != null)
            {
                textosOpciones[i].color = (i == opcionSeleccionada) ? colorOpcionSeleccionada : colorOpcionNormal;
            }
        }
        
        // Resaltar slider de brillo si está seleccionado
        if (sliderBrillo != null)
        {
            ColorBlock colors = sliderBrillo.colors;
            colors.normalColor = (opcionSeleccionada == 2) ? colorOpcionSeleccionada : colorOpcionNormal;
            sliderBrillo.colors = colors;
        }
    }
    
    private void ModificarOpcion(int direccion)
    {
        switch (opcionSeleccionada)
        {
            case 0: // Resolución
                CambiarResolucion(direccion);
                break;
            case 1: // Calidad
                CambiarCalidad(direccion);
                break;
            case 2: // Brillo
                CambiarBrillo(direccion);
                break;
        }
    }
    
    private void ConfigurarResoluciones()
    {
        resoluciones = Screen.resolutions;
        
        // Encontrar la resolución actual
        for (int i = 0; i < resoluciones.Length; i++)
        {
            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                indiceResolucionActual = i;
                break;
            }
        }
        
        ActualizarTextoResolucion();
    }
    
    private void ConfigurarCalidad()
    {
        indiceCalidadActual = QualitySettings.GetQualityLevel();
        ActualizarTextoCalidad();
    }
    
    private void ConfigurarBrillo()
    {
        if (sliderBrillo != null)
        {
            sliderBrillo.value = 0.85f; // Valor inicial 85%
            sliderBrillo.minValue = 0f;
            sliderBrillo.maxValue = 1f;
        }
    }
    
    private void CambiarResolucion(int direccion)
    {
        indiceResolucionActual += direccion;
        
        // Ciclar entre resoluciones
        if (indiceResolucionActual < 0) indiceResolucionActual = resoluciones.Length - 1;
        if (indiceResolucionActual >= resoluciones.Length) indiceResolucionActual = 0;
        
        Resolution resolucion = resoluciones[indiceResolucionActual];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
        
        ActualizarTextoResolucion();
        Debug.Log($"Resolución cambiada a: {resolucion.width}x{resolucion.height}");
    }
    
    private void CambiarCalidad(int direccion)
    {
        indiceCalidadActual += direccion;
        
        // Ciclar entre niveles de calidad
        int maxCalidad = QualitySettings.names.Length - 1;
        if (indiceCalidadActual < 0) indiceCalidadActual = maxCalidad;
        if (indiceCalidadActual > maxCalidad) indiceCalidadActual = 0;
        
        QualitySettings.SetQualityLevel(indiceCalidadActual);
        
        ActualizarTextoCalidad();
        Debug.Log($"Calidad cambiada a: {QualitySettings.names[indiceCalidadActual]}");
    }
    
    private void CambiarBrillo(int direccion)
    {
        if (sliderBrillo != null)
        {
            float nuevoBrillo = sliderBrillo.value + (direccion * 0.05f);
            nuevoBrillo = Mathf.Clamp01(nuevoBrillo);
            sliderBrillo.value = nuevoBrillo;
        }
    }
    
    private void ActualizarTextoResolucion()
    {
        if (textoResolucion != null && resoluciones != null && indiceResolucionActual < resoluciones.Length)
        {
            Resolution res = resoluciones[indiceResolucionActual];
            textoResolucion.text = $"{res.width}x{res.height}";
        }
    }
    
    private void ActualizarTextoCalidad()
    {
        if (textoCalidad != null)
        {
            string[] calidades = new string[] { "Low", "Medium", "High", "Ultra" };
            int index = Mathf.Min(indiceCalidadActual, calidades.Length - 1);
            textoCalidad.text = calidades[index];
        }
    }
}
