using UnityEngine;
using TMPro;
public class SpeedModifierUpdater : MonoBehaviour
{
#if DEBUGGER
    private PlayerGameplayController pgc = null;
    private TextMeshProUGUI element = null;
#endif
    private void Start()
    {
#if DEBUGGER
        pgc = GetComponentInParent<PlayerGameplayController>();
        element = GetComponent<TextMeshProUGUI>();
        element.color = Color.magenta;
#else
        Destroy(GetComponent<TextMeshProUGUI>());
        Destroy(this);
#endif
    }
#if DEBUGGER
    private void Update()
    {
        int display = (int)pgc.DebugSpeedIncrease;
        if (0 != display)
        {
            if (!element.IsActive())
                element.enabled = true;
            element.SetText(display.ToString());
        }
        else
        {
            if (element.IsActive())
            {
                element.SetText("");
                element.enabled = false;
            }
        }
    }
#endif
}