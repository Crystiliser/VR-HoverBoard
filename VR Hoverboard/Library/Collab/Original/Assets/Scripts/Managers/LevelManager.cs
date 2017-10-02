using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//use our LevelManger to initialize any objects that carry from one scene to the next
public class LevelManager : MonoBehaviour
{
    [HideInInspector] public ManagerClasses.GameState gameState;

    GameManager gameManager;

    GameObject player;
    Transform playerTransform;
    PlayerMenuController menuController;

    //for transitions
    [HideInInspector] public bool fadeing = false;
    [HideInInspector] public bool doLoadOnce = true;
    [HideInInspector] public int nextScene;
    [HideInInspector] public bool HudOnOff = true;
    [HideInInspector] public bool RingPathIsOn = true;

    [HideInInspector] public bool makeSureMovementStaysLocked;

    //stores each player spawn point at each different level
    public Transform[] spawnPoints;


    public void SetupLevelManager(ManagerClasses.GameState s, GameObject p, GameManager g)
    {
        player = p;
        playerTransform = p.GetComponent<Transform>();
        gameState = s;
        gameManager = g;
        menuController = p.GetComponent<PlayerMenuController>();
    } 

    //for debugging
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Scene changed to: " + SceneManager.GetActiveScene().name);
    }

    public void DoSceneTransition(int sceneIndex)
    {
        nextScene = sceneIndex;
        gameState.currentState = GameStates.SceneTransition;
        EventManager.OnTriggerSelectionLock(true);
        EventManager.OnSetMovementLock(true);
        player.GetComponentInChildren<effectController>().disableAllEffects();
        doLoadOnce = true;
        fadeing = true;
        EventManager.OnTriggerFade();
    }

    public void UndoSceneTransitionLocks(Scene scene, LoadSceneMode mode)
    {
        //set our gameState based off of our scene build index
        switch (scene.buildIndex)
        {
            case 0: //Starting area
                EventManager.OnSetMovementLock(true);
                menuController.ToggleMenuMovement(true);
                EventManager.OnSetHudOnOff(false);
                makeSureMovementStaysLocked = true;
                gameState.currentState = GameStates.MainMenu;
                break;
            case 1: // HubWorld
                //do things like lock player movement here....
                EventManager.OnSetMovementLock(true);
                menuController.ToggleMenuMovement(false);
                EventManager.OnSetHudOnOff(false);
                makeSureMovementStaysLocked = true;
                gameState.currentState = GameStates.MainMenu;
                //TODO:: make sure to store our score and any other imformation for high scores if we want to...
                gameManager.scoreScript.score = 0;
                gameManager.scoreScript.ringHitCount = 0;
                break;             
            default:
                makeSureMovementStaysLocked = false;
                EventManager.OnSetHudOnOff(HudOnOff);
                EventManager.OnSetArrowOnOff(HudOnOff);
                applyGamemodeChanges();
                gameState.currentState = GameStates.GamePlay;
                break;
        }

        playerTransform.SetPositionAndRotation(spawnPoints[scene.buildIndex].position, spawnPoints[scene.buildIndex].rotation);

        EventManager.OnTriggerSelectionLock(false);

        if (!makeSureMovementStaysLocked)
        {
            EventManager.OnSetMovementLock(false);
        }
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
