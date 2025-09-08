using System.Collections;
using UnityEngine;

public enum StatType
{
    MaxHealth, // 적용 완 // TODO :: 최대 체력 올리면 체력도 그만큼 올라가는 식으로
    HeartDropRate,
    InvinsibleTime, // 적용 완
    DodgeRate, // 적용 완
    MoveSpeedMultiplier, // 적용 완
    ReverseGravCoolTime, // 적용 완
    MaxJump, // 적용 완
    GravAttackDamage, // 적용 완
    GravAttackMinSpeed, // 적용 완
    BoxSpawnCoolTime,// 적용 완
    // ThrowableMonster,
    // AttackType,
    // FireInstantKill,
    // IceFreeze,
    // ElectricSpread
}

public enum MonsterType { None, Small, Medium, Big, Boss }
public enum SpecialAttack { None, Fire, Ice, Electric }

public class CharacterStat : MonoBehaviour
{
    public Character character;
    [Header("InGame Dynamic")]
    public int currentHealth;
    public float currentExp = 0;
    public int currentLevel = 0;
    public bool isInvinsible { get; private set; } = false;
    SpriteRenderer[] spriteRenderer;

    [Header("Level")]
    public int[] currentStatLevels;

    [Header("Health & Dodge")]
    public int maxHealth;
    public float heartDropRate; // %
    public float invinsibleTime;
    public float dodgeRate; // %

    [Header("Movement")]
    public float moveSpeedMultiplier;
    public float reverseGravCoolTime;
    public int maxJump = 2;

    [Header("Gravity Attack")]
    public float gravAttackDamage;
    public float gravAttackMinSpeed;

    [Header("Spawn Object & Enemy")]
    public float boxSpawnCoolTime;
    public float portalSpawnCoolTime;

    // [Header("Special Attack")]
    // public MonsterType throwableMonster = MonsterType.None;
    // public SpecialAttack attackType = SpecialAttack.None;
    // public int fireInstantKill;
    // public int iceFreeze;
    // public int electricSpread;

    public void Awake()
    {
        currentStatLevels = new int[System.Enum.GetValues(typeof(StatType)).Length];
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void RaiseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        // TODO :: UI 업뎃, 혹은 프로퍼티로 연동
    }

    public void HealHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        // TODO :: UI 업뎃
    }

    public void TakeDamage(int damage)
    {
        // 만약 무적 시간이라면 데미지를 안 받고 그대로 return
        // 회피 시도가 성공했다면 dodge() 호출하고 그대로 return
        // 피격당했을 때 체력이 0이 됐다면 사망
        // 피격당했을 때 무적 시간 시작
        if (isInvinsible) return;
        if (Random.value < dodgeRate * 0.01f) { Dodge(); return; }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        GlobalData.OnHPChange?.Invoke((float)((float)currentHealth / maxHealth));
        if (currentHealth <= 0) { Dead(); }
        else { StartCoroutine(CoInvinsible()); }
    }

    private void Dodge()
    {
        // TODO :: 회피했다는 UI 뜸
        Debug.Log("Dodged!");
    }

    private IEnumerator CoInvinsible()
    {
        isInvinsible = true;
        // TODO :: sprite renderer 투명해짐
        foreach (var sr in spriteRenderer)
        {
            Color color = sr.color;
            color.a = 0.5f;
            sr.color = color;
        }
        yield return new WaitForSeconds(invinsibleTime);
        foreach (var sr in spriteRenderer)
        {
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }
        isInvinsible = false;
    }

    private void Dead()
    {
       SceneChangeManager.Instance.LoadSceneAsync("GameOver");
    }
}