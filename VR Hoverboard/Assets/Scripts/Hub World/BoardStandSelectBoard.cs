using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStandSelectBoard : SelectedObject
{
    BoardManager boardManager;

    //our material and board types are stored in the BoardStandScript
    BoardStandProperties selectionVariables;

    Material renderMat = null;

    new private void Start()
    {
        base.Start();
        boardManager = GameManager.instance.boardScript;

        selectionVariables = GetComponentInParent<BoardStandProperties>();
        renderMat = gameObject.GetComponent<Renderer>().material;
        //Color boardColor = renderMat.color;
        renderMat.SetColor("_EmissionColor", Color.black);
        //renderMat.DisableKeyword("_EMISSION");
    }

    protected override void selectedFuntion()
    {
        base.selectedFuntion();
        //renderMat.EnableKeyword("_EMISSION");
        renderMat.SetColor("_EmissionColor", Color.white);
    }
    protected override void deSelectedFunction()
    {
        base.deSelectedFunction();
        //renderMat.DisableKeyword("_EMISSION");
        renderMat.SetColor("_EmissionColor", Color.black);
    }
    override public void selectSuccessFunction()
    {
        //set the player board to one of our pre-defined boards
        boardManager.BoardSelect(selectionVariables.boardType);
        EventManager.OnCallBoardMenuEffects();
    }

}
