using TMPro;
public class ScoreTextUpdateScript : UnityEngine.MonoBehaviour
{
    private TextMeshProUGUI element = null;
    private void Start() => element = GetComponent<TextMeshProUGUI>();
    private void Update() => element.SetText(" " + ScoreManager.score + " ");
}