using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<EStatusType, int> onTrainingTypeAndCurrentLevel;
    public event Action<EStatusType, int> onAwakenUpgrade;
    public event Action<EStatusType> onAbilityUpgrade;
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
    
    [field: SerializeField] public AbilityEffect[] AbilityEffects { get; set; }
    
    [field: SerializeField] public AbilityConsumption[] abilityConsumptionInfo { get; set; }
    
    [field: SerializeField] public AbilityPercentage[] abilityPercentageInfo { get; set; }
    

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

    public void InitAbilityLength(int length)
    {
        abilityUpgradeInfo = new AbilityUpgradeInfo[length];
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
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, new BigInteger(randomAbility));
        

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);

        info.title = effects.title.ToString();
        info.rank = rank;
        info.randomAbilityIndex = randomAbility;
        info.saveAbilityIndex = saveAbility;
        
        info.LevelUp();
        info.Save();

        onAbilityUpgrade?.Invoke(info.statusType);
    }

    private int randomRank;
    [HideInInspector] public string rank;
    
    public void RandomRank()
    {
        //string newRank = abilityPercentageInfo[1].abilityRank;
        randomRank = UnityEngine.Random.Range(1, 101);

        foreach (var percentPerRank in abilityPercentageInfo)
        {
            if (randomRank <= percentPerRank.abilityPercentage)
            {
                rank = percentPerRank.abilityRank;
                break;
            }
            else
            {
                randomRank -= percentPerRank.abilityPercentage;
            }
        }
    }

    [HideInInspector] public int randomAbility;
    [HideInInspector] public AbilityEffect effects = default;
    int upgradeType;

    // 예시 참고
    private EStatusType[] randomAbilityType = new EStatusType[] { EStatusType.ATK , EStatusType.HP, EStatusType.CRIT_DMG, EStatusType.SKILL_DMG};
    public void RandomAbilityUp(AbilityUpgradeInfo info)
    {
        upgradeType = UnityEngine.Random.Range(0, 4);
        info.statusType = randomAbilityType[upgradeType];
        
        // 타입 체크 -> 랭크 체크 -> 랜덤 돌리기 -> 적용
        foreach (var typeCheck in AbilityEffects)
        {
            if (info.statusType == typeCheck.abilityStat && rank == typeCheck.rank)
            {
                effects = typeCheck;
                break;
            }
        }
        
        randomAbility = Random.Range(effects.min, effects.max + 1);
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
        
        foreach (var upgradeInfo in abilityUpgradeInfo)
        {
            upgradeInfo.Init();
        }
    }

    /// <summary>
    /// 업그레이드 로드
    /// </summary>
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

        /*================== 수정 필요 임시 i값 데이터로 변환 해야함 ==================*/
        /*================== 수정 필요 임시 i값 데이터로 변환 해야함 ==================*/
        int i = 0;
        foreach (var upgradeInfo in abilityUpgradeInfo)
        {
            upgradeInfo.level = i;
            upgradeInfo.Load();
            i++;
        }
        /*================== 수정 필요 임시 i값 데이터로 변환 해야함 ==================*/
        /*================== 수정 필요 임시 i값 데이터로 변환 해야함 ==================*/
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

        foreach (var upgradeInfo in abilityUpgradeInfo)
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
    public string title;

    public string rank;

    public int randomAbilityIndex;

    public int saveAbilityIndex;
    
    // 업글 관련
    public EStatusType statusType;

    public int upgradePerLevelInt;

    public float upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType;
    public int baseCost;
    public int increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image;

    private AbilityConsumption abilityIndex;

    public int level;
    
    // 업그레이드
    // 확률에 따른 등급 변경
    public void LevelUp()
    {
        Debug.Log(level);
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(level)}_{level.ToString()}_{nameof(title)}", title);
        DataManager.Instance.Save($"{nameof(level)}_{level.ToString()}_{nameof(rank)}", rank);
        DataManager.Instance.Save($"{nameof(level)}_{level.ToString()}_{nameof(randomAbilityIndex.ToString)}", randomAbilityIndex);
        DataManager.Instance.Save($"{nameof(level)}_{level.ToString()}_{nameof(saveAbilityIndex)}", saveAbilityIndex);
        DataManager.Instance.Save($"{nameof(level)}_{level.ToString()}_{nameof(cost)}", cost.ToString());
    }
    
    public void Load()
    {
        title = DataManager.Instance.Load($"{nameof(level)}_{level.ToString()}_{nameof(title)}", title);
        rank = DataManager.Instance.Load($"{nameof(level)}_{level.ToString()}_{nameof(rank)}", rank);
        randomAbilityIndex = DataManager.Instance.Load($"{nameof(level)}_{level.ToString()}_{nameof(randomAbilityIndex)}", randomAbilityIndex);
        saveAbilityIndex = DataManager.Instance.Load($"{nameof(level)}_{level.ToString()}_{nameof(saveAbilityIndex)}", saveAbilityIndex);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(level)}_{level.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitAwaken(statusType, (new BigInteger(upgradePerLevelInt)));
    }
    
    public bool CheckAbilityUpgradeCondition()
    {
        if (cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }

    public void Init()
    {
        cost = baseCost;
        for (int i = 0; i < UpgradeManager.instance.abilityConsumptionInfo.Length; i++)
        {
            level = UpgradeManager.instance.abilityConsumptionInfo[i].abilityLevel;
        }
    }
}

[Serializable]
public struct AbilityEffect
{
    public string title;
    public EStatusType abilityStat;
    public string rank;
    public int min;
    public int max;
    public string currency;

