using UnityEngine;
public class FloatEffect : MonoBehaviour
{
    public enum StartDirection { Up, Down };
    [SerializeField] private Transform objectTransform;
    [Space, SerializeField] private bool randomizeStartHeight = true;
    [SerializeField] private StartDirection startDirection = StartDirection.Up;
    [SerializeField] private float floatDistance = 0.5f, floatRate = 0.1f, rotateRate = 60.0f;
    private float originalHeight = 0.0f, direction = 1.0f;
    private void Start()
    {
        originalHeight = objectTransform.position.y;
        if (randomizeStartHeight)
            objectTransform.Translate(0.0f, Random.Range(-floatDistance, floatDistance), 0.0f, Space.World);
        if (startDirection != StartDirection.Up)
            direction = -1.0f;
    }
    private void Update()
    {
        if (direction > 0.0f)
        {
            if (!(objectTransform.position.y < originalHeight + floatDistance))
                direction = -direction;
        }
        else if (!(objectTransform.position.y > originalHeight - floatDistance))
            direction = -direction;
        objectTransform.Translate(0.0f, Time.deltaTime * floatRate * direction, 0.0f, Space.World);
        objectTransform.Rotate(Vector3.up, rotateRate * Time.deltaTime, Space.World);
    }
}