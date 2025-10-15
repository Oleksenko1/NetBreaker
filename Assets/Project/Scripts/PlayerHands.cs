using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHands : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float downMoveOffset;
    [Header("Components")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [Space(10)]
    [SerializeField] private RectTransform canvasRT;
    private Vector3 leftHandStartPos;
    private Vector3 rightHandStartPos;
    Vector3[] canvasAngles = new Vector3[4];
    private bool lastHandLeft = true; // If last moved hand in animation was left
    void Awake()
    {
        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);

        InitializeTransforms();
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        PlayClickAnim();
    }
    private void ResetHandsPos()
    {
        Transform handTR = lastHandLeft ? rightHand : leftHand;
        Vector3 handStartPos = lastHandLeft ? rightHandStartPos : leftHandStartPos;

        DOTween.Kill(handTR);
        handTR.position = handStartPos;
    }
    private void PlayClickAnim()
    {
        ResetHandsPos();

        Transform handTR = lastHandLeft ? rightHand : leftHand;
        Vector3 handStartPos = lastHandLeft ? rightHandStartPos : leftHandStartPos;

        handTR.DOMoveY(handStartPos.y + downMoveOffset, 0.4f).SetEase(Ease.OutBack);

        lastHandLeft = !lastHandLeft;
    }
    private void InitializeTransforms()
    {
        Vector3[] canvasAngles = new Vector3[4];
        canvasRT.GetWorldCorners(canvasAngles);

        rightHandStartPos = rightHand.position;
        rightHandStartPos.x = canvasAngles[2].x;
        rightHandStartPos.y -= downMoveOffset;

        leftHandStartPos = leftHand.position;
        leftHandStartPos.x = canvasAngles[0].x;
        leftHandStartPos.y -= downMoveOffset;

        // Invoked 2 times so both hands would reset
        PlayClickAnim();
        PlayClickAnim();
    }
}
