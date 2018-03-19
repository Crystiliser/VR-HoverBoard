using UnityEngine;
public class BoardStandSelectBoard : SelectedObject
{
    [SerializeField] private Transform boardCopy = null;
    private static Transform playerBoard = null;
    private BoardStandProperties selectionVariables = null;
    private bool animationRunning = false;
    private float tVal = 0.0f, invAnimationTime = 1.0f;
    private Vector3 startPosition, startScale;
    private Quaternion startRotation, ogLocalRot;
    new private void Start()
    {
        base.Start();
        selectionVariables = GetComponentInParent<BoardStandProperties>();
        ogLocalRot = boardCopy.localRotation;
        if (null == playerBoard)
            playerBoard = GameManager.player.GetComponentInChildren<BoardSelector>(true).CurrentBoard;
    }
    protected override void SuccessFunction()
    {
        boardCopy.localPosition = Vector3.zero;
        boardCopy.localRotation = ogLocalRot;
        boardCopy.localScale = Vector3.one;
        boardCopy.parent = null;
        startPosition = boardCopy.position;
        startRotation = boardCopy.rotation;
        startScale = boardCopy.lossyScale;
        boardCopy.gameObject.SetActive(true);
        invAnimationTime = 1.0f / WaitTime;
        tVal = 0.0f;
        animationRunning = true;
    }
    private void OnEndAnimation()
    {
        animationRunning = false;
        boardCopy.parent = transform;
        boardCopy.gameObject.SetActive(false);
        BoardManager.BoardSelect(selectionVariables.boardType);
        EventManager.OnCallBoardMenuEffects();
    }
    new private void Update()
    {
        base.Update();
        if (animationRunning)
        {
            boardCopy.position = Vector3.Lerp(startPosition, playerBoard.position, tVal);
            boardCopy.rotation = Quaternion.Slerp(startRotation, playerBoard.rotation, tVal);
            boardCopy.localScale = Vector3.Lerp(startScale, playerBoard.lossyScale, tVal);
            tVal += Time.deltaTime * invAnimationTime;
            if (tVal >= 1.0f)
                OnEndAnimation();
        }
    }
}