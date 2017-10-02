using UnityEngine;

public class EnterMainMenuButton : SelectedObject
{
    [SerializeField]
    private GameObject menuBox = null;
    [SerializeField]
    private Transform lockTransform = null;
    [SerializeField]
    private MainMenu menuSystem = null;
    private void Start()
    {
        if (null == menuSystem)
            menuSystem = GetComponentInParent<MainMenu>();
        UnityEditor.Selection.activeGameObject = GetComponentsInChildren<Transform>()[4].gameObject;
    }
    [SerializeField, Tooltip("degrees per second")]
    private float rotationSpeed = 9001.0f;
    private void Update()
    {
        gameObject.transform.Rotate(0.0f, Time.deltaTime * rotationSpeed, 0.0f);
    }

    public override void selectSuccessFunction()
    {
        if (null != menuBox && null != menuSystem)
        {
            menuBox.SetActive(true);
            if (null != lockTransform)
                GameManager.player.GetComponent<PlayerMenuController>().LockPlayerToPosition(lockTransform.position, lockTransform.rotation);
            else
                GameManager.player.GetComponent<PlayerMenuController>().LockPlayerToPosition(GameManager.player.transform.position);
            menuSystem.mainTab.EnableButtons();
            gameObject.SetActive(false);
        }
    }
}