using UnityEngine;
public class playerInsideBoundary : MonoBehaviour
{
    private PlayerRespawn playerRespawnScript = null;
    private void Start() => playerRespawnScript = GetComponent<PlayerRespawn>();
    private void OnCollisionEnter(Collision collision)
    {
        if ("Boundary" == collision.gameObject.tag)
            playerRespawnScript.RespawnPlayer(ScoreManager.prevRingTransform, 5.0f + ScoreManager.prevRingBonusTime);
    }
}