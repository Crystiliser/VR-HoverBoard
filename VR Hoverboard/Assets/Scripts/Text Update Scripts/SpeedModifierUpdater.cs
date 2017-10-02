using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpeedModifierUpdater : MonoBehaviour
{
    PlayerGameplayController pgc;
    TextMeshProUGUI element;

    int display;

    void Start()
    {
        pgc = GetComponentInParent<PlayerGameplayController>();
        element = GetComponent<TextMeshProUGUI>();
        element.color = new Color(1, 0, 1);
    }

    void Update()
    {
        display = (int)pgc.DebugSpeedIncrease;

        if (display != 0)
        {
            if (!element.IsActive())
                element.enabled = true;

            element.SetText(display.ToString());
        }
        else
        {
            if (element.IsActive())
            {
                element.text = "";
                element.enabled = false;
            }
        }
    }
}
