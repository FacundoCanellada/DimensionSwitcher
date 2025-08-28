using UnityEngine;

public class Cientifico : MonoBehaviour
{
    public int salud = 100;
    public int stamina = 100;
    public int hambre = 100;
    public int sed = 100;
    public InventorySO inventory;
    public Item arma;
    public float velocidad = 5f;
    public SpriteRenderer armaRenderer;
    public float rangoAtaque = 2f; // Rango para atacar enemigos

    private bool controlHabilitado = true;
    private Vector3 posicionInicial;
    private Item armaInicial; // Guarda referencia al arma inicial (si la hay)

    void Start()
    {
        posicionInicial = transform.position;
        armaInicial = arma; // Guarda el arma inicial, si hay una al comenzar
    }

    void Update()
    {
        if (!controlHabilitado) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, v, 0); // Y es arriba/abajo en Unity 2D
        transform.position += dir.normalized * velocidad * Time.deltaTime;

        // Mostrar el arma en pantalla
        if (arma != null && arma.icon != null)
        {
            armaRenderer.sprite = arma.icon;
            armaRenderer.enabled = true;
        }
        else
        {
            armaRenderer.enabled = false;
        }

        // ATAQUE: presiona barra espaciadora para atacar
        if (Input.GetKeyDown(KeyCode.Space) && arma != null && arma.type == ItemType.Weapon)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, rangoAtaque);
        foreach (var hit in hits)
        {
            Enemy enemigo = hit.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(arma.weaponDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }

    public void RecibirDanio(int cantidad, GameManager gameManager)
    {
        salud -= cantidad;
        if (salud < 0) salud = 0;
        gameManager.ComprobarDerrota();
    }

    public void AgarrarItem(Item item)
    {
        inventory.AddItem(item, 1);
    }

    public void Resetear()
    {
        salud = 100;
        stamina = 100;
        hambre = 100;
        sed = 100;
        inventory.Clear();
        arma = armaInicial; // Restaura el arma inicial
        transform.position = posicionInicial; // Vuelve a la posición inicial
        armaRenderer.enabled = arma != null && arma.icon != null;
        controlHabilitado = true; // Reactiva el control después del reset
    }

    /// <summary>
    /// Habilita o deshabilita el control del jugador
    /// </summary>
    public void HabilitarControl(bool habilitado)
    {
        controlHabilitado = habilitado;
        // Opcional: desactivar el arma visualmente si está deshabilitado
        if (!habilitado && armaRenderer != null)
            armaRenderer.enabled = false;
    }
}