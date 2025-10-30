using UnityEngine;

/// <summary>
/// Controlador de animaciones SIMPLE para el personaje
/// Usa Blend Trees - mucho más fácil de configurar
/// </summary>
public class PlayerAnimationControllerSimple : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    
    [Header("Configuración")]
    public float attackDuration = 0.5f;
    
    // Variables internas
    private Vector2 currentDirection = Vector2.down;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    
    // Eventos
    public System.Action OnAttackComplete;
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (animator == null)
        {return;
        }
        
        // Configurar dirección inicial
        UpdateAnimator();
    }
    
    void Update()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                CompleteAttack();
            }
        }
    }
    
    /// <summary>
    /// Actualizar movimiento - SÚPER SIMPLE
    /// </summary>
    public void UpdateMovement(Vector2 movement)
    {
        if (isAttacking) return;
        
        // Si hay movimiento, actualizar dirección
        if (movement.magnitude > 0.1f)
        {
            currentDirection = movement.normalized;
            
            // Para el blend tree: movimiento rápido = run (multiplicar por 2)
            animator.SetFloat("DirectionX", currentDirection.x * 2f);
            animator.SetFloat("DirectionY", currentDirection.y * 2f);
        }
        else
        {
            // Idle - usar la última dirección pero con magnitud 1
            animator.SetFloat("DirectionX", currentDirection.x);
            animator.SetFloat("DirectionY", currentDirection.y);
        }
    }
    
    /// <summary>
    /// Iniciar ataque
    /// </summary>
    public void StartAttack()
    {
        if (isAttacking) return;
        
        isAttacking = true;
        attackTimer = 0f;
        
        // Configurar parámetros para ataque
        animator.SetBool("IsAttacking", true);
        animator.SetFloat("DirectionX", currentDirection.x);
        animator.SetFloat("DirectionY", currentDirection.y);
    }
    
    /// <summary>
    /// Terminar ataque
    /// </summary>
    private void CompleteAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        
        // Volver a la animación normal
        animator.SetFloat("DirectionX", currentDirection.x);
        animator.SetFloat("DirectionY", currentDirection.y);
        
        OnAttackComplete?.Invoke();
    }
    
    /// <summary>
    /// Actualizar animator (método interno)
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator == null) return;
        
        animator.SetFloat("DirectionX", currentDirection.x);
        animator.SetFloat("DirectionY", currentDirection.y);
    }
    
    /// <summary>
    /// ¿Está atacando?
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    /// <summary>
    /// Obtener dirección actual
    /// </summary>
    public Vector2 GetCurrentDirection()
    {
        return currentDirection;
    }
}