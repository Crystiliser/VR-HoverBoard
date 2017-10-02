using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupControlsScreen : MonoBehaviour
{
    BoardManager boardManager;
    [SerializeField] Sprite[] controlsImages;
    [SerializeField] GameObject controlsObject;

	// Use this for initialization
	void Start ()
    {
        boardManager = GameManager.instance.boardScript;
        StartCoroutine(WaitForDetection());
	}
	
	IEnumerator WaitForDetection()
    {
        yield return new WaitForSeconds(0.15f);

        if (boardManager.gamepadEnabled)
            controlsObject.GetComponent<Image>().sprite = controlsImages[0];
        else
            controlsObject.GetComponent<Image>().sprite = controlsImages[1];
    }
}
