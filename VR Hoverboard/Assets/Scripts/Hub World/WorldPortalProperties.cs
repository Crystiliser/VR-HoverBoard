using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WorldPortalScript uses this property
public class WorldPortalProperties : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex = 0;
    public delegate void SceneIndexChangeEvent();
    public SceneIndexChangeEvent OnSceneIndexChanged;
    public int SceneIndex
    {
        get
        {
            return sceneIndex;
        }
        set
        {
            if (sceneIndex != value)
            {
                sceneIndex = value;
                if (null != OnSceneIndexChanged)
                    OnSceneIndexChanged();
            }
        }
    }
}