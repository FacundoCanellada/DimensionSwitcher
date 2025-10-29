using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel simplificado que muestra SOLO LA SALUD del jugador
/// </summary>
public class PanelEstadisticas : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public Cientifico cientifico;
    
    [Header("Barra de Salud")]
    public Slider barraSalud;
    public TextMeshProUGUI textoSalud;
    
    [Header("Actualización")]
    public float velocidadActualizacion = 0.1f;
    
    private float tiempoUltimaActualizacion = 0f;
    
    void Start()
    {
        // Buscar científico si no está asignado
        if (cientifico == null)
        {
            cientifico = FindFirstObjectByType<Cientifico>();
        }
        
        Debug.Log("PanelEstadisticas inicializado - Sistema simplificado (solo salud)");
    }
    
    void OnEnable()
    {
        // Al abrir el panel, actualizar inmediatamente
        ActualizarEstadisticas();
    }
    
    void Update()
    {
        // Actualizar periódicamente mientras el panel está activo
        tiempoUltimaActualizacion += Time.unscaledDeltaTime;
        
        if (tiempoUltimaActualizacion >= velocidadActualizacion)
        {
            ActualizarEstadisticas();
            tiempoUltimaActualizacion = 0f;
        }
    }
    
    private void ActualizarEstadisticas()
    {
        if (cientifico == null) 
        {
            Debug.LogWarning("PanelEstadisticas: No hay referencia al Científico!");
            return;
        }
        
        // Actualizar barra de salud
        if (barraSalud != null)
        {
            barraSalud.maxValue = 100;
            barraSalud.value = cientifico.salud;
            
            Debug.Log($"PanelEstadisticas actualizando: barraSalud.value = {cientifico.salud}");
            
            // Actualizar color según el valor
            if (barraSalud.fillRect != null)
            {
                Image fillImage = barraSalud.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    if (cientifico.salud > 50)
                        fillImage.color = Color.green;
                    else if (cientifico.salud > 25)
                        fillImage.color = Color.yellow;
                    else
                        fillImage.color = Color.red;
                }
                else
                {
                    Debug.LogWarning("PanelEstadisticas: barraSalud.fillRect no tiene componente Image!");
                }
            }
            else
            {
                Debug.LogWarning("PanelEstadisticas: barraSalud.fillRect es null!");
            }
        }
        else
        {
            Debug.LogWarning("PanelEstadisticas: barraSalud es null!");
        }
        
        // Actualizar texto
        if (textoSalud != null)
        {
            textoSalud.text = $": {Mathf.RoundToInt(cientifico.salud)}/100";
        }
        else
        {
            Debug.LogWarning("PanelEstadisticas: textoSalud es null!");
        }
    }
}
