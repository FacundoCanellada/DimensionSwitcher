using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Cientifico cientifico;
    public EstabilizadorCuantico estabilizador;
    public UIManager hud;
    public GameObject pantallaVictoria;
    public GameObject pantallaDerrota;

    public List<GameObject> enemigoPrefab;
    public List<Vector2> posicionesEnemigosIniciales;
    public List<Item> componentesDropEnemigos;

    private bool juegoTerminado = false;

    void Awake()
    {
        // Solo guarda las posiciones si la lista está vacía
        if (posicionesEnemigosIniciales == null || posicionesEnemigosIniciales.Count == 0)
            GuardarPosicionesInicialesEnemigos();
    }

    void Start()
    {
        // Guarda posiciones iniciales de enemigos
        GuardarPosicionesInicialesEnemigos();
        RespawnearEnemigos();
    }

    public void ComprobarDerrota()
    {
        if (!juegoTerminado && cientifico.salud <= 0)
        {
            TerminarJuego(pantallaDerrota);
        }
    }

    public void ComprobarVictoria()
    {
        if (!juegoTerminado && estabilizador.reparado)
        {
            TerminarJuego(pantallaVictoria);
        }
    }

    void TerminarJuego(GameObject pantalla)
    {
        juegoTerminado = true;
        pantallaVictoria.SetActive(false);
        pantallaDerrota.SetActive(false);
        pantalla.SetActive(true);

        // Congela el tiempo y deshabilita los inputs
        Time.timeScale = 0;
        cientifico.HabilitarControl(false);
    }

    public void ReiniciarNivel()
    {
        // Reactiva el tiempo y los inputs
        Time.timeScale = 1;
        juegoTerminado = false;
        pantallaVictoria.SetActive(false);
        pantallaDerrota.SetActive(false);

        cientifico.Resetear();
        estabilizador.Resetear();
        RespawnearEnemigos();

        hud.Actualizar(cientifico, estabilizador);

        Debug.Log("¡Nivel reiniciado!");
    }

    void Update()
    {
        // Permite reiniciar con R si se terminó el juego
        if ((pantallaDerrota.activeSelf || pantallaVictoria.activeSelf) && Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarNivel();
        }
    }

    void GuardarPosicionesInicialesEnemigos()
    {
        posicionesEnemigosIniciales = new List<Vector2>();
        foreach (var enemigo in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            posicionesEnemigosIniciales.Add(enemigo.transform.position);
        }
    }

    public void RespawnearEnemigos()
    {
        // Elimina todos los enemigos actuales
        foreach (var enemigo in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(enemigo);

        // Respawnea cada enemigo en la posición y le asigna su componente
        for (int i = 0; i < enemigoPrefab.Count; i++)
        {
            GameObject prefab = enemigoPrefab[i];
            Vector2 pos = posicionesEnemigosIniciales[i];
            Item drop = componentesDropEnemigos[i];

            GameObject enemigo = Instantiate(prefab, pos, Quaternion.identity);
            Enemy enemyScript = enemigo.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.enabled = true;
                enemyScript.dropItem = drop;
                enemyScript.saludEnemy = 100; // Reinicia la vida si lo necesitas
            }

            // Fuerza el CapsuleCollider2D a estar activo y habilitado
            CapsuleCollider2D capsula = enemigo.GetComponent<CapsuleCollider2D>();
            if (capsula != null)
            {
                capsula.enabled = true;
                capsula.gameObject.SetActive(true);
            }
        }
    }
}