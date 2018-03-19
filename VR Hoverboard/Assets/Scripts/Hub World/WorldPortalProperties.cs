using UnityEngine;
public class WorldPortalProperties : MonoBehaviour
{
    [SerializeField] private int sceneIndex = 0;
    public delegate void SceneIndexChangeEvent();
    public event SceneIndexChangeEvent OnSceneIndexChanged;
    public int SceneIndex
    {
        get { return sceneIndex; }
        set
        {
            if (sceneIndex != value)
            {
                sceneIndex = value;
                OnSceneIndexChanged?.Invoke();
            }
        }
    }
}