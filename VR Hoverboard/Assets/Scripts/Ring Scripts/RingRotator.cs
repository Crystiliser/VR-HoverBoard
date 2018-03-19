using UnityEngine;
public class RingRotator : MonoBehaviour
{
    [SerializeField] private float rotateRate = 5.0f;
    private void Update()
    { transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotateRate); }
}