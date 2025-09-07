using UnityEngine;
using EditorAttributes;
using TMPro;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class Character : MonoBehaviour
{
    [Header("Components")]


    [Header("Stat Card UI")]
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] GameObject cardPanel;
    [SerializeField] StatUpCard[] cards;
    [SerializeField] StatUpCardSO[] cardSOs;

    [Header("Basic Stats")]
    [field: SerializeField] public CharacterStat stat { get; private set; }
    private Dictionary<StatType, StatUpCardSO> statUpCardSODict = new Dictionary<StatType, StatUpCardSO>();
    public Dictionary<string, List<float>> statUpInfoDict = new Dictionary<string, List<float>>();
    public List<float> expInfoDict = new List<float>();

    void Start()
    {
        InitStatData();
        InitExpData();

        InitStats();
        InitCards();
        InitCardSOs();
    }

    private void InitStats()
    {
        // 레벨들을 0으로 초기화하고, 스탯 업뎃 호출
        stat.currentStatLevels = new int[System.Enum.GetValues(typeof(StatType)).Length];
        for (int i = 0; i < stat.currentStatLevels.Length; i++)
            UpdateStat((StatType)i);
    }

    private void InitCards()
    {
        // 카드에 character 참조 주입
        foreach (var card in cards)
            card.character = this;
    }

    private void InitCardSOs()
    {
        // cardSO에 들어있는 type 기반으로 SO를 가져올 수 있게 할당
        foreach (var cardSO in cardSOs)
            statUpCardSODict[cardSO.statType] = cardSO;
    }

    private void InitStatData()
    {
        string filePath = Path.Combine(Application.dataPath, "StatSheet.csv");
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');
            string type = values[0];
            for (int j = 1; j < values.Length; j++)
            {
                if (statUpInfoDict.ContainsKey(type))
                {
                    if (string.IsNullOrWhiteSpace(values[j])) continue;
                    statUpInfoDict[type].Add(float.Parse(values[j]));
                }
                else
                    statUpInfoDict[type] = new List<float>() { float.Parse(values[j]) };
            }
        }

        foreach (var kvp in statUpInfoDict)
        {
            string type = kvp.Key;
            string values = string.Join(", ", kvp.Value.Select(v => v.ToString()));
            Debug.Log($"StatUpDict[{type}] = [{values}]");
        }
    }

    private void InitExpData()
    {
        string filePath = Path.Combine(Application.dataPath, "ExpSheet.csv");
        string[] lines = File.ReadAllLines(filePath);
        string[] expNeed = lines[1].Split(',');
        for (int i = 1; i < expNeed.Length; i++)
            expInfoDict.Add(float.Parse(expNeed[i]));

        string values = string.Join(", ", expInfoDict.Select(v => v.ToString()));
        Debug.Log($"LevelUpNeedEXP: [{values}]");
    }

    void Update()
    {
        debugText.text =
            $"CurrentHealth: {stat.currentHealth}\n" +
            $"MaxHealth: {stat.maxHealth}\n" +
            $"CurrentExp: {stat.currentExp}\n" +
            $"CurrentLevel: {stat.currentLevel}\n" +
            $"CurrentStatLevels: {string.Join(", ", stat.currentStatLevels)}\n" +
            $"MaxJump: {stat.maxJump}\n" +
            $"RunningSpeed: {stat.moveSpeedMultiplier}\n" +
            $"ReverseGravCoolTime: {stat.reverseGravCoolTime}\n" +
            $"ReverseGravFallSpeed: {stat.reverseGravFallSpeed}\n" +
            $"HeartDropRate: {stat.heartDropRate}\n" +
            $"InvinsibleTime: {stat.invinsibleTime}\n" +
            $"DodgeRate: {stat.dodgeRate}\n" +
            $"GravAttackDamage: {stat.gravAttackDamage}\n" +
            $"GravAttackMinSpeed: {stat.gravAttackMinSpeed}\n" +
            $"BoxSpawnCoolTime: {stat.boxSpawnCoolTime}\n" +
            $"PortalSpawnCoolTime: {stat.portalSpawnCoolTime}\n";
        // $"ThrowableMonster: {stat.throwableMonster}\n" +
        // $"AttackType: {stat.attackType}\n" +
        // $"FireInstantKill: {stat.fireInstantKill}\n" +
        // $"IceFreeze: {stat.iceFreeze}\n" +
        // $"ElectricSpread: {stat.electricSpread}\n";
    }

    public void GainEXP(float exp)
    {
        stat.currentExp += exp;
        // 만렙을 달성해서 데이터가 더 이상 없다면, 마지막 레벨의 경험치를 그대로 사용
        if (stat.currentLevel >= expInfoDict.Count)
        {
            if (stat.currentExp >= expInfoDict[expInfoDict.Count - 1])
            {
                stat.currentExp -= expInfoDict[expInfoDict.Count - 1];
                LevelUp();
            }
        }
        else if (stat.currentExp >= expInfoDict[stat.currentLevel])
        {
            stat.currentExp -= expInfoDict[stat.currentLevel];
            stat.currentLevel++;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        PauseGame();
        ShowLevelUpCard();
    }

    private void ShowLevelUpCard()
    {
        InitCardUIs(GetRandomStatType()); // 필요한 카드들의 텍스트를 업뎃
        cardPanel.SetActive(true);
    }

    // 랜덤 type 3개 반환
    private List<StatType> GetRandomStatType()
    {
        // 다음 레벨이 존재하는 StatType만 추출
        var availableTypes = cardSOs
            .Select(so => so.statType)
            .Where(type =>
            {
                int currentLevel = stat.currentStatLevels[(int)type];
                return statUpInfoDict.ContainsKey(type.ToString()) &&
                       currentLevel + 1 < statUpInfoDict[type.ToString()].Count;
            })
            .ToList();

        // 랜덤 셔플 후 3개 선택 (최대 3개)
        System.Random rng = new System.Random();
        return availableTypes.OrderBy(_ => rng.Next()).Take(3).ToList();
    }

    private void InitCardUIs(List<StatType> types)
    {
        foreach (GameObject card in cards.Select(c => c.gameObject))
            card.SetActive(false);

        for (int i = 0; i < types.Count; i++)
        {
            StatUpCard card = cards[i];
            card.gameObject.SetActive(true);
            StatType type = types[i];
            var statCardSO = statUpCardSODict[type];
            card.SetType(type);
            card.title.text = statCardSO.title;
            card.detail.text = statCardSO.description;
        }

        // type가 전혀 없으면 그냥 바로 게임 재개
        if (types.Count == 0)
        {
            cardPanel.SetActive(false);
            ResumeGame();
        }
    }

    public void StatUp(StatType type)
    {
        stat.currentStatLevels[(int)type] += 1;
        Debug.Log($"Stat Up! {type} is now level {stat.currentStatLevels[(int)type]}");
        UpdateStat(type);
        cardPanel.SetActive(false);
        ResumeGame();
    }

    // 스탯 하나를 레벨에 맞게 업데이트
    private void UpdateStat(StatType type)
    {
        int level = stat.currentStatLevels[(int)type];
        float data = statUpInfoDict[type.ToString()][level]; // 현재 레벨에 맞는 스탯을 가져옴

        switch (type)
        {
            case StatType.MaxHealth:
                stat.maxHealth = (int)data;
                break;
            case StatType.MaxJump:
                stat.maxJump = (int)data;
                break;
            case StatType.MoveSpeedMultiplier:
                stat.moveSpeedMultiplier = data;
                break;
            case StatType.ReverseGravCoolTime:
                stat.reverseGravCoolTime = data;
                break;
            case StatType.ReverseGravFallSpeed:
                stat.reverseGravFallSpeed = data;
                break;
            case StatType.HeartDropRate:
                stat.heartDropRate = data;
                break;
            case StatType.InvinsibleTime:
                stat.invinsibleTime = data;
                break;
            case StatType.DodgeRate:
                stat.dodgeRate = data;
                break;
            case StatType.GravAttackDamage:
                stat.gravAttackDamage = data;
                break;
            case StatType.GravAttackMinSpeed:
                stat.gravAttackMinSpeed = data;
                break;
            case StatType.BoxSpawnCoolTime:
                stat.boxSpawnCoolTime = data;
                break;
            case StatType.PortalSpawnCoolTime:
                stat.portalSpawnCoolTime = data;
                break;
            // case StatType.ThrowableMonster:
            //     stat.throwableMonster = (MonsterType)(int)data;
            //     break;
            // case StatType.AttackType:
            //     stat.attackType = (SpecialAttack)(int)data;
            //     break;
            // case StatType.FireInstantKill:
            //     stat.fireInstantKill = (int)data;
            //     break;
            // case StatType.IceFreeze:
            //     stat.iceFreeze = (int)data;
            //     break;
            // case StatType.ElectricSpread:
            //     stat.electricSpread = (int)data;
            //     break;
            default:
                Debug.LogError("Unknown StatType: " + type);
                break;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
