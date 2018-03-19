using UnityEngine;
using TMPro;
public class altimeterTextUpdater : MonoBehaviour
{
    private TextMeshProUGUI element = null;
    private Transform playerTransform = null;
    private void Start()
    {
        playerTransform = GameManager.player.transform;
        element = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        element.SetText(" " + (int)playerTransform.position.y + " ");
    }
}