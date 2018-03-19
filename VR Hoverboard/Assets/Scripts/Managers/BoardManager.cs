using System.Collections;
using UnityEngine;
public enum BoardType { Original, MachI, MachII, MachIII }
public class BoardManager : MonoBehaviour
{
    public static SpatialData gyro = null;
#if DEBUGGER
    public static bool debugSpeedEnabled = false;
#endif
    public static bool gamepadEnabled = false;
    public static BoardType currentBoardSelection = BoardType.Original;
    private static BoardSelector boardSelector = null;
    private static PlayerGameplayController pgc = null;
    private static PlayerMenuController pmc = null;
    private static PlayerFanController pfc = null;
    public void SetupBoardManager()
    {
        StartCoroutine(DetectGyroCoroutine());
        pgc = GameManager.player.GetComponent<PlayerGameplayController>();
        pmc = GameManager.player.GetComponent<PlayerMenuController>();
        pfc = GameManager.player.GetComponent<PlayerFanController>();
        boardSelector = GameManager.player.GetComponentInChildren<BoardSelector>(true);
        pgc.SetupGameplayControllerScript();
        pmc.SetupMenuControllerScript();
        pfc.SetupFanControllerScript();
        BoardSelect(currentBoardSelection);
#if DEBUGGER
        if (debugSpeedEnabled)
            pgc.StartDebugSpeedControls();
#endif
    }
    private static IEnumerator DetectGyroCoroutine()
    {
        gyro = new SpatialData();
        yield return new WaitForSeconds(SpatialData.WaitForAttach);
        gamepadEnabled = !(gyro.device?.Attached ?? false);
        if (gamepadEnabled)
        {
            gyro.Close();
            gyro = null;
        }
        UpdateControlsType(gamepadEnabled);
    }
    public static void UpdateControlsType(bool gPadEnabled)
    {
        gamepadEnabled = gPadEnabled;
        gyro?.Close();
        if (gamepadEnabled)
            gyro = null;
        else
            gyro = new SpatialData();
        pgc.UpdateGameplayControlsType(gPadEnabled, gyro);
        pmc.UpdateMenuControlsType(gPadEnabled, gyro);
    }
    public static void BoardSelect(BoardType bSelect)
    {
        currentBoardSelection = bSelect;
        boardSelector.SelectBoard(bSelect);
        pgc.UpdatePlayerBoard(gamepadEnabled ? GamepadBoardSelect(currentBoardSelection) : GyroBoardSelect());
        pfc.UpdateFanPercentage();
    }
    public static PlayerMovementVariables GamepadBoardSelect(BoardType boardType)
    {
        PlayerMovementVariables pmv = new PlayerMovementVariables();
        switch (boardType)
        {
            case BoardType.Original:
                pmv.downwardAcceleration = 30.0f;
                pmv.restingAcceleration = 17.0f;
                pmv.upwardAcceleration = 15.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 25.0f;
                pmv.restingSpeed = 15.0f;
                pmv.minSpeed = 12.0f;
                pmv.pitchSensitivity = 3.45f;
                pmv.yawSensitivity = 3.45f;
                pmv.maxDescendAngle = 30.0f;
                pmv.restingThreshold = 15.0f;
                pmv.maxAscendAngle = 30.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachI:
                pmv.downwardAcceleration = 45.0f;
                pmv.restingAcceleration = 25.0f;
                pmv.upwardAcceleration = 22.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 35.0f;
                pmv.restingSpeed = 23.0f;
                pmv.minSpeed = 20.0f;
                pmv.pitchSensitivity = 3.2f;
                pmv.yawSensitivity = 3.2f;
                pmv.maxDescendAngle = 30.0f;
                pmv.restingThreshold = 18.0f;
                pmv.maxAscendAngle = 30.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachII:
                pmv.downwardAcceleration = 55.0f;
                pmv.restingAcceleration = 30.0f;
                pmv.upwardAcceleration = 25.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 40.0f;
                pmv.restingSpeed = 25.0f;
                pmv.minSpeed = 21.0f;
                pmv.pitchSensitivity = 3.15f;
                pmv.yawSensitivity = 3.15f;
                pmv.maxDescendAngle = 32.0f;
                pmv.restingThreshold = 15.0f;
                pmv.maxAscendAngle = 32.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachIII:
                pmv.downwardAcceleration = 70.0f;
                pmv.restingAcceleration = 42.0f;
                pmv.upwardAcceleration = 37.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 50.0f;
                pmv.restingSpeed = 35.0f;
                pmv.minSpeed = 31.0f;
                pmv.pitchSensitivity = 3.0f;
                pmv.yawSensitivity = 3.0f;
                pmv.maxDescendAngle = 35.0f;
                pmv.restingThreshold = 10.0f;
                pmv.maxAscendAngle = 35.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            default:
                break;
        }
        return pmv;
    }
    private static PlayerMovementVariables GyroBoardSelect()
    {
        PlayerMovementVariables pmv = new PlayerMovementVariables();
        switch (currentBoardSelection)
        {
            case BoardType.Original:
                pmv.downwardAcceleration = 30.0f;
                pmv.restingAcceleration = 17.0f;
                pmv.upwardAcceleration = 15.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 25.0f;
                pmv.restingSpeed = 15.0f;
                pmv.minSpeed = 12.0f;
                pmv.pitchSensitivity = 2.5f;
                pmv.yawSensitivity = 2.75f;
                pmv.maxDescendAngle = 30.0f;
                pmv.restingThreshold = 15.0f;
                pmv.maxAscendAngle = 30.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachI:
                pmv.downwardAcceleration = 45.0f;
                pmv.restingAcceleration = 25.0f;
                pmv.upwardAcceleration = 22.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 35.0f;
                pmv.restingSpeed = 23.0f;
                pmv.minSpeed = 20.0f;
                pmv.pitchSensitivity = 2.5f;
                pmv.yawSensitivity = 2.75f;
                pmv.maxDescendAngle = 30.0f;
                pmv.restingThreshold = 18.0f;
                pmv.maxAscendAngle = 30.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachII:
                pmv.downwardAcceleration = 55.0f;
                pmv.restingAcceleration = 30.0f;
                pmv.upwardAcceleration = 25.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 40.0f;
                pmv.restingSpeed = 25.0f;
                pmv.minSpeed = 21.0f;
                pmv.pitchSensitivity = 2.5f;
                pmv.yawSensitivity = 2.75f;
                pmv.maxDescendAngle = 32.0f;
                pmv.restingThreshold = 15.0f;
                pmv.maxAscendAngle = 32.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            case BoardType.MachIII:
                pmv.downwardAcceleration = 70.0f;
                pmv.restingAcceleration = 42.0f;
                pmv.upwardAcceleration = 37.0f;
                pmv.momentum = 0.1f;
                pmv.maxSpeed = 50.0f;
                pmv.restingSpeed = 35.0f;
                pmv.minSpeed = 31.0f;
                pmv.pitchSensitivity = 2.5f;
                pmv.yawSensitivity = 2.75f;
                pmv.maxDescendAngle = 35.0f;
                pmv.restingThreshold = 10.0f;
                pmv.maxAscendAngle = 35.0f;
                pmv.bounceModifier = 1.0f;
                pmv.mass = 1.0f;
                pmv.drag = 1.0f;
                pmv.angularDrag = 5.0f;
                break;
            default:
                break;
        }
        return pmv;
    }
    private void OnDisable() => gyro?.Close();
}