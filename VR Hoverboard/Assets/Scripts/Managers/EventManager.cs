public static class EventManager
{
    public delegate void IntParamEvent(int value);
    public delegate void BoolParamEvent(bool value);
    public delegate void VoidParamEvent();
    public static event IntParamEvent OnTransition;
    public static event BoolParamEvent OnToggleMovement, OnSelectionLock, OnToggleHud, OnSetRingPath;
    public static event VoidParamEvent OnUpdateBoardMenuEffects, OnStartRingPulse, OnStopRingPulse;
    public static void OnSetGameplayMovementLock(bool locked) => OnToggleMovement?.Invoke(locked);
    public static void OnTriggerTransition(int sceneIndex) => OnTransition?.Invoke(sceneIndex);
    public static void OnTriggerSelectionLock(bool locked) => OnSelectionLock?.Invoke(locked);
    public static void OnSetHudOnOff(bool isOn) => OnToggleHud?.Invoke(isOn);
    public static void OnCallSetRingPath(bool isOn) => OnSetRingPath?.Invoke(isOn);
    public static void OnCallBoardMenuEffects() => OnUpdateBoardMenuEffects?.Invoke();
    public static void StartRingPulse() => OnStartRingPulse?.Invoke();
    public static void StopRingPulse() => OnStopRingPulse?.Invoke();
}