using System.Collections;
using UnityEngine;
public class BackBoardSinkEffect : MonoBehaviour
{
    public delegate void ContentUpdate();
    public static event ContentUpdate StartContentUpdate;
    [SerializeField] private float transitionTime = 1.0f, sinkRate = 6.0f, sinkDistance = 0.2f;
    public IEnumerator SinkEffectCoroutine(Transform backBoardTransform)
    {
        float transitionTimer = 0.0f;
        Vector3 originalPosition = backBoardTransform.position, currPosition = backBoardTransform.position;
        transform.position = backBoardTransform.position;
        transform.rotation = backBoardTransform.rotation;
        transform.localScale = backBoardTransform.localScale;
        transform.Translate(0.0f, 0.0f, sinkDistance);
        Vector3 sinkToPosition = transform.position;
        bool isSinking = true;
        while (true)
        {
            Vector3.Lerp(currPosition, isSinking ? sinkToPosition : originalPosition, sinkRate * Time.deltaTime);
            backBoardTransform.position = currPosition;
            transitionTimer += Time.deltaTime;
            if (isSinking && transitionTimer >= transitionTime * 0.5f)
            {
                isSinking = false;
                StartContentUpdate?.Invoke();
            }
            else if (!isSinking && transitionTimer >= transitionTime)
            {
                backBoardTransform.position = originalPosition;
                break;
            }
            yield return null;
        }
    }
}