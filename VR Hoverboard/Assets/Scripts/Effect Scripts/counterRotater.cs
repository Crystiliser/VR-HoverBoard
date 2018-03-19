using UnityEngine;
public class counterRotater : MonoBehaviour
{
    private Quaternion rotation;
    private void Start()
    {
        rotation = transform.rotation;
    }
    private void Update()
    {
        transform.rotation = rotation;
    }
}