using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crashEffectPlayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        MeshRenderer theMesh = hitObject.GetComponent<MeshRenderer>();
        if (theMesh)
        {
            Material texture = theMesh.material;
            effectController effects = gameObject.GetComponentInChildren<effectController>();
            ParticleSystem particleEffect = effects.triggerParticleEffects[(int)particleEffectTypesEnum.crash];
            particleEffect.GetComponent<Renderer>().material = texture;
            particleEffect.GetComponent<Renderer>().materials[1] = texture;
            particleEffect.Play();
        }
    }
}
