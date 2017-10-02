using UnityEngine;

public class TooltipTextScript : MonoBehaviour
{
    private delegate void UpdateTooltipEvent(string str);
    private static UpdateTooltipEvent OnUpdateTooltip;
    TMPro.TextMeshProUGUI textMesh = null;
    public static void SetText(string str = null)
    {
        if (null != OnUpdateTooltip)
            OnUpdateTooltip(str);
    }
    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
        textMesh.SetText("");
        textMesh.enabled = false;
    }
    private void OnEnable()
    {
        OnUpdateTooltip += UpdateTooltip;
    }
    private void OnDisable()
    {
        OnUpdateTooltip -= UpdateTooltip;
    }
    void UpdateTooltip(string str)
    {
        if (null != textMesh)
        {
            if (null == str || "" == str)
            {
                textMesh.SetText("");
                textMesh.enabled = false;
                bugfixwait = true;
            }
            else if (!bugfixwait)
            {
                textMesh.enabled = true;
                textMesh.SetText(str);
                bugfixwait = true;
            }
        }
    }
    float bugfixtimer = 0.0f;
    const float bugfixtime = 0.1f;
    bool bugfixwait { get { return bugfixtimer > 0.0f; } set { bugfixtimer = value ? bugfixtime : 0.0f; } }
    private void Update()
    {
        if (bugfixwait)
            bugfixtimer -= Time.deltaTime;
    }
}