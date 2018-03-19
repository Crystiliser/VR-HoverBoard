using UnityEngine;
public class WorldPortalScript : MonoBehaviour
{
    [SerializeField] private bool isDemoMode = false;
    private static readonly System.Type boxCollider = typeof(CapsuleCollider);
    private PlayerMenuController pmc = null;
    private void Start() => pmc = GameManager.player.GetComponent<PlayerMenuController>();
    private void OnTriggerEnter(Collider other)
    {
        if (boxCollider == other.GetType() && "Board" == other.gameObject.tag && GetComponent<Renderer>().isVisible)
        {

            if (isDemoMode)
                GameManager.gameMode = GameMode.Continuous;
            int level = GetComponentInParent<WorldPortalProperties>().SceneIndex;
            GameManager.lastPortalBuildIndex = level;
            EventManager.OnTriggerTransition(level);
            pmc.ToggleMenuMovement(true);
        }

    }
}