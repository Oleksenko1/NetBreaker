using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIMenuButtons : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button toggleMenuBtn;
    [SerializeField] private Button openSettingsBtn;
    [SerializeField] private Button openUpgradesBtn;
    [Space(10)]
    [SerializeField] private VerticalLayoutGroup verticalLayout;
    [Inject] private UIUpgradesPanel uIUpgradesPanel;
    [Inject] private SoundPlayer soundPlayer;
    private RectTransform toggleMenuBtnRT;
    private bool isMenuOpen;
    private const float openSpacingOfLayout = 40f;
    private const float closedSpacingOfLayout = -160;

    private Tweener spacingTween;
    private Tweener rotationTween;

    private void Awake()
    {
        toggleMenuBtnRT = toggleMenuBtn.GetComponent<RectTransform>();
        toggleMenuBtn.onClick.AddListener(OnToggleMenuPressed);

        openUpgradesBtn.onClick.AddListener(uIUpgradesPanel.OpenPanel);

        verticalLayout.spacing = closedSpacingOfLayout;
        toggleMenuBtnRT.rotation = Quaternion.identity;
    }

    private void OnToggleMenuPressed()
    {
        soundPlayer.PlayUI_SFX(SFXType.UIClick);

        float targetSpacing = isMenuOpen ? closedSpacingOfLayout : openSpacingOfLayout;
        Vector3 rotation = isMenuOpen ? Vector3.zero : Vector3.forward * 180;
        float animDuration = 0.3f;

        spacingTween?.Kill();
        rotationTween?.Kill();

        spacingTween = DOTween.To(
            () => verticalLayout.spacing,
            x => verticalLayout.spacing = x,
            targetSpacing,
            animDuration
        ).SetEase(Ease.InCubic);

        DOVirtual.DelayedCall(animDuration * 0.5f, () =>
        {
            rotationTween = toggleMenuBtnRT.DORotate(rotation, animDuration / 2)
                .SetEase(Ease.Linear);
        });

        isMenuOpen = !isMenuOpen;
    }

    private void OnDestroy()
    {
        spacingTween?.Kill();
        rotationTween?.Kill();
    }
}