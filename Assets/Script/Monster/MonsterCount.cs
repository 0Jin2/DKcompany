using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterCount : MonoBehaviour
{
    public TextMeshProUGUI monsterCounterText;
    public List<GameObject> monsterPrefabs;
    public int lifeMonsterCount;

    private int monsterCount;

    void Start()
    {
        monsterCount = lifeMonsterCount;
        UpdateMonsterCounter();
    }

    public void MonsterDestroyed(GameObject destroyedMonster)
    {
        if (monsterPrefabs.Contains(destroyedMonster))
        {
            monsterCount--;
            UpdateMonsterCounter();
        }
    }

    void UpdateMonsterCounter()
    {
        monsterCounterText.text = "Monsters Remaining: " + monsterCount;
    }
}