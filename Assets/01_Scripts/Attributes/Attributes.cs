using EditorAttributes;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    [Header(" - Attributes - ")]
    [SerializeField, Min(0)] protected float maxHealth;
    [ReadOnly, SerializeField] protected float currentHealth;
    [field: SerializeField] public SO_Stat Stat { get; private set; }

    Collider2D col;
    public Vector3 Center { get => col.bounds.center; }

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        currentHealth = maxHealth;

        TryGetComponent(out col);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log("Current Health: " + currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}
