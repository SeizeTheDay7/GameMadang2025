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

    // Attributes의 TakeDamage는 적이 갖고 있는 것만 호출된다.
    // 플레이어는 Projectile에게서 직접 데미지를 받는다.
    // CharacterStat은 grabbableObject가 갖고있던 참조를 갖고 와서, 죽을 때 사용한다.
    public virtual void TakeDamage(CharacterStat stat)
    {
        if (isPlayer) return;

        currentHealth = Mathf.Max(currentHealth - stat.gravAttackDamage, 0);
        Debug.Log("Current Health: " + currentHealth);
        UpdateUI();

        if (currentHealth == 0)
        {
            Die(stat);
        }
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

        UpdateUI();
    }

    [Button]
    // 죽는 것 역시 플레이어는 CharacterStat에서만 일어난다.
    // 이 Die는 적이 죽는 것.
    protected virtual void Die(CharacterStat stat)
    {
        Debug.Log(gameObject.name + " died.");

        // if (isPlayer)
        // {
        //     //������
        //     return;
        // }

        stat.character.GainEXP(Stat.exp);

        if (Stat.ItemDropPercentage >= Random.value)
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
