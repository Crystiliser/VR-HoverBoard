using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class bonusTimeTextUpdater : MonoBehaviour
{
    public Color myColor;

    TextMeshProUGUI element;
    Animator myAnimator;

    public void play(string bonusTime)
    {
        element.SetText(bonusTime);
        myAnimator.SetBool("AddTime", true);
    }

    private void Start()
    {
        element = gameObject.GetComponent<TextMeshProUGUI>();
        myAnimator = gameObject.GetComponent<Animator>();
        element.overrideColorTags = true;
        element.color = myColor;
    }

    public void animationEnded()
    {
        myAnimator.SetBool("AddTime", false);
    }

    private void Update()
    {
        element.color = myColor;
    }
}
