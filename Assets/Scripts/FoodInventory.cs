using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInventory : MonoBehaviour
{
    public List<GameObject> inventory;

    public int index = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            inventory[index].SetActive(false);

            index++;
            if (index >= 3)
                index = 0;

            inventory[index].SetActive(true);
        }
    }
}
