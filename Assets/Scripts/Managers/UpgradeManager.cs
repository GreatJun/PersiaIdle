using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<EStatusType, int> onTrainingTypeAndCurrentLevel;
    public event Action<EStatusType, int> onAwakenUpgrade;
    public event Action<EStatusType, int> onAbilityUpgrade;
    public event Action<int> onBaseAttackUpgrade;
    public event Action<int> onBaseHealthUpgrade;
    public event Action<float> onBaseDamageReductionUpgrade;
    public event Action<int> onBaseManaUpgrade;
    public event Action<int> onBaseRecoveryUpgrade;
    public event Action<float> onBaseCriticalChanceUpgrade;
    public event Action<int> onBaseCriticalDamageUpgrade;

    public event Action<float> onBaseAttackSpeedUpgrade;

    // public event Action<float> onBaseAttackRangeUpgrade;
    public event Action<float> onBaseMovementSpeedUpgrade;

    public event Action<int> onAwakenAttack;
    public event Action<float> onAwakenDamageReduction;
    public event Action<float> onAwakenCriticalChance;
    public event Action<int> onAwakenCriticalDamage;
    public event Action<float> onAwakenAttackSpeed;
    public event Action<int> onAwakenSkillMultiplier;

    [field: SerializeField] public StatUpgradeInfo[] statUpgradeInfo { get; protected set; }

    [field: SerializeField] public AwakenUpgradeInfo[] awakenUpgradeInfo { get; protected set; }
    
    /* ============================================================================================================= */
    
    [field: SerializeField] public AbilityUpgradeInfo[] abilityUpgradeInfo { get; protected set; }

    // [field: SerializeField] public SpecialityUpgradeInfo[] specialityUpgradeInfo { get; protected set; }
    // [field: SerializeField] public RelicUpgradeInfo[] relicUpgradeInfo { get; protected set; }
    
    /* ============================================================================================================= */
    public void InitStatus(EStatusType type, BigInteger value)
    {
        PlayerManager.instance.status.ChangeBaseStat(type, value);
    }

    public void InitStatus(EStatusType type, float value)
    {
        PlayerManager.instance.status.ChangeBaseStat(type, value);
    }

    public void InitAwaken(EStatusType type, BigInteger value)
    {
        PlayerManager.instance.status.ChangePercentStat(type, value);
    }

    public void InitAwaken(EStatusType type, float value)
    {
        PlayerManager.instance.status.ChangePercentStat(type, value);
    }

    public void UpgradeBaseStatus(StatUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        
        if (info.upgradePerLevelInt != 0)
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, info.upgradePerLevelInt);
        else
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, info.upgradePerLevelFloat);
        
        switch (info.statusType)
        {
            case EStatusType.ATK:
                onBaseAttackUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.HP:
                onBaseHealthUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP:
                onBaseManaUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP_RECO:
                onBaseRecoveryUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.CRIT_DMG:
                onBaseCriticalDamageUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.DMG_REDU:
                onBaseDamageReductionUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.CRIT_CH:
                onBaseCriticalChanceUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.ATK_SPD:
                onBaseAttackSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.MOV_SPD:
                onBaseMovementSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
        }

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        
        info.LevelUp();

        onTrainingTypeAndCurrentLevel?.Invoke(info.statusType, info.level);
    }
    
    /* ============================================================================================================= */

    /// <summary>
    /// Ability Upgrade Info
    /// </summary>
    ///
    private int saveAbility = 0;
    public void UpgradeBaseStatus(AbilityUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        
        // 이전 업그레이드 원상복귀
        if (saveAbility != 0)
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, -saveAbility);
        
        RandomRank();
        RandomAbilityUp(info);
        saveAbility = randomAbility;
        
        // 새로 업그레이드
        if (info.upgradePerLevelInt != 0)
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, randomAbility);
        else
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, randomAbility);
        
        switch (info.statusType)
        {
            case EStatusType.ATK:
                onBaseAttackUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.HP:
                onBaseHealthUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP:
                onBaseManaUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP_RECO:
                onBaseRecoveryUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.CRIT_DMG:
                onBaseCriticalDamageUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.DMG_REDU:
                onBaseDamageReductionUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.CRIT_CH:
                onBaseCriticalChanceUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.ATK_SPD:
                onBaseAttackSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.MOV_SPD:
                onBaseMovementSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
        }

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        
        info.LevelUp();

        onAbilityUpgrade?.Invoke(info.statusType, info.level);
    }

    private int randomRank;
    [HideInInspector] public string rank = "";
    
    public void RandomRank()
    {
        randomRank = UnityEngine.Random.Range(1, 101);

        if (randomRank <= 1)
            rank = "SS";
        else if (randomRank <= 10)
            rank = "S";
        else if (randomRank <= 30)
            rank = "A";
        else if (randomRank <= 60)
            rank = "B";
        else
            rank = "C";
    }

    [HideInInspector] public int randomAbility;
    int upgradeType;
    public void RandomAbilityUp(AbilityUpgradeInfo info)
    {
        if (info.statusType == EStatusType.ATK)
            upgradeType = 0;
        else if (info.statusType == EStatusType.HP)
            upgradeType = 0;
        else if (info.statusType == EStatusType.CRIT_CH)
            upgradeType = 0;
        else if (info.statusType == EStatusType.DMG_REDU)
            upgradeType = 0;
        
        switch (rank)
        {
            case "SS":
                if (upgradeType == 0)
                    randomAbility = UnityEngine.Random.Range(81, 101);
                else if (upgradeType == 1)
                    randomAbility = UnityEngine.Random.Range(89, 111);
                else if (upgradeType == 2)
                    randomAbility = UnityEngine.Random.Range(41, 51);
                else if (upgradeType == 3)
                    randomAbility = UnityEngine.Random.Range(21, 26);
                break;
            case "S":
                if (upgradeType == 0)
                    randomAbility = UnityEngine.Random.Range(61, 81);
                else if (upgradeType == 1)
                    randomAbility = UnityEngine.Random.Range(67, 88);
                else if (upgradeType == 2)
                    randomAbility = UnityEngine.Random.Range(31, 41);
                else if (upgradeType == 3)
                    randomAbility = UnityEngine.Random.Range(16, 21);
                break;
            case "A":
                if (upgradeType == 0)
                    randomAbility = UnityEngine.Random.Range(41, 61);
                else if (upgradeType == 1)
                    randomAbility = UnityEngine.Random.Range(45, 67);
                else if (upgradeType == 2)
                    randomAbility = UnityEngine.Random.Range(21, 31);
                else if (upgradeType == 3)
                    randomAbility = UnityEngine.Random.Range(11, 15);
                break;
            case "B":
                if (upgradeType == 0)
                    randomAbility = UnityEngine.Random.Range(21, 41);
                else if (upgradeType == 1)
                    randomAbility = UnityEngine.Random.Range(23, 45);
                else if (upgradeType == 2)
                    randomAbility = UnityEngine.Random.Range(11, 21);
                else if (upgradeType == 3)
                    randomAbility = UnityEngine.Random.Range(6, 11);
                break;
            case "C":
                if (upgradeType == 0)
                    randomAbility = UnityEngine.Random.Range(1, 21);
                else if (upgradeType == 1)
                    randomAbility = UnityEngine.Random.Range(11, 23);
                else if (upgradeType == 2)
                    randomAbility = UnityEngine.Random.Range(1, 11);
                else if (upgradeType == 3)
                    randomAbility = UnityEngine.Random.Range(1, 6);
                break;
        }
    }
    
    /* ============================================================================================================= */

    public void UpgradePercentStatus(AwakenUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        
        if (info.upgradePerLevelInt != 0)
            PlayerManager.instance.status.ChangePercentStat(info.statusType, new BigInteger(info.upgradePerLevelInt));
        else
            PlayerManager.instance.status.ChangePercentStat(info.statusType, info.upgradePerLevelFloat);
        
        switch (info.statusType)
        {
            case EStatusType.ATK:
                onAwakenAttack?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.CRIT_DMG:
                onAwakenCriticalDamage?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.SKILL_DMG:
                onAwakenSkillMultiplier?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.DMG_REDU:
                onAwakenDamageReduction?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.CRIT_CH:
                onAwakenCriticalChance?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.ATK_SPD:
                onAwakenAttackSpeed?.Invoke(info.upgradePerLevelFloat);
                break;
        }
        
        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        
        info.LevelUP();
        
        onAwakenUpgrade?.Invoke(info.statusType, info.level);
    }

    public void InitUpgradeManager()
    {
        // TODO Save & Load Upgrade Information
        if (ES3.KeyExists("Init_Game"))
        {
            LoadUpgradeInfo();
        }
        else
        {
            InitUpgradeInfo();
        }
    }

    private void InitUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Init();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Init();
        }
    }

    public void LoadUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Load();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Load();
        }
    }

    public void SaveUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Save();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Save();
        }
    }
}

