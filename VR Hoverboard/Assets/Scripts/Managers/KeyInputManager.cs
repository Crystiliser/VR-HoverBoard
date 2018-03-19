using System.Collections;
using UnityEngine;
using static KeyInputManager.VR;
using static Xander.Debugging.Helper;
public class KeyInputManager : MonoBehaviour
{
    public static class VR
    {
        public static bool VRPresent => UnityEngine.XR.XRDevice.isPresent;
        public static Quaternion GetHeadRotation() => UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
    }
#if UNITY_STANDALONE_OSX
    public const KeyCode XBOX_A = KeyCode.JoystickButton16;
    public const KeyCode XBOX_X = KeyCode.JoystickButton18;
    public const KeyCode XBOX_Y = KeyCode.JoystickButton19;
    public const KeyCode XBOX_LB = KeyCode.JoystickButton13;
    public const KeyCode XBOX_RB = KeyCode.JoystickButton14;
    public const KeyCode XBOX_BACK = KeyCode.JoystickButton10;
    public const KeyCode XBOX_START = KeyCode.JoystickButton9;
#else
    public const KeyCode XBOX_A = KeyCode.JoystickButton0;
    public const KeyCode XBOX_X = KeyCode.JoystickButton2;
    public const KeyCode XBOX_Y = KeyCode.JoystickButton3;
    public const KeyCode XBOX_LB = KeyCode.JoystickButton4;
    public const KeyCode XBOX_RB = KeyCode.JoystickButton5;
    public const KeyCode XBOX_BACK = KeyCode.JoystickButton6;
    public const KeyCode XBOX_START = KeyCode.JoystickButton7;
#endif
    [SerializeField] private float flippedTimer = 3.0f;
    [SerializeField] private bool hubOnFlippedHMD = false;
    private static bool countingDown = false;
    private static float timeUpsideDown = 0.0f;
    private static ThirdPersonCamera thirdPersonCameraScript = null;
    private static Transform cameraContainer = null;
    private static Quaternion flippedQuaternion;
    private static Vector3 cameraContainerPositionDifference;
    public static void SetupKeyInputManager()
    {
        thirdPersonCameraScript = GameManager.player.GetComponentInChildren<ThirdPersonCamera>();
        cameraContainer = GameManager.player.GetComponentInChildren<CameraCounterRotate>().transform;
        cameraContainerPositionDifference = cameraContainer.position - GameManager.player.transform.position;
    }
    private void Awake() => StartCoroutine(CalibrationCoroutine());
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameState.HubWorld != GameManager.gameState)
            {
                GameManager.lastPortalBuildIndex = -1;
                EventManager.OnTriggerTransition(1);
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(XBOX_BACK))
            StartCoroutine(CalibrationCoroutine());
        if (GameState.HubWorld != GameManager.gameState && Input.GetKeyDown(XBOX_START))
        {
            GameManager.lastPortalBuildIndex = -1;
            EventManager.OnTriggerTransition(1);
        }
        if (Input.GetKeyDown(XBOX_Y))
            thirdPersonCameraScript.UpdateThirdPersonCamera();
        if (VRPresent && hubOnFlippedHMD && GameState.HubWorld != GameManager.gameState)
        {
            flippedQuaternion = GetHeadRotation();
            if (flippedQuaternion.eulerAngles.z > 150.0f && flippedQuaternion.eulerAngles.z < 210.0f && !countingDown)
            {
                countingDown = true;
                timeUpsideDown = 0.0f;
            }
            else if (countingDown)
            {
                if (flippedQuaternion.eulerAngles.z > 150.0f && flippedQuaternion.eulerAngles.z < 210.0f)
                    timeUpsideDown += Time.deltaTime;
                else
                    countingDown = false;
                if (timeUpsideDown > flippedTimer)
                {
                    countingDown = false;
                    EventManager.OnTriggerTransition(1);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Screenshot Saved to " + Application.persistentDataPath);
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + $"/Cybersurf_{DozenalTimeStamp}.png");
        }
    }
    private static IEnumerator CalibrationCoroutine()
    {
        if (!thirdPersonCameraScript.UpdatingCameraPosition)
        {
            yield return new WaitForEndOfFrame();
            Transform player = GameManager.player.transform;
            Transform screenFade = player.GetComponentInChildren<ScreenFade>().transform;
            cameraContainer.SetPositionAndRotation(player.position, player.rotation);
            cameraContainer.Translate(cameraContainerPositionDifference);
            cameraContainer.Rotate(0.0f, Mathf.DeltaAngle(screenFade.eulerAngles.y, cameraContainer.eulerAngles.y), 0.0f);
            cameraContainer.Translate(-screenFade.localPosition);
            thirdPersonCameraScript.CalibrateThirdPersonAnchors(cameraContainer.position, player.rotation);
        }
    }
}