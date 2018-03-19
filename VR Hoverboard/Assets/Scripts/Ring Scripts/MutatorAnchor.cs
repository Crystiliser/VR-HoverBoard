using UnityEngine;
public class MutatorAnchor : MonoBehaviour
{
    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(this);
    }
}