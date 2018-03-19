using UnityEngine;
using TMPro;
public class SpeedUpdate : MonoBehaviour
{
    private Rigidbody playerRB = null;
    private TextMeshProUGUI element = null;
    private void Start()
    {
        playerRB = GetComponentInParent<Rigidbody>();
        element = GetComponent<TextMeshProUGUI>();
        //element.color = Color.magenta;
    }
    private void Update()
    {
        element.SetText(playerRB.velocity.magnitude.ToString("##"));
    }
}