using DG.Tweening;
using UnityEngine;

public class PlayerKeyboard : MonoBehaviour
{
    [Header("Animation parametrs")]
    [SerializeField] private float jumpStrengthMin = 0.1f;
    [SerializeField] private float jumpStrengthMax = 1f;
    [SerializeField] private float rotationStrength;
    [SerializeField] private float animDuration;
    private Vector3 startPosition;
    private int orientation = 1;
    void Awake()
    {
        startPosition = transform.position;

        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        // Flipping the orientation
        orientation *= -1;

        DOTween.Kill(transform);

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        float randomMult = Random.Range(0, 1f);
        float jumpStrength = Mathf.Lerp(jumpStrengthMin, jumpStrengthMax, randomMult);
        float rotation = rotationStrength * randomMult * orientation;

        transform.DOLocalJump(startPosition, jumpStrength, 1, animDuration / 2f);
        transform.DOPunchRotation(Vector3.forward * rotation, animDuration);
    }
}
