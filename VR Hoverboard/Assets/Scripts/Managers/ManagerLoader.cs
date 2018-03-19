using System.Collections.Generic;
using UnityEngine;
public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private GameObject gameManager = null;
    private static bool isLoaded = false;
    private void Awake()
    {
        Cursor.visible = false;
        if (!isLoaded)
        {
            isLoaded = true;
            Application.runInBackground = true;
            BuildDebugger.InitDebugger();
            Instantiate(gameManager);
        }
        try { Destroy(gameObject); } catch { }
    }
}