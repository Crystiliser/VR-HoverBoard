using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startupSplashScreenPlayer : MonoBehaviour
{
    public GameObject selector;
    public GameObject[] splashScreens;
    public float[] timesToPlayScreens;

    private float timePlayingCurrent = 0.0f;
    private int currentScreen = 0;
    private bool active = true;


    void Start()
    {
        for (int i = 1; i < splashScreens.Length; i++)
        {
            splashScreens[i].SetActive(false);
        }
        selector.SetActive(false);
    }


    private void FixedUpdate()
    {
        if (active)
        {
            timePlayingCurrent += Time.deltaTime;

            if (timePlayingCurrent >= timesToPlayScreens[currentScreen])
            {
                splashScreens[currentScreen].SetActive(false);
                currentScreen++;
                if (currentScreen < splashScreens.Length)
                {
                    splashScreens[currentScreen].SetActive(true);
                }
                else
                {
                    selector.SetActive(true);
                    active = false;
                }
                timePlayingCurrent = 0;
            }
        }
    }
}
