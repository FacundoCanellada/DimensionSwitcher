using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Cientifico cientifico;
    public EstabilizadorCuantico estabilizador;
    public UIManager hud;
    public GameObject pantallaVictoria;
    public GameObject pantallaDerrota;

    private bool juegoTerminado = false;

    public void ComprobarDerrota()
    {
        if (!juegoTerminado && cientifico.salud <= 0)
        {
            juegoTerminado = true;
            pantallaDerrota.SetActive(true);
            pantallaVictoria.SetActive(false);
        }
    }

    public void ComprobarVictoria()
    {
        if (!juegoTerminado && estabilizador.reparado)
        {
            juegoTerminado = true;
            pantallaVictoria.SetActive(true);
            pantallaDerrota.SetActive(false);
        }
    }

    public void ReiniciarNivel()
    {
        pantallaVictoria.SetActive(false);
        pantallaDerrota.SetActive(false);
        juegoTerminado = false;
        cientifico.Resetear();
        estabilizador.Resetear();
        // Si querés, respawnea enemigos acá
        Debug.Log("¡Reiniciado!");
    }

    void Update()
    {
        hud.Actualizar(cientifico, estabilizador);

        if (pantallaDerrota.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarNivel();
        }
    }
}