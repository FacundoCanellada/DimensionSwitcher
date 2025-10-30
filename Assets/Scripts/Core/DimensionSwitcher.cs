using UnityEngine;

public class DimensionSwitcher : MonoBehaviour
{
    void Start()
    {
        MostrarTodosLosObjetos();
    }
    
    private void MostrarTodosLosObjetos()
    {
        MostrarObjetosPorTag("Enemy");
        MostrarObjetosPorTag("Estabilizador");
        MostrarObjetosPorTag("Item");
    }
    
    private void MostrarObjetosPorTag(string tag)
    {
        GameObject[] objetos = GameObject.FindGameObjectsWithTag(tag);
        
        foreach (GameObject obj in objetos)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = true;
            
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            
            Collider2D[] colliders = obj.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in colliders)
                collider.enabled = true;
            
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
                enemy.enabled = true;
        }
    }
    
    public void Resetear()
    {
        MostrarTodosLosObjetos();
    }
}
