using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int saludEnemy = 100;
    public int daño = 10;
    public float rangoDeteccion = 10f;
    public float velocidad = 5f;
    public Item dropItem;

    Cientifico target;
    GameManager gameManager;

    float tiempoUltimoAtaque = 0f;
    public float tiempoEntreAtaques = 1f; // 1 segundo entre golpes

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        target = FindObjectOfType<Cientifico>();
    }

    void Update()
    {
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < rangoDeteccion)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, velocidad * Time.deltaTime);
                if (dist < 2f) // rango de ataque
                {
                    if (Time.time - tiempoUltimoAtaque > tiempoEntreAtaques)
                    {
                        target.RecibirDanio(daño, gameManager);
                        tiempoUltimoAtaque = Time.time;
                    }
                }
            }
        }
    }

    public void RecibirDanio(int amount)
    {
        saludEnemy -= amount;
        if (saludEnemy <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
    }

    void DropItem()
    {
        if (dropItem != null)
        {
            target.AgarrarItem(dropItem);
        }
    }
}