using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/AbilityUpgradeFixedInfo", fileName = "AbilityUpgradeFixedInfo")]
public class AbilityUpgradeFixedInfo : ScriptableObject
{
    public string title;
    public int maxLevel;

    public EStatusType statusType;
    
    [Header("ATK, HP, MP, MP_RECO, CRIT_DMG")]
    public int upgradePerLevelInt;

    [Header("DMG_REDU, CRIT_CH, ATK_SPD, ATK_RAN, MOV_SPD")]
    public float upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType;
    public int baseCost;
    public int increaseCostPerLevel;

    // 꾸미기 관련
    public Sprite image;
}
