using UnityEngine;
public class RingProperties : MonoBehaviour
{
    [SerializeField] private bool duplicatePosition = false;
    public int positionInOrder = 0;
    public float bonusTime = 0.0f;
    public bool LastRingInScene => nextScene >= 0;
    public int nextScene = -1;
    public static Lap_Text_script laptext = null;
    public bool DuplicatePosition => duplicatePosition;
    private void Awake()
    {
        if (null == laptext) laptext = FindObjectOfType<Lap_Text_script>();
        if (duplicatePosition)
        {
            RingProperties[] rps = GetComponentsInChildren<RingProperties>();
            foreach (RingProperties rp in rps)
            {
                rp.duplicatePosition = duplicatePosition;
                rp.positionInOrder = positionInOrder;
                rp.bonusTime = bonusTime;
                rp.nextScene = nextScene;
            }
        }
    }
    private void Start()
    {
        if (GameMode.Cursed == GameManager.gameMode)
        {
            PlayerMovementVariables
                currPMV = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables,
                basePMV = BoardManager.GamepadBoardSelect(BoardType.MachII);
            if (BoardType.MachII != BoardManager.currentBoardSelection)
                bonusTime *= (basePMV.minSpeed + basePMV.restingAcceleration + basePMV.maxSpeed) / (currPMV.minSpeed + currPMV.restingSpeed + currPMV.maxSpeed);
        }
    }
}