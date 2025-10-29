using UnityEngine;
using TMPro;

public class PanelControles : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI textoControles;
    
    void Start()
    {
        ConfigurarControles();
    }
    
    void OnEnable()
    {
        ConfigurarControles();
    }
    
    private void ConfigurarControles()
    {
        if (textoControles == null) return;
        
        string controles = @"

<size=24><b>MOVIMIENTO:</b></size>
  <b>W/A/S/D</b> o <b>Flechas</b> - Mover personaje
  
<size=24><b>ACCIONES:</b></size>
  <b>ESPACIO</b> - Atacar
  <b>TAB</b> - Cambiar dimensión
  <b>E</b> - Interactuar con objetos
  
<size=24><b>MENÚ:</b></size>
  <b>ESC</b> o <b>P</b> - Abrir/Cerrar menú de pausa
  <b>E</b> - Siguiente pestaña
  <b>Q</b> - Pestaña anterior
  <b>C</b> - Cerrar menú
  
<size=24><b>INVENTARIO:</b></size>
  <b>W/A/S/D</b> - Navegar entre items
  <b>X</b> - Usar/Equipar item seleccionado
  <b>DEL</b> - Tirar item
  
<size=24><b>OPCIONES:</b></size>
  <b>W/S</b> - Navegar entre opciones
  <b>A/D</b> - Cambiar valores";

        textoControles.text = controles;
    }
}