[Serializable]
public class AwakenUpgradeInfo
{
    public string gemName => info.gemName;
    public string title => info.title;
    public int level;
    public int maxLevel => info.maxLevel;

    // 업글 관련
    public EStatusType statusType => info.statusType;
    
    public int upgradePerLevelInt => info.upgradePerLevelInt;
    public float upgradePerLevelFloat => info.upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType => info.currencyType;
    public int baseCost => info.baseCost;
    public int increaseCostPerLevel => info.increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image => info.image;

    [SerializeField] private AwakenUpgradeFixedInfo info;

    public void LevelUP()
    {
        ++level;
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        DataManager.Instance.Save($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}",
            cost.ToString());
    }

    public void Load()
    {
        level = DataManager.Instance.Load($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}",
            level);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitAwaken(statusType, (new BigInteger(upgradePerLevelInt)) * level);
        else
            UpgradeManager.instance.InitAwaken(statusType, (upgradePerLevelFloat) * level);
    }

    public bool CheckUpgradeCondition()
    {
        if (level >= maxLevel || cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }


    public void Init()
    {
        level = 0;
        cost = baseCost;
    }
}


[Serializable]
public class StatUpgradeInfo
{
    public string title => info.title;
    public int level;
    public int maxLevel => info.maxLevel;

