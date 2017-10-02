using UnityEngine;
using TMPro;

public class WorldPortalText : MonoBehaviour
{
    [SerializeField]
    WorldPortalProperties properties = null;
    [SerializeField]
    TextMeshPro theName = null;
    private void Awake()
    {
        if (null == properties)
            properties = GetComponent<WorldPortalProperties>();
        if (null == theName)
            theName = GetComponentInChildren<TextMeshPro>();
    }
    private void OnEnable()
    {
        properties.OnSceneIndexChanged += UpdateText;
        UpdateText();
    }
    private void OnDisable()
    {
        properties.OnSceneIndexChanged -= UpdateText;
    }
    private void UpdateText()
    {
        string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(properties.SceneIndex);
        theName.SetText(path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1));
    }
}