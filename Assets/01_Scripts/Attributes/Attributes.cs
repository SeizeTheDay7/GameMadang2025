using EditorAttributes;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    [Header(" - Attributes - ")]
    [SerializeField, Min(0)] float maxHealth;
    [ReadOnly, SerializeField] float currentHealth;
    [field: SerializeField] public SO_Stat Stat { get; private set; }

    Collider2D col;
    public Vector3 Center { get => col.bounds.center; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        currentHealth = maxHealth;

        TryGetComponent(out col);
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log("Current Health: " + currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}
