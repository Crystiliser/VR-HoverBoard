using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerRings : MonoBehaviour
{
    public delegate void StartPulse();
    public static event StartPulse StartRingPulse;

    static public void OnStartRingPulse()
    {
        if (StartRingPulse != null)
            StartRingPulse();
    }

    public delegate void StopPulse();
    public static event StopPulse StopRingPulse;

    static public void OnStopRingPulse()
    {
        if (StopRingPulse != null)
            StopRingPulse();
    }
}
