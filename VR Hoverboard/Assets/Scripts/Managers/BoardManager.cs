using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardType { Original, MachI, MachII, MachIII, Custom }

public class BoardManager : MonoBehaviour
{
    [HideInInspector] public SpatialData gyro;

    PlayerGameplayController pgc;
    PlayerMenuController pmc;
    PlayerFanController pfc;
    MeshRenderer boardMR;

    public bool debugSpeedEnabled = false;
    [HideInInspector] public bool gamepadEnabled = false;

    //store our current board selection
    public BoardType currentBoardSelection = BoardType.MachI;
    [SerializeField] Material[] boardMaterials;

    [Space]
    public ManagerClasses.PlayerMovementVariables customGamepadMovementVariables = new ManagerClasses.PlayerMovementVariables();
    public ManagerClasses.PlayerMovementVariables customGyroMovementVariables = new ManagerClasses.PlayerMovementVariables();

    //use this instead of Awake() so that we can control the execution order through the GameManager
    public void SetupBoardManager(GameObject p)
    {
        //set our controls type based off of if the gyro is connected
        StartCoroutine(DetectGyroCoroutine());

        pgc = p.GetComponent<PlayerGameplayController>();
        pmc = p.GetComponent<PlayerMenuController>();
        pfc = p.GetComponent<PlayerFanController>();

        GameObject g = p.GetComponentInChildren<BoardRollEffect>().gameObject;
        boardMR = g.GetComponent<MeshRenderer>();

        pgc.SetupGameplayControllerScript();
        pmc.SetupMenuControllerScript();
        pfc.SetupFanControllerScript();

        currentBoardSelection = (BoardType)PlayerPrefs.GetInt("BoardSelection", (int)currentBoardSelection);
        PlayerPrefs.SetInt("BoardSelection", (int)currentBoardSelection);
        BoardSelect(currentBoardSelection);

        if (debugSpeedEnabled)
            pgc.StartDebugSpeedControls();
    }

    IEnumerator DetectGyroCoroutine()
    {
        gyro = new SpatialData();

        //wait for the gyro to attach
        yield return new WaitForSeconds(0.1f);

        if (null != gyro.device && gyro.device.Attached)
            gamepadEnabled = false;
        else
        {
            gamepadEnabled = true;
            gyro.Close();
            gyro = null;
        }

        //update our controller scripts
        UpdateControlsType(gamepadEnabled);
    }

    public void UpdateDebugSpeedControls(bool dsEnabled)
    {
        debugSpeedEnabled = dsEnabled;

        if (dsEnabled == true)
            pgc.StartDebugSpeedControls();
        else
            pgc.StopDebugSpeedControls();
    }

    //updates our player controller scripts depending on what type of controls we are using
    //  if gPadEnabled == true, then our controllers will assume that we are using a xbox controller
    //  if false, then the gyro controls will be used
    public void UpdateControlsType(bool gPadEnabled)
    {
        gamepadEnabled = gPadEnabled;

        if (!gamepadEnabled)
        {
            if (gyro != null)
            {
                gyro.Close();
                gyro = null;
            }

            gyro = new SpatialData();
        }

        else if (gamepadEnabled)
        {
            if (gyro != null)
            {
                gyro.Close();
                gyro = null;
            }
        }

        pgc.UpdateGameplayControlsType(gPadEnabled, gyro);
        pmc.UpdateMenuControlsType(gPadEnabled, gyro);
    }

    //returns our controller specific movement variables and updates, currentBoardSelection and updates the board material
    public void BoardSelect(BoardType bSelect)
    {
        ManagerClasses.PlayerMovementVariables newBoardVariables = new ManagerClasses.PlayerMovementVariables();

        //update our current board selection
        currentBoardSelection = bSelect;
        PlayerPrefs.SetInt("BoardSelection", (int)currentBoardSelection);

        //update our current board material
        boardMR.material = boardMaterials[(int)currentBoardSelection];

        //return the proper variables, depending on if we are using a gamepad or gyro
        if (gamepadEnabled)
            GamepadBoardSelect(out newBoardVariables);
        else
            GyroBoardSelect(out newBoardVariables);

        pgc.UpdatePlayerBoard(newBoardVariables);

        //make sure our fan updates
        pfc.UpdateFanPercentage();
    }

    //helper function
    void GamepadBoardSelect(out ManagerClasses.PlayerMovementVariables pmv)
    {
        switch (currentBoardSelection)
        {
            case BoardType.Original:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    30f, 17f, 15f, 0.1f,
                    25f, 15f, 12f,
                    3.45f, 3.45f,
                    30f, 15f, 30f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachI:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    45f, 25f, 22f, 0.1f,
                    35f, 23f, 20f,
                    3.3f, 3.3f,
                    30f, 18f, 30f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachII:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    55f, 30f, 25f, 0.1f,
                    40f, 25f, 21f,
                    3.15f, 3.15f,
                    32f, 15f, 32f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachIII:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    70f, 42f, 37f, 0.1f,
                    50f, 35f, 31f,
                    3f, 3f,
                    35f, 10f, 35f,
                    1f, 1f, 1f, 5f
                    );
                break;
            default:
                pmv = customGamepadMovementVariables;
                break;
        }
    }

    //helper function
    void GyroBoardSelect(out ManagerClasses.PlayerMovementVariables pmv)
    {
        switch (currentBoardSelection)
        {
            case BoardType.Original:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    30f, 17f, 15f, 0.1f,
                    25f, 15f, 12f,
                    2.5f, 2.5f,
                    30f, 15f, 30f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachI:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    45f, 25f, 22f, 0.1f,
                    35f, 23f, 20f,
                    2.5f, 2.5f,
                    30f, 18f, 30f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachII:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    55f, 30f, 25f, 0.1f,
                    40f, 25f, 21f,
                    2.5f, 2.5f,
                    32f, 15f, 32f,
                    1f, 1f, 1f, 5f
                    );
                break;
            case BoardType.MachIII:
                pmv = new ManagerClasses.PlayerMovementVariables
                    (
                    70f, 42f, 37f, 0.1f,
                    50f, 35f, 31f,
                    2.5f, 2.5f,
                    35f, 10f, 35f,
                    1f, 1f, 1f, 5f
                    );
                break;
            default:
                pmv = customGyroMovementVariables;
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (gyro != null)
            gyro.Close();
    }

}