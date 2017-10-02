using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBoardSinkEffect : MonoBehaviour
{
    enum TransitionStates { Sinking, Floating };

    Transform translationTransform;

    [SerializeField] float transitionTime = 1f;
    [SerializeField] float sinkRate = 0.1f;
    [SerializeField] float sinkDistance = 0.2f;

    void Start ()
    {
		translationTransform = GetComponent<Transform>();
    }

    public IEnumerator SinkEffectCoroutine(Transform backBoardTransform)
    {    
        float transitionTimer = 0f, halfTransitionTime = transitionTime / 2f;

        Vector3 originalPosition = backBoardTransform.position, currPosition = backBoardTransform.position;

        translationTransform.position = backBoardTransform.position;
        translationTransform.rotation = backBoardTransform.rotation;
        translationTransform.localScale = backBoardTransform.localScale;

        translationTransform.Translate(Vector3.forward * sinkDistance);
        Vector3 sinkToPosition = translationTransform.position;

        TransitionStates currentTransition = TransitionStates.Sinking;

        while (true)
        {
            switch (currentTransition)
            {
                case TransitionStates.Sinking:
                    currPosition = Vector3.Lerp(currPosition, sinkToPosition, sinkRate);
                    break;
                case TransitionStates.Floating:
                    currPosition = Vector3.Lerp(currPosition, originalPosition, sinkRate);
                    break;
            }

            backBoardTransform.position = currPosition;

            transitionTimer += Time.deltaTime;

            if (currentTransition == TransitionStates.Sinking && transitionTimer >= halfTransitionTime)
            {
                currentTransition = TransitionStates.Floating;
                OnStartContentUpdate();
            }

            else if (currentTransition == TransitionStates.Floating && transitionTimer >= transitionTime)
            {
                backBoardTransform.position = originalPosition;
                break;
            }

            yield return null;
        }
    }

    public delegate void ContentUpdate();
    public static event ContentUpdate StartContentUpdate;

    static public void OnStartContentUpdate()
    {
        if (StartContentUpdate != null)
            StartContentUpdate();
    }

}
