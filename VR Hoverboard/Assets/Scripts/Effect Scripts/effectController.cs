using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum particleEffectTypesEnum { rain, snow, crash, other }

public class effectController : MonoBehaviour
{
    public ParticleSystem[] triggerParticleEffects;

    public ParticleSystem dustField;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += dustFieldActivation;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= dustFieldActivation;
    }


    // Use this for initialization
    void Start()
    {
        disableAllEffects();
    }

    void dustFieldActivation(Scene scene, LoadSceneMode loadMode)
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                dustField.Stop();
                break;
            case 1:
                dustField.Stop();
                break;
            default:
                dustField.Play();
                //dustField.Pause();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            particleEffectTypesEnum theEffect = other.gameObject.GetComponent<effectZoneProperties>().myEffect;
            switch (theEffect)
            {
                case particleEffectTypesEnum.rain:
                    triggerParticleEffects[0].Play();
                    break;
                case particleEffectTypesEnum.snow:
                    triggerParticleEffects[1].Play();
                    break;
                case particleEffectTypesEnum.other:
                    for (int i = 0; i < triggerParticleEffects.Length; i++)
                    {
                        triggerParticleEffects[i].Play();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void disableAllEffects()
    {
        for (int i = 0; i < triggerParticleEffects.Length; i++)
        {
            triggerParticleEffects[i].Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            particleEffectTypesEnum theEffect = other.gameObject.GetComponent<effectZoneProperties>().myEffect;
            switch (theEffect)
            {
                case particleEffectTypesEnum.rain:
                    triggerParticleEffects[0].Stop();
                    break;
                case particleEffectTypesEnum.snow:
                    triggerParticleEffects[1].Stop();
                    break;
                case particleEffectTypesEnum.other:
                    for (int i = 0; i < triggerParticleEffects.Length; i++)
                    {
                        triggerParticleEffects[i].Stop();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
