using DG.Tweening;
using TMPro;
using UnityEngine;

public class ClickPopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro bitAmountTxt;
    private ClickPopupManager popupManager;
    private Sequence animSequence;
    public void Initialize(ClickPopupManager popupManager)
    {
        this.popupManager = popupManager;

        bitAmountTxt.SetText("");

        gameObject.SetActive(false);
    }
    public void ShowPopup(BigNum num, Vector3 position)
    {
        bitAmountTxt.SetText(num.ToString());

        transform.position = position;

        PlayAnimation();
    }
    private void PlayAnimation()
    {
        DOTween.Kill(transform);

        if (animSequence != null)
        {
            animSequence.Kill();
        }

        animSequence = DOTween.Sequence();
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        animSequence.Append(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        animSequence.Append(transform.DOShakePosition(1f, 0.02f));
        animSequence.Append(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        animSequence.OnComplete(() => popupManager.ReturnObject(this));

        animSequence.Play();
    }
}
