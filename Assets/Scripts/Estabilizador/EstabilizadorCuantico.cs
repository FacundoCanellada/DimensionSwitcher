using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EstabilizadorCuantico : MonoBehaviour
{
    public List<int> componentesNecesarios = new() { 1, 2, 3 };
    public List<int> componentesInsertados = new();
    public bool reparado = false;

    [Header("Interacci�n")]
    public TextMeshProUGUI textoInteractuar; // Asignar en el inspector (Canvas UI)
    private Cientifico cientificoCerca;

    [Header("GameManager")]
    public GameManager gameManager; // Asignar en el inspector

    [Header("Componentes disponibles (ItemSO)")]
    public List<Item> todosLosComponentes; // Asignar todos los ItemSO de tipo Components

    void Start()
    {
        if (textoInteractuar != null)
            textoInteractuar.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Cientifico player = other.GetComponent<Cientifico>();
        if (player != null)
        {
            cientificoCerca = player;
            if (textoInteractuar != null)
            {
                textoInteractuar.text = "Presiona E para colocar componente";
                textoInteractuar.enabled = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Enter 2D con: " + other.name);
        Cientifico player = other.GetComponent<Cientifico>();
        if (player != null && textoInteractuar != null)
        {
            textoInteractuar.enabled = false;
            cientificoCerca = null;
        }
    }

    void Update()
    {
        if (cientificoCerca != null && Input.GetKeyDown(KeyCode.E))
        {
            bool colocoAlguno = false;
            foreach (var id in componentesNecesarios)
            {
                if (!componentesInsertados.Contains(id) && cientificoCerca.inventory.CheckItem(id, 1))
                {
                    Item item = BuscarItemPorId(id);
                    if (item != null && cientificoCerca.inventory.RemoverItem(id, 1))
                    {
                        componentesInsertados.Add(id);
                        colocoAlguno = true;
                        textoInteractuar.text = $"Colocaste componente {id}!";
                        VerificarReparado();
                        if (gameManager != null)
                            gameManager.ComprobarVictoria();
                        break; // Solo coloca uno por pulsaci�n
                    }
                }
            }
            if (!colocoAlguno)
            {
                textoInteractuar.text = "No tienes componentes para colocar";
            }
        }
    }

    Item BuscarItemPorId(int id)
    {
        foreach (var item in todosLosComponentes)
        {
            if (item != null && item.id == id)
                return item;
        }
        return null;
    }

    void VerificarReparado()
    {
        reparado = componentesNecesarios.TrueForAll(x => componentesInsertados.Contains(x));
        if (reparado && textoInteractuar != null)
        {
            textoInteractuar.text = "�Estabilizador reparado!";
        }
    }

    public void Resetear()
    {
        componentesInsertados.Clear();
        reparado = false;
        if (textoInteractuar != null)
            textoInteractuar.enabled = false;
    }
}