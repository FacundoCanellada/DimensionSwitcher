using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EstabilizadorCuantico : MonoBehaviour
{
    public List<int> componentesNecesarios = new() { 1, 2, 3 };
    public List<int> componentesInsertados = new();
    public bool reparado = false;

    [Header("Interacción")]
    public GameObject panelTexto; // Panel hijo con el texto (World Space)
    public TextMeshProUGUI textoInteractuar; // TextMeshPro dentro del panel
    private Cientifico cientificoCerca;

    [Header("GameManager")]
    public GameManager gameManager; // Asignar en el inspector

    [Header("Componentes disponibles (ItemSO)")]
    public List<Item> todosLosComponentes; // Asignar todos los ItemSO de tipo Components

    void Start()
    {
        // Ocultar el panel al inicio
        if (panelTexto != null)
            panelTexto.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Cientifico player = other.GetComponent<Cientifico>();
        if (player != null)
        {
            cientificoCerca = player;
            if (panelTexto != null && textoInteractuar != null)
            {
                panelTexto.SetActive(true);
                textoInteractuar.text = "Presiona E para colocar componente";
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Cientifico player = other.GetComponent<Cientifico>();
        if (player != null)
        {
            if (panelTexto != null)
                panelTexto.SetActive(false);
            
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
            textoInteractuar.text = "¡Estabilizador reparado!";
            Debug.Log("=== ESTABILIZADOR COMPLETAMENTE REPARADO ===");
            
            // Notificar al GameManager inmediatamente
            if (gameManager != null)
            {
                gameManager.ComprobarVictoria();
            }
        }
    }

    public void Resetear()
    {
        componentesInsertados.Clear();
        reparado = false;
        
        // Ocultar el panel al resetear
        if (panelTexto != null)
            panelTexto.SetActive(false);
    }
}