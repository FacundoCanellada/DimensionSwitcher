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
                        
                        // Reproducir sonido de componente colocado
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.SonidoComponenteColocado();
                        }
                        
                        // Notificar al QuestManager que el componente fue USADO (removerlo de recolectados)
                        if (gameManager != null)
                        {
                            QuestManager questManager = FindFirstObjectByType<QuestManager>();
                            if (questManager != null)
                            {
                                questManager.OnComponenteColocado(id);
                            }
                        }
                        
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
        
        Debug.Log($"VerificarReparado llamado. Componentes insertados: [{string.Join(", ", componentesInsertados)}]. Reparado: {reparado}");
        
        if (reparado && textoInteractuar != null)
        {
            textoInteractuar.text = "¡Estabilizador reparado!";
            
            Debug.Log("¡ESTABILIZADOR COMPLETAMENTE REPARADO! Notificando a GameManager...");
            
            // Reproducir sonido de estabilizador completo
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SonidoEstabilizadorCompleto();
            }
            
            // Notificar al GameManager inmediatamente
            if (gameManager != null)
            {
                Debug.Log("Llamando a gameManager.ComprobarVictoria()");
                gameManager.ComprobarVictoria();
            }
            else
            {
                Debug.LogError("GameManager es NULL! No se puede comprobar victoria.");
            }
        }
    }

    public void Resetear()
    {
        componentesInsertados.Clear();
        reparado = false;
        
        // Actualizar el texto para reflejar el estado reseteado
        if (textoInteractuar != null)
        {
            textoInteractuar.text = "Presiona E para colocar componentes";
        }
        
        // Ocultar el panel al resetear
        if (panelTexto != null)
            panelTexto.SetActive(false);
        
        Debug.Log("Estabilizador reseteado - Componentes eliminados");
    }
}