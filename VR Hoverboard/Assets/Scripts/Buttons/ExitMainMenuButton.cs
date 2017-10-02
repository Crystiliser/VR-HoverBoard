using UnityEngine;

public class ExitMainMenuButton : SelectedObject
{
    [SerializeField]
    EnterMainMenuButton enterMenuObject = null;
    new private void Start()
    {
        base.Start();
        if (null == enterMenuObject)
            enterMenuObject = FindObjectOfType<EnterMainMenuButton>();
    }

    public override void selectSuccessFunction()
    {
        if (null != GetComponentInParent<MainMenu>().OnMenuExit)
            GetComponentInParent<MainMenu>().OnMenuExit();
        PlayerPrefs.Save();
        enterMenuObject.gameObject.SetActive(true);
    }
}