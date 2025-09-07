using UnityEngine;

public enum StatType
{
    MaxHealth,
    HeartDropRate,
    InvinsibleTime,
    DodgeRate,
    MoveSpeedMultiplier,
    ReverseGravCoolTime,
    ReverseGravFallSpeed,
    MaxJump,
    GravAttackDamage,
    GravAttackMinSpeed,
    BoxSpawnCoolTime,
    PortalSpawnCoolTime,
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
    [Header("InGame Dynamic")]
    public int currentHealth;
    public float currentExp = 0;
    public int currentLevel = 0;

    [Header("Level")]
    public int[] currentStatLevels;

    [Header("Health & Dodge")]
    public int maxHealth; // TODO :: 최대 체력 올리면 체력도 그만큼 올라가는 식으로
    public float heartDropRate; // %
    public float invinsibleTime;
    public float dodgeRate; // %

    [Header("Movement")]
    public float moveSpeedMultiplier; // 적용 완료
    public float reverseGravCoolTime; // 적용 완료
    public float reverseGravFallSpeed;
    public int maxJump = 2;

    [Header("Gravity Attack")]
    public float gravAttackDamage; // 
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
    }
}