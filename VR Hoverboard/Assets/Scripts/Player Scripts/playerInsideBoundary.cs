using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInsideBoundary : MonoBehaviour
{
    PlayerRespawn playerRespawnScript;
    ScoreManager scoreScript;

    private void Start()
    {
        playerRespawnScript = gameObject.GetComponent<PlayerRespawn>();
        scoreScript = GameManager.instance.scoreScript;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boundary")
        {
            playerRespawnScript.RespawnPlayer(scoreScript.prevRingTransform, 5.0f + scoreScript.prevRingBonusTime);
        }
    }

}
