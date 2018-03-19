using UnityEngine;
public class HubPortalRotator : MonoBehaviour
{
    [SerializeField] private float rotateRate = 30.0f, distanceFromCenter = 28.0f;
    private float rotateAmount = 0.0f;
    private void Start()
    {
        rotateAmount = 360.0f / transform.childCount;
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).Rotate(Vector3.up * i * rotateAmount);
            transform.GetChild(i).Translate(Vector3.back * distanceFromCenter, Space.Self);
            transform.GetChild(i).LookAt(transform);
        }
    }
    private void Update()
    {
        transform.Rotate(Vector3.up, rotateRate * Time.deltaTime);
    }
}