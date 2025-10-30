using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente simple para añadir sonidos a botones UI
/// Añade este componente a cualquier botón para que tenga sonido automáticamente
/// </summary>
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Configuración")]
    [Tooltip("Reproducir sonido al hacer hover")]
    public bool reproducirHover = true;
    
    [Tooltip("Reproducir sonido al hacer clic")]
    public bool reproducirClick = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (reproducirHover && AudioManager.Instance != null)
        {
            AudioManager.Instance.SonidoBotonHover();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (reproducirClick && AudioManager.Instance != null)
        {
            AudioManager.Instance.SonidoBoton();
        }
    }
}
