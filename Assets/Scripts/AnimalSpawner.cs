using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> animals;

    void Start()
    {
        StartCoroutine(SpawnAnimal());
    }

    IEnumerator SpawnAnimal()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < 2; i++)
            {
                float randomX = Random.Range(-23f, 23f);
                float randomY = Random.Range(-13f, 33f);
                Vector3 spawnPos = new Vector3(randomX, 0, randomY);

                int randomIndex = Random.Range(0, animals.Count);

                Instantiate(animals[randomIndex], spawnPos, Quaternion.identity);
            }
        }
    }
}
