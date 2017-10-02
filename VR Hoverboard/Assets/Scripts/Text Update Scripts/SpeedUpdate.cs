using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedUpdate : MonoBehaviour
{
    Rigidbody playerRB;
    TextMeshProUGUI element;

	void Start ()
    {
        playerRB = GetComponentInParent<Rigidbody>();
        element = GetComponent<TextMeshProUGUI>();
        element.color = new Color(1, 0, 1);
    }

	void Update ()
    {
        element.SetText(playerRB.velocity.magnitude.ToString("##"));
	}
}