    public AbilityEffect(string title, EStatusType abilityStat, string rank, int min, int max, string currency)
    {
        this.title = title;
        this.abilityStat = abilityStat;
        this.rank = rank;
        this.min = min;
        this.max = max;
        this.currency = currency;
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(AbilityEffect)}_{rank}_{nameof(rank)}", rank);
        DataManager.Instance.Save($"{nameof(UpgradeManager)}_{UpgradeManager.instance.effects.title}_{nameof(title)}", UpgradeManager.instance.effects.title);
    }
}

[Serializable]
public struct AbilityPercentage
{
    public string abilityRank;
    public int abilityPercentage;

    public AbilityPercentage(string rank, int percentage)
    {
        abilityRank = rank;
        abilityPercentage = percentage;
    }
}

[Serializable]
public struct AbilityConsumption
{
    public int abilityLevel;
    public int consumption;

    public AbilityConsumption(int level, int consumption)
    {
        abilityLevel = level;
        this.consumption = consumption;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeManager))]
public class CustomEditorUpgradeManager : Editor
{
    private TextAsset abilityCSVFile1;
    private TextAsset abilityCSVFile2;
    private TextAsset abilityCSVFile3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        
        abilityCSVFile1 = EditorGUILayout.ObjectField("어빌리티 효과", abilityCSVFile1, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load QuestData form CSV"))
        {
            LoadAbilityEffect(abilityCSVFile1);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        abilityCSVFile2 = EditorGUILayout.ObjectField("어빌리티 소모량", abilityCSVFile2, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load QuestData form CSV"))
        {
            LoadAbilityConsumption(abilityCSVFile2);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        abilityCSVFile3 = EditorGUILayout.ObjectField("어빌리티 확률", abilityCSVFile3, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load QuestData form CSV"))
        {
            LoadAbilityProbability(abilityCSVFile3);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    // 어빌리티 효과
    private void LoadAbilityEffect(TextAsset csv)
    {
        string[] lines = csv.text.Split('\n');

        int length = lines.Length - 1;
        (target as UpgradeManager).AbilityEffects = new AbilityEffect[length - 1]; // 어빌리티 CSV 라인 크기 설정
        //UpgradeManager.instance.InitAbilityLength(length);
        
        for (int i = 1; i < length; i++) // 첫 번째 줄 스킵 (분류)
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fileds = line.Split(',');

                EStatusType statusType = EStatusType.ATK;
                bool isSuccess = false;
                string error;

                string title = fileds[0].Trim();
                string ability = fileds[1].Trim();
                foreach (EStatusType type in Enum.GetValues(typeof(EStatusType)))
                {
                    if (type.ToString() == ability)
                    {
                        statusType = type;
                        break;
                    }
                }
                string rank = fileds[2].Trim();

                error = fileds[3].Trim();
                isSuccess = int.TryParse(error, out int min);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 3 : {min}");
                    continue;
                }
                
                error = fileds[4].Trim();
                isSuccess = int.TryParse(error, out int max);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 3 : {min}");
                    continue;
                }


                string currency = fileds[5].Trim();

                AbilityEffect abilityInfo = new AbilityEffect
                (title, statusType, rank, min, max
                    , currency);

                //UpgradeManager.instance.abilityUpgradeInfo[i - 1] = abilityInfo;
                (target as UpgradeManager).AbilityEffects[i - 1] = abilityInfo;
                // Custom Editor 에 정해진 규칙
                // Editor를 상속받는데 Editor에 미리 정해진 target, serializeObject 등의 필드가 있다.
                // [CustomEditor(typeof(UpgradeManager))] 처럼 typeof 안에 있는 클래스가 target, serializeObject에 할당이 된다.
                // target -> object , target as UpgradeManager 해서 캐스팅해서 사용해야 한다. 코드 위 주석처럼 사용하면 널레퍼런스 
            }
            
            EditorUtility.SetDirty(target);
        }
    }
    
    // 어빌리티 소모량
    private void LoadAbilityConsumption(TextAsset csv)
    {
        string[] lines = csv.text.Split('\n');

        int length = lines.Length - 1;
        (target as UpgradeManager).abilityConsumptionInfo = new AbilityConsumption[length - 1]; // 어빌리티 CSV 라인 크기 설정
        
        for (int i = 1; i < length; i++) // 첫 번째 줄 스킵 (분류)
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fileds = line.Split(',');

                bool isSuccess = false;
                string error;
                
                error = fileds[0].Trim();
                isSuccess = int.TryParse(error, out int level);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 0 : {level}");
                    continue;
                }
                
                error = fileds[1].Trim();
                isSuccess = int.TryParse(error, out int consumption);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 1 : {consumption}");
                    continue;
                }

                AbilityConsumption abilityInfo = new AbilityConsumption
                (level, consumption);
                
                (target as UpgradeManager).abilityConsumptionInfo[i - 1] = abilityInfo;
            }
            
            EditorUtility.SetDirty(target);
        }
    }
    
    // 어빌리티 확률
    private void LoadAbilityProbability(TextAsset csv)
    {
        string[] lines = csv.text.Split('\n');

        int length = lines.Length - 1;
        (target as UpgradeManager).abilityPercentageInfo = new AbilityPercentage[length - 1]; // 어빌리티 CSV 라인 크기 설정
        
        for (int i = 1; i < length; i++) // 첫 번째 줄 스킵 (분류)
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fileds = line.Split(',');

                bool isSuccess = false;
                string error;
                
                string rank = fileds[0].Trim();
                
                error = fileds[1].Trim();
                isSuccess = int.TryParse(error, out int percentage);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 1 : {percentage}");
                    continue;
                }

                AbilityPercentage abilityInfo = new AbilityPercentage
                    (rank, percentage);
                
                (target as UpgradeManager).abilityPercentageInfo[i - 1] = abilityInfo;
            }
            
            EditorUtility.SetDirty(target);
        }
    }
}
#endif