using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

//our ManagerLoader prefab, will ensure that an instance of GameManager is loaded
public class GameManager : MonoBehaviour
{
    //store our game game state
    public ManagerClasses.GameState gameState = new ManagerClasses.GameState();

    //store our game mode
    public ManagerClasses.GameMode gameMode = new ManagerClasses.GameMode();

    //last level and mode we were in
    public GameModes lastMode;
    public int lastLevel = -1;

    //set our player prefab through the inspector
    [SerializeField] GameObject playerPrefab;

    //variable for singleton, static makes this variable the same through all GameManager objects
    public static GameManager instance = null;

    //variable to store our player clone
    public static GameObject player = null;

    //store our managers
    [HideInInspector] public ScoreManager scoreScript;
    [HideInInspector] public LevelManager levelScript;
    [HideInInspector] public BoardManager boardScript;
    [HideInInspector] public KeyInputManager keyInputScript;

    float originalCameraContainerHeight;

    void Awake()
    {
        //make sure we only have one instance of GameManager
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        //ensures that our game manager persists between scenes
        DontDestroyOnLoad(gameObject);

        //store our managers
        scoreScript = GetComponent<ScoreManager>();
        levelScript = GetComponent<LevelManager>();
        boardScript = GetComponent<BoardManager>();
        keyInputScript = GetComponent<KeyInputManager>();

        //Instantiate our player, store the clone, then make sure it persists between scenes
        player = Instantiate(playerPrefab);
        DontDestroyOnLoad(player);

        InitGame();
    }

    //using this instead of Awake() in our scripts allows us to control the execution order
    void InitGame()
    {
        originalCameraContainerHeight = player.GetComponentInChildren<CameraCounterRotate>().transform.localPosition.y;

        boardScript.SetupBoardManager(player);
        levelScript.SetupLevelManager(gameState, player, instance);
        scoreScript.SetupScoreManager();
        keyInputScript.setupKeyInputManager(gameState);

        StartCoroutine(CalibrationCoroutine());
    }

    public IEnumerator CalibrationCoroutine()
    {
        if (VRDevice.isPresent)
        {
            //wait for the end of the frame so we can catch positional data from the VR headset
            yield return new WaitForEndOfFrame();

            Transform cameraContainer = player.GetComponentInChildren<CameraCounterRotate>().transform;

            Vector3 playerPosition = player.GetComponent<Transform>().position;
            Vector3 originalPosition = new Vector3(playerPosition.x, playerPosition.y + originalCameraContainerHeight, playerPosition.z);
            Quaternion playerRotation = player.GetComponent<Transform>().rotation;

            //set the cameraContainer back on top of the board, in case we are re-calibrating
            cameraContainer.SetPositionAndRotation(originalPosition, playerRotation);          

            Vector3 headPosition = player.GetComponentInChildren<ScreenFade>().transform.localPosition;
            Vector3 headRotation = player.GetComponentInChildren<ScreenFade>().transform.eulerAngles;

            //rotate, then translate

            //rotate the camera so that it is rotated in the same direction as the board
            float yRotation = Mathf.DeltaAngle(headRotation.y, cameraContainer.eulerAngles.y);
            cameraContainer.Rotate(Vector3.up * yRotation);

            //headPosition acts as though the cameraContainer is the ground
            //so if headPosition.y = 1.4, then the camera will be sitting 1.4 meters above the cameraContainer
            //therefore, translate the cameraContainer in opposite directions of wherever the headPosition is
            cameraContainer.Translate(headPosition * -1f);
        }
    }

    public void Update()
    {
        if (gameState.currentState == GameStates.SceneTransition)
        {
            //keep going until fade finishes
            if (!levelScript.fadeing && levelScript.doLoadOnce)
            {
                //if (levelScript.nextScene != SceneManager.GetActiveScene().buildIndex)
                //{
                //    //AsyncOperation opertion = SceneManager.LoadSceneAsync(levelScript.nextScene, LoadSceneMode.Single);
                //
                //    //levelScript.loadOpertion.allowSceneActivation = true;
                //}

                levelScript.doLoadOnce = false;
            }
        }
    }
}