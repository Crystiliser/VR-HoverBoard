using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardSelectedEffectController : MonoBehaviour
{
    BoardStandProperties[] boardEffects;
    BoardManager boardManager;
	void Start ()
    {
        boardEffects = gameObject.GetComponentsInChildren<BoardStandProperties>();
        boardManager = GameManager.instance.boardScript;
        EventManager.OnCallBoardMenuEffects();
    }

    private void OnEnable()
    {
        EventManager.OnUpdateBoardMenuEffects += setActiveBoard;
    }

    private void OnDisable()
    {
        EventManager.OnUpdateBoardMenuEffects -= setActiveBoard;
    }

    void setActiveBoard()
    {
        for (int i = 0; i < boardEffects.Length; i++)
        {
            if (boardEffects[i].boardType == boardManager.currentBoardSelection)
            {
                boardEffects[i].GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                boardEffects[i].GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
    }
}
