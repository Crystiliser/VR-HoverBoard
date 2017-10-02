using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//use our LevelManger to initialize any objects that carry from one scene to the next
public class LevelManager : MonoBehaviour
{
    [HideInInspector] public ManagerClasses.GameState gameState;

    GameManager gameManager;
    ScreenFade screenFade;

    GameObject player;
    Transform playerTransform;
    PlayerMenuController menuController;

    //for transitions
    [HideInInspector] public int nextScene;
    [HideInInspector] public bool HudOnOff = true;
    [HideInInspector] public bool RingPathIsOn = true;

    //stores each player spawn point at each different level
    public Transform[] spawnPoints;

    public void SetupLevelManager(ManagerClasses.GameState s, GameObject p, GameManager g)
    {
        player = p;
        playerTransform = p.GetComponent<Transform>();
        gameState = s;
        gameManager = g;
        menuController = p.GetComponent<PlayerMenuController>();
        screenFade = p.GetComponentInChildren<ScreenFade>();
    } 

    //for debugging
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildDebugger.WriteLine("Scene changed to: " + scene.name);
    }

    public void DoSceneTransition(int sceneIndex)
    {
        nextScene = sceneIndex;
        EventManager.OnTriggerSelectionLock(true);
        player.GetComponentInChildren<effectController>().disableAllEffects();
        screenFade.StartTransitionFade();  
    }

    public void UndoSceneTransitionLocks(Scene scene, LoadSceneMode mode)
    {
        //set our gameState based off of our scene build index
        switch (scene.buildIndex)
        {
            case 0: //Starting area
                menuController.ToggleMenuMovement(true);
                EventManager.OnSetHudOnOff(false);
                gameState.currentState = GameStates.MainMenu;
                break;
            case 1: // HubWorld
                //moved to a button switch after first startup
                if (gameManager.lastPortalBuildIndex == -1)
                {
                    menuController.ToggleMenuMovement(false);
                }
                           
                EventManager.OnSetHudOnOff(false);
                gameState.currentState = GameStates.MainMenu;
                gameManager.scoreScript.score = 0;
                gameManager.scoreScript.ringHitCount = 0;
                gameManager.scoreScript.firstPortal = true;
                break;             
            default:
                EventManager.OnSetHudOnOff(HudOnOff);
                EventManager.OnSetArrowOnOff(HudOnOff);
                applyGamemodeChanges();
                gameState.currentState = GameStates.GamePlay;
                break;
        }

        playerTransform.SetPositionAndRotation(spawnPoints[scene.buildIndex].position, spawnPoints[scene.buildIndex].rotation);

        EventManager.OnTriggerSelectionLock(false);
    }

    void applyGamemodeChanges()
    {
        switch (gameManager.gameMode.currentMode)
        {
            case GameModes.Continuous:

                EventManager.OnCallSetRingPath(RingPathIsOn);
                break;
            case GameModes.Cursed:

                EventManager.OnCallSetRingPath(RingPathIsOn);
                break;
            case GameModes.Free:

                EventManager.OnCallSetRingPath(false);
                break;
            default:
                break;
        }
    }

    public void OnEnable()
    {
        EventManager.OnTransition += DoSceneTransition;
        SceneManager.sceneLoaded += UndoSceneTransitionLocks;
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    public void OnDisable()
    {
        EventManager.OnTransition -= DoSceneTransition;
        SceneManager.sceneLoaded -= UndoSceneTransitionLocks;
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

}
