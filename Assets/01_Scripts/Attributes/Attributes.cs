using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Attributes : MonoBehaviour
{
    [Header(" - Attributes - ")]
    [SerializeField, Min(0)] protected float maxHealth;
    [ReadOnly, SerializeField] protected float currentHealth;
    [field: SerializeField] public SO_Stat Stat { get; private set; }
    [SerializeField] bool isPlayer;

    [Header(" - UI - ")]
    [SerializeField] Canvas hpCanvas;
    [SerializeField] Image hp;

    Collider col;
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

    [Button]
    public virtual void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        UpdateUI();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        UpdateUI();
    }

    [Button]
    protected virtual void Die()
    {
        if(isPlayer)
        {
            //¿£µù¾À
            return;
        }

        if(Stat.ItemDropPercentage >= Random.value)
        {
            Instantiate(Stat.Item, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        hpCanvas.enabled = true;
        hp.fillAmount = currentHealth / maxHealth;
    }
}
