using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMobSpawner : MonoBehaviour
{
    [Header("몬스터 프리팹 리스트")]
    public GameObject[] objectPrefabs;

    [Header("몬스터 생성 최소 딜레이")]
    public float spawnTimeMin = 1f;

    [Header("몬스터 생성 최대 딜레이")]
    public float spawnTimeMax = 5f;

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    public IEnumerator SpawnObjects()
    {
        while (true)
        {
            float waitTime = Random.Range(spawnTimeMin, spawnTimeMax);
            yield return new WaitForSeconds(waitTime);

            if (objectPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, objectPrefabs.Length);
                GameObject spawnedObject = Instantiate(objectPrefabs[randomIndex], transform.position, Quaternion.identity);

                if (!spawnedObject.activeInHierarchy)
                {
                    spawnedObject.SetActive(true);
                }
            }
        }
    }
}