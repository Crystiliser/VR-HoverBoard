using UnityEngine;
using Xander.Debugging;
using Xander.NullConversion;
public class EyeRayCaster : MonoBehaviour
{
    [SerializeField] private bool canSelect = true;
    [SerializeField] private Camera myCam = null;
    public float rayCheckLength = 12.0f;
    [SerializeField] private LayerMask layerMask;
    private GameObject preObj = null, curObj = null;
    private ReticleScript reticleScript = null;
    private RaycastHit hit;
    private void Start() => reticleScript = GetComponent<ReticleScript>();
    private void Update()
    {
        preObj = curObj;
        if (Physics.Raycast(myCam.transform.position, myCam.transform.TransformDirection(Vector3.forward), out hit, rayCheckLength, layerMask))
        {
            if (canSelect)
            {
                SelectedObject selected = hit.collider.GetNullConvertedComponent<SelectedObject>();
                if (null == selected)
                    Debug.LogWarning("Missing SelectedObject script on object in the " + SelectedObject.LAYERNAME + " layer. (" + hit.collider.gameObject.HierarchyPath() + ")" + this.Info(), this);
                curObj = selected?.gameObject;
                selected?.Selected(reticleScript);
            }
        }
        else
            curObj = null;
        if (preObj != curObj)
            preObj.ConvertNull()?.GetComponent<SelectedObject>().Deselected();
    }
    private void SetSelectionLock(bool locked)
    {
        canSelect = !locked;
        curObj.ConvertNull()?.GetComponent<SelectedObject>().Deselected();
    }
    private void OnEnable() => EventManager.OnSelectionLock += SetSelectionLock;
    private void OnDisable() => EventManager.OnSelectionLock -= SetSelectionLock;
}