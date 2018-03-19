using UnityEngine;
public class RingEffects : MonoBehaviour
{
    private Animator anim = null;
    private static readonly int pulseHash = Animator.StringToHash("PulseActive");
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void StartRingPulse()
    {
        anim.SetBool(pulseHash, true);
    }
    private void StopRingPulse()
    {
        anim.SetBool(pulseHash, false);
    }
    private void OnEnable()
    {
        EventManager.OnStartRingPulse += StartRingPulse;
        EventManager.OnStopRingPulse += StopRingPulse;
    }
    private void OnDisable()
    {
        EventManager.OnStartRingPulse -= StartRingPulse;
        EventManager.OnStopRingPulse -= StopRingPulse;
    }
}