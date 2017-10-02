using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPortalScript : MonoBehaviour
{
    [SerializeField] bool isDemoMode = false;

    System.Type boxCollider;
    PlayerMenuController pmc;

    ManagerClasses.GameMode gameMode;
    LevelMenu levelMenuScript;

    private void Start()
    {
        boxCollider = typeof(UnityEngine.CapsuleCollider);
        pmc = GameManager.player.GetComponent<PlayerMenuController>();
        gameMode = GameManager.instance.gameMode;

        //if we are in the hub world
        if (SceneManager.GetActiveScene().buildIndex == 1)
            levelMenuScript = GameObject.Find("LevelMenu").GetComponent<LevelMenu>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == boxCollider && other.gameObject.tag == "Board")
        {
            if (isDemoMode && levelMenuScript != null)
            {
                while (gameMode.currentMode != GameModes.Continuous)
                    levelMenuScript.NextGameMode();
            }

            int level = GetComponentInParent<WorldPortalProperties>().SceneIndex;
            GameManager gameManager = GameManager.instance;
            gameManager.lastPortalBuildIndex = level;
            gameManager.lastMode = gameManager.gameMode.currentMode;

            EventManager.OnTriggerTransition(level);
            pmc.ToggleMenuMovement(true);
        }
    }
}
