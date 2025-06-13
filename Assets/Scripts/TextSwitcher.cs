using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSwitcher : MonoBehaviour
{
    TextMeshProUGUI text;

    string defaultText = "Path Finding Algorithm:\n";

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            text.text = defaultText + "BFS";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            text.text = defaultText + "Greedy";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            text.text = defaultText + "A*";
        }
    }
}
