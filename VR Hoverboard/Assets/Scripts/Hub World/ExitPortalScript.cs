using System.Collections;
using UnityEngine;
public class ExitPortalScript : MonoBehaviour
{
    private Renderer theFadeObj = null;
    private static readonly System.Type boxCollider = typeof(CapsuleCollider);
    private PlayerMenuController pmc = null;
    private const float fadeTime = 0.8f;
    private void Start()
    {
        pmc = GameManager.player.GetComponent<PlayerMenuController>();
        theFadeObj = GameManager.player.GetComponentInChildren<counterRotater>().GetComponent<Renderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (boxCollider == other.GetType() && "Board" == other.gameObject.tag)
        {
            pmc.ToggleMenuMovement(true);
            StartCoroutine(ExitGameCoroutine());
        }
    }
    private IEnumerator ExitGameCoroutine()
    {
        float timeIntoFade = 0.0f;
        while (timeIntoFade < fadeTime)
        {
            timeIntoFade += Time.deltaTime;
            theFadeObj.material.SetFloat("_AlphaValue", Mathf.Clamp01(timeIntoFade / fadeTime));
            yield return null;
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}