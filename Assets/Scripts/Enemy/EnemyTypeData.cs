using UnityEngine;

/// <summary>
/// Configuración de un tipo de enemigo específico
/// Permite personalizar stats, sprites y comportamiento
/// </summary>
[CreateAssetMenu(fileName = "EnemyTypeData", menuName = "DimensionSwitcher/Enemy Type Data")]
public class EnemyTypeData : ScriptableObject
{
    [Header("Información Básica")]
    public EnemyType enemyType;
    public string enemyName;
    
    [Header("Estadísticas")]
    public int health = 10;
    public int damage = 5;
    public float speed = 3f;
    public float detectionRange = 8f;
    public float attackCooldown = 1.5f;
    
    [Header("Sprites - Idle (4 direcciones)")]
    public Sprite[] idleDown = new Sprite[8];
    public Sprite[] idleLeft = new Sprite[8];
    public Sprite[] idleRight = new Sprite[8];
    public Sprite[] idleUp = new Sprite[8];
    
    [Header("Sprites - Run (4 direcciones)")]
    public Sprite[] runDown = new Sprite[8];
    public Sprite[] runLeft = new Sprite[8];
    public Sprite[] runRight = new Sprite[8];
    public Sprite[] runUp = new Sprite[8];
    
    [Header("Sprites - Attack (4 direcciones)")]
    public Sprite[] attackDown = new Sprite[8];
    public Sprite[] attackLeft = new Sprite[8];
    public Sprite[] attackRight = new Sprite[8];
    public Sprite[] attackUp = new Sprite[8];
    
    [Header("Sprites - Death (4 direcciones)")]
    public Sprite[] deathDown = new Sprite[8];
    public Sprite[] deathLeft = new Sprite[8];
    public Sprite[] deathRight = new Sprite[8];
    public Sprite[] deathUp = new Sprite[8];
    
    [Header("Drops")]
    public Item[] possibleDrops;
    [Range(0f, 1f)]
    public float dropChance = 0.8f;
    public int dropMinAmount = 1;
    public int dropMaxAmount = 1;
    
    [Header("Colores/Efectos")]
    public Color tintColor = Color.white;
    public RuntimeAnimatorController animatorController;
}