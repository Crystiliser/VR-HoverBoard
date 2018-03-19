using TMPro;
public class RingCountTextUpdate : UnityEngine.MonoBehaviour
{
    private TextMeshProUGUI element = null;
    private void Start() => element = GetComponent<TextMeshProUGUI>();
    private void Update() => element.SetText(" " + ScoreManager.ringHitCount + " ");
}