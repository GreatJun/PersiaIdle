using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEditor;
using UnityEngine;

public class TestDataParse : MonoBehaviour
{
    /*
    public static TestDataParse instance;

    private void Awake()
    {
        instance = this;
    }

    // 어빌리티 랭크 & 확률
    private void LoadAbilityRank(TextAsset csv)
    {
        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄 스킵 (분류)
        {
            string line = lines[i];
            
        }
    }

    // 어빌리티 능력치 종류 & 등급별 상승률
    public static void LoadAbilityProbability(TextAsset csv)
    {
        var abilityProbability = new Dictionary<int, List<string>>();
        
        string[] lines = csv.text.Split('\n');


        int length = lines.Length - 1;
        UpgradeManager.instance.InitAbilityLength(length);
        
        for (int i = 1; i < length; i++) // 첫 번째 줄 스킵 (분류)
        {
            string line = lines[i];
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] fileds = line.Split(',');

                bool isSuccess = false;
                string error;

                string title = fileds[0].Trim();
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].title = title;
                string ability = fileds[1].Trim();
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].abilityStat = ability;
                string rank = fileds[2].Trim();
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].rank = rank;

                error = fileds[3].Trim();
                isSuccess = int.TryParse(error, out int min);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 3 : {min}");
                    continue;
                }
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].min = min;
                
                error = fileds[4].Trim();
                isSuccess = int.TryParse(error, out int max);
                if (!isSuccess)
                {
                    Debug.LogWarning($"Failed 3 : {min}");
                    continue;
                }
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].max = max;


                string currency = fileds[5].Trim();
                UpgradeManager.instance.abilityUpgradeInfo[i - 1].currency = currency;

                abilityProbability.Add
                (
                    i,
                    new List<string>
                    {
                        title, ability, rank, min.ToString(), max.ToString(), currency
                    }
                );
            }
            
            Debug.Log(abilityProbability[i]);
        }
    }
    */
}
