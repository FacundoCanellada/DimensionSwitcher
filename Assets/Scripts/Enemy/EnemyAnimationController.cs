using UnityEngine;

/// <summary>
/// Estados de animación del enemigo
/// </summary>
public enum EnemyAnimationState
{
    Idle = 0,
    Running = 1,
    Attacking = 2,
    Death = 3
}

/// <summary>
/// Controlador de animaciones para enemigos
/// Maneja estados (Idle, Run, Attack, Death) y 4 direcciones usando Blend Trees
/// </summary>
public class EnemyAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Configuración")]
    public float attackDuration = 0.5f;
    public float deathDuration = 1f;
    
    // Variables internas
    private Vector2 currentDirection = Vector2.down;
    private EnemyAnimationState currentState = EnemyAnimationState.Idle;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackTimer = 0f;
    
    // Sistema de sprites dinámicos
    private EnemyTypeData currentTypeData;
    private int currentSpriteVariant = 0;
    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private float frameRate = 8f; // 8 FPS
    
    // Eventos
    public System.Action OnAttackComplete;
    public System.Action OnDeathComplete;
    
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (animator == null)
        {
            Debug.LogError("No se encontró Animator en EnemyAnimationController!");
            return;
        }
        
        // Obtener configuración del tipo de enemigo
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            currentTypeData = enemy.enemyTypeData;
            currentSpriteVariant = enemy.spriteVariant;
        }
        
        // Configurar animación inicial
        UpdateAnimation();
    }
    
    void Update()
    {
        // Manejar timer de ataque
        if (isAttacking && !isDead)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                CompleteAttack();
            }
        }
        
        // Actualizar sprites manualmente
        UpdateSpriteAnimation();
    }
    
    /// <summary>
    /// Actualizar movimiento del enemigo
    /// </summary>
    public void UpdateMovement(Vector2 movement)
    {
        if (isAttacking || isDead) return;
        
        // Si hay movimiento, actualizar dirección
        if (movement.magnitude > 0.1f)
        {
            currentDirection = movement.normalized;
            if (currentState != EnemyAnimationState.Running)
            {
                currentState = EnemyAnimationState.Running;
                UpdateAnimation();
            }
        }
        else
        {
            if (currentState != EnemyAnimationState.Idle)
            {
                currentState = EnemyAnimationState.Idle;
                UpdateAnimation();
            }
        }
    }
    
    /// <summary>
    /// Establecer dirección sin movimiento (para idle direccional)
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (isDead) return;
        
        if (direction.magnitude > 0.1f)
        {
            currentDirection = direction.normalized;
        }
    }
    
    /// <summary>
    /// Iniciar ataque
    /// </summary>
    public void StartAttack()
    {
        if (isAttacking || isDead) return;
        
        isAttacking = true;
        attackTimer = 0f;
        currentState = EnemyAnimationState.Attacking;
        UpdateAnimation();
    }
    
    /// <summary>
    /// Terminar ataque
    /// </summary>
    private void CompleteAttack()
    {
        if (isDead) return;
        
        isAttacking = false;
        
        // Volver a idle
        currentState = EnemyAnimationState.Idle;
        UpdateAnimation();
        
        OnAttackComplete?.Invoke();
    }
    
    /// <summary>
    /// Iniciar animación de muerte
    /// </summary>
    public void StartDeath()
    {
        if (isDead) return;
        
        isDead = true;
        isAttacking = false;
        currentState = EnemyAnimationState.Death;
        UpdateAnimation();
        
        // Terminar muerte después del tiempo especificado
        Invoke(nameof(CompleteDeath), deathDuration);
    }
    
    /// <summary>
    /// Completar animación de muerte
    /// </summary>
    private void CompleteDeath()
    {
        OnDeathComplete?.Invoke();
    }
    
    /// <summary>
    /// ¿Está atacando?
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    /// <summary>
    /// ¿Está muerto?
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
    
    /// <summary>
    /// Obtener dirección actual
    /// </summary>
    public Vector2 GetCurrentDirection()
    {
        return currentDirection;
    }
    
    /// <summary>
    /// Obtener estado actual
    /// </summary>
    public EnemyAnimationState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Actualiza la animación de sprites manualmente
    /// </summary>
    private void UpdateSpriteAnimation()
    {
        if (currentTypeData == null || spriteRenderer == null) return;
        
        // Actualizar timer de frame
        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            
            // Obtener el array de sprites actual
            Sprite[] currentSprites = GetCurrentSpriteArray();
            if (currentSprites != null && currentSprites.Length > 0)
            {
                // Avanzar frame
                if (currentState == EnemyAnimationState.Idle || currentState == EnemyAnimationState.Running)
                {
                    currentFrameIndex = (currentFrameIndex + 1) % currentSprites.Length; // Loop
                }
                else if (currentState == EnemyAnimationState.Attacking || currentState == EnemyAnimationState.Death)
                {
                    if (currentFrameIndex < currentSprites.Length - 1)
                        currentFrameIndex++;
                    // No loop para attack y death
                }
                
                // Aplicar sprite
                spriteRenderer.sprite = currentSprites[currentFrameIndex];
            }
        }
    }
    
    /// <summary>
    /// Obtiene el array de sprites apropiado según estado y dirección
    /// </summary>
    private Sprite[] GetCurrentSpriteArray()
    {
        if (currentTypeData == null) return null;
        
        // Determinar dirección
        bool isLeft = currentDirection.x < -0.5f;
        bool isRight = currentDirection.x > 0.5f;
        bool isUp = currentDirection.y > 0.5f;
        bool isDown = currentDirection.y < -0.5f || (!isLeft && !isRight && !isUp);
        
        // Seleccionar array según estado y dirección
        switch (currentState)
        {
            case EnemyAnimationState.Idle:
                if (isDown) return currentTypeData.idleDown;
                if (isLeft) return currentTypeData.idleLeft;
                if (isRight) return currentTypeData.idleRight;
                if (isUp) return currentTypeData.idleUp;
                break;
                
            case EnemyAnimationState.Running:
                if (isDown) return currentTypeData.runDown;
                if (isLeft) return currentTypeData.runLeft;
                if (isRight) return currentTypeData.runRight;
                if (isUp) return currentTypeData.runUp;
                break;
                
            case EnemyAnimationState.Attacking:
                if (isDown) return currentTypeData.attackDown;
                if (isLeft) return currentTypeData.attackLeft;
                if (isRight) return currentTypeData.attackRight;
                if (isUp) return currentTypeData.attackUp;
                break;
                
            case EnemyAnimationState.Death:
                if (isDown) return currentTypeData.deathDown;
                if (isLeft) return currentTypeData.deathLeft;
                if (isRight) return currentTypeData.deathRight;
                if (isUp) return currentTypeData.deathUp;
                break;
        }
        
        return null;
    }
    
    /// <summary>
    /// Actualiza el estado de animación
    /// </summary>
    private void UpdateAnimation()
    {
        // Reset frame cuando cambia el estado
        currentFrameIndex = 0;
        frameTimer = 0f;
    }
}