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
    void Awake()
    {
        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);

        InitializeTransforms();
    }
    private void OnClickPressed(ClickPressed_event e)
    {

    }
    private void InitializeTransforms()
    {
        Vector3[] canvasAngles = new Vector3[4];
        canvasRT.GetWorldCorners(canvasAngles);

        var leftHandPos = leftHand.position;
        leftHandPos.x = canvasAngles[0].x;
        leftHand.position = leftHandPos;

        var rightHandPos = rightHand.position;
        rightHandPos.x = canvasAngles[2].x;
        rightHand.position = rightHandPos;
    }
}
