using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class altimeterTextUpdater : MonoBehaviour
{
    TextMeshProUGUI element;
    Transform playerTransform;

	void Start ()
    {
        playerTransform = GameManager.player.GetComponent<Transform>();
        element = GetComponent<TextMeshProUGUI>();
	}
	
	void Update ()
    {
        string textToWrite = " " + (int)playerTransform.position.y + " ";
        element.SetText(textToWrite);       
	}
}