    // 업글 관련
    public EStatusType statusType => info.statusType;
    
    public int upgradePerLevelInt => info.upgradePerLevelInt;
    
    public float upgradePerLevelFloat => info.upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType => info.currencyType;
    public int baseCost => info.baseCost;
    public int increaseCostPerLevel => info.increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image => info.image;

    [SerializeField] private StatUpgradeFixedInfo info;
    
    public void LevelUp()
    {
        ++level;
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", cost.ToString());
    }

    public void Load()
    {
        level = DataManager.Instance.Load($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitStatus(statusType, (new BigInteger(upgradePerLevelInt)) * level);
        else
            UpgradeManager.instance.InitStatus(statusType, (upgradePerLevelFloat) * level);
    }

    public bool CheckUpgradeCondition()
    {
        if (level >= maxLevel || cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }

    public void Init()
    {
        level = 0;
        cost = baseCost;
    }
}


/* ============================================================================================================= */
/* ============================================================================================================= */
/* ============================================================================================================= */


[Serializable]
public class AbilityUpgradeInfo
{
    public string title => info.title;
    public int level;
    public int maxLevel => info.maxLevel;

    // 업글 관련
    public EStatusType statusType => info.statusType;
    
    public int upgradePerLevelInt => info.upgradePerLevelInt;
    
    public float upgradePerLevelFloat => info.upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType => info.currencyType;
    public int baseCost => info.baseCost;
    public int increaseCostPerLevel => info.increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image => info.image;

    [SerializeField] private AbilityUpgradeFixedInfo info;
    
    public void LevelUp()
    {
        ++level;
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", cost.ToString());
    }

    public void Load()
    {
        level = DataManager.Instance.Load($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitStatus(statusType, (new BigInteger(upgradePerLevelInt)) * level);
        else
            UpgradeManager.instance.InitStatus(statusType, (upgradePerLevelFloat) * level);
    }

    public bool CheckUpgradeCondition()
    {
        if (level >= maxLevel || cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }

    public void Init()
    {
        level = 0;
        cost = baseCost;
    }
}