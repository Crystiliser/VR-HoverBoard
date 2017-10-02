using TMPro;

public class TogglePredictivePathButton : SelectedObject
{
    TextMeshPro onOffText;
    bool IsOn { get { return trailStripCreator.inst.PredictivePathEnabled; } set { trailStripCreator.inst.PredictivePathEnabled = value; } }

    public void isOnUpdate()
    {
        if (IsOn)
        {
            onOffText.SetText("On");
        }
        else
        {
            onOffText.SetText("Off");
        }
    }

    private void OnEnable()
    {
        onOffText = gameObject.GetComponentsInChildren<TextMeshPro>()[0];
        EventManager.OnUpdateButtons += isOnUpdate;
        isOnUpdate();
    }
    public override void selectSuccessFunction()
    {
        IsOn = !IsOn;
        if (IsOn)
        {
            onOffText.SetText("On");
        }
        else
        {
            onOffText.SetText("Off");
        }
    }

    private void OnDisable()
    {
        EventManager.OnUpdateButtons -= isOnUpdate;
    }
}
