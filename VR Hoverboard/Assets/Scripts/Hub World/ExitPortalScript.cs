using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitPortalScript : MonoBehaviour
{
    Image theFadeObj;

    System.Type boxCollider;
    PlayerMenuController pmc;

    private void Start()
    {
        boxCollider = typeof(UnityEngine.CapsuleCollider);
        pmc = GameManager.player.GetComponent<PlayerMenuController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == boxCollider && other.gameObject.tag == "Board")
        {
            pmc.ToggleMenuMovement(true);

            theFadeObj = GameObject.FindGameObjectWithTag("FadeCover").GetComponent<Image>();
            StartCoroutine(ExitGameCoroutine());
        }
    }

    IEnumerator ExitGameCoroutine()
    {
        float timeIntoFade = 0f;
        float fadeTime = 0.8f;
        float alpha = theFadeObj.color.a;

        while (timeIntoFade < fadeTime)
        {
            timeIntoFade += Time.deltaTime;

            alpha = timeIntoFade / fadeTime;
            alpha = Mathf.Clamp01(alpha);

            theFadeObj.material.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        SaveLoader.save();
        Application.Quit();

        //in case we're in the editor
        yield return new WaitForSeconds(1f);
        pmc.ToggleMenuMovement(false);
        theFadeObj.material.color = new Color(0f, 0f, 0f, 0f);
    }
}
