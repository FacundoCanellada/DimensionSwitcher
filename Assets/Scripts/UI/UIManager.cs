using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider salud;
    public Slider sed;
    public Slider hambre;
    public Slider stamina;
    public TextMeshProUGUI dimensionText;
    public Image icon1;
    public Image icon2;
    public Image icon3;

    public void Actualizar(Cientifico cientifico, EstabilizadorCuantico estabilizador)
    {
        salud.value = cientifico.salud;
        sed.value = cientifico.sed;
        hambre.value = cientifico.hambre;
        stamina.value = cientifico.stamina;
        // dimensionText.text = ... // Si usás DimensionSwitcher
        icon1.enabled = estabilizador.componentesInsertados.Contains(1);
        icon2.enabled = estabilizador.componentesInsertados.Contains(2);
        icon3.enabled = estabilizador.componentesInsertados.Contains(3);
    }
}