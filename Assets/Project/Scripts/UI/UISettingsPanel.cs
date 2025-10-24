using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UISettingsPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [Space(10)]
    [SerializeField] private Button closeBtn;
    [Inject] private SoundPlayer soundPlayer;
    void Awake()
    {
        closeBtn.onClick.AddListener(ClosePanel);

        gameObject.SetActive(false);
    }
    public void OpenPanel()
    {
        soundPlayer.PlayUI_SFX(SFXType.UIOpenPanelBtn);

        gameObject.SetActive(true);
    }
    private void ClosePanel()
    {
        soundPlayer.PlayUI_SFX(SFXType.UIClose);

        gameObject.SetActive(false);
    }
}
