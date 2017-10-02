using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEffects : MonoBehaviour
{
    Animator anim;

    int pulseHash = Animator.StringToHash("PulseActive");

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartRingPulse()
    {
        anim.SetBool(pulseHash, true);
    }

    public void StopRingPulse()
    {
        anim.SetBool(pulseHash, false);
    }

    private void OnEnable()
    {
        EventManagerRings.StartRingPulse += StartRingPulse;
        EventManagerRings.StopRingPulse += StopRingPulse;
    }

    private void OnDisable()
    {
        EventManagerRings.StartRingPulse -= StartRingPulse;
        EventManagerRings.StopRingPulse -= StopRingPulse;
    }
       
}