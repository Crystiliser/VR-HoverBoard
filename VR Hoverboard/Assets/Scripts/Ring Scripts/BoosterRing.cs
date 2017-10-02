using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterRing : MonoBehaviour
{
    [SerializeField] MeshRenderer arrowRenderer;
    [SerializeField] Transform directionTransform;
    [SerializeField] float boostAmount = 3f;
    [SerializeField] float boostLength = 0.25f;

    float timeIntoBoost = 0f;

    Rigidbody rb;

    private void Start()
    {
        if (arrowRenderer.enabled == true)
            arrowRenderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb = other.GetComponent<Rigidbody>();
            StartCoroutine(BoostCoroutine());
        }      
    }

    IEnumerator BoostCoroutine()
    {
        while (timeIntoBoost < boostLength)
        {
            yield return new WaitForFixedUpdate();

            timeIntoBoost += Time.deltaTime;
            rb.AddForce(directionTransform.forward * boostAmount, ForceMode.Impulse);
        }

        timeIntoBoost = 0f;
        rb = null;
    }
}
