using UnityEngine;
//using UnityEditor;

public class EnterMainMenuButton : SelectedObject
{
    [SerializeField]
    private GameObject menuBox = null;
    [SerializeField]
    private Transform lockTransform = null;
    [SerializeField]
    private MainMenu menuSystem = null;
    [SerializeField]
    private Material hoverMat = null;
    [SerializeField]
    private Material noHoverMat = null;
    private MeshRenderer meshRenderer = null;
    new private void Start()
    {
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
        if (null == menuSystem)
            menuSystem = GetComponentInParent<MainMenu>();
        if (null == noHoverMat)
            noHoverMat = meshRenderer.material;
        if (null == hoverMat)
            hoverMat = noHoverMat;
        meshRenderer.material = noHoverMat;
    }
    [SerializeField, Tooltip("degrees per second")]
    private float rotationSpeed = 111.0f;
    new private void Update()
    {
        base.Update();
        gameObject.transform.Rotate(0.0f, Time.deltaTime * rotationSpeed, 0.0f);
    }

    protected override void SelectedFunction()
    {
        base.SelectedFunction();
        meshRenderer.material = hoverMat;
    }
    protected override void DeselectedFunction()
    {
        base.DeselectedFunction();
        meshRenderer.material = noHoverMat;
    }
    public override void selectSuccessFunction()
    {
        if (null != menuBox && null != menuSystem)
        {
            menuBox.SetActive(true);
            if (null != lockTransform)
                GameManager.player.GetComponent<PlayerMenuController>().LockPlayerToPosition(lockTransform.position, lockTransform.rotation);
            else
                GameManager.player.GetComponent<PlayerMenuController>().LockPlayerToPosition(GameManager.player.transform.position, GameManager.player.transform.rotation);
            menuSystem.mainTab.EnableButtons();
            gameObject.SetActive(false);
        }
    }
}