using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRayCaster : MonoBehaviour
{
    //turns on or off the ability to do selecting
    public bool canSelect = true;

    //object for selection purposes;
    private GameObject preObj = null;
    private GameObject curObj = null;

    public Camera myCam;
    private reticle reticleScript = null;
    private RaycastHit hit;

    [SerializeField]
    private float rayCheckLength = 50.0f;

    [SerializeField]
    private LayerMask layerMask;

    private void Start ()
    {
        reticleScript = GetComponent<reticle>();
	}
	
	private void Update ()
    {
        preObj = curObj;
        if (Physics.Raycast(myCam.transform.position, myCam.transform.TransformDirection(Vector3.forward), out hit, rayCheckLength, layerMask))
        {
            if (canSelect)
            {
                curObj = hit.collider.gameObject;
                curObj.GetComponent<SelectedObject>().Selected(reticleScript);
            }
        }
        else
            curObj = null;
        if (preObj != null && preObj != curObj)
            preObj.GetComponent<SelectedObject>().Deselected();
    }

    private void SetSelectionLock(bool locked)
    {
        canSelect = !locked;
        if (curObj != null)
            curObj.GetComponent<SelectedObject>().Deselected();
    }

    public void OnEnable()
    {
        EventManager.OnSelectionLock += SetSelectionLock;
    }

    public void OnDisable()
    {
        EventManager.OnSelectionLock -= SetSelectionLock;
    }
}