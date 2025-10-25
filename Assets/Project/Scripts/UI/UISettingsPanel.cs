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
    void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);

        InitializeVolumeSliders();

        gameObject.SetActive(false);
    }
    private void InitializeVolumeSliders()
    {
        // Loading slider values from player prefs
        musicVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsValues.MusicVolume.ToString(), 0.7f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsValues.SFXVolume.ToString(), 0.7f);
        uiVolumeSlider.value = PlayerPrefs.GetFloat(PlayerPrefsValues.UIVolume.ToString(), 0.7f);

        // Subscribing methods to the sliders
        musicVolumeSlider.onValueChanged.AddListener((float f) => OnVolumeSliderChanged(AudioSourceType.MusicSource, f, PlayerPrefsValues.MusicVolume));
        sfxVolumeSlider.onValueChanged.AddListener((float f) => OnVolumeSliderChanged(AudioSourceType.SFXSource, f, PlayerPrefsValues.SFXVolume));
        uiVolumeSlider.onValueChanged.AddListener((float f) => OnVolumeSliderChanged(AudioSourceType.UISource, f, PlayerPrefsValues.UIVolume));

        // Updating volume 
        OnVolumeSliderChanged(AudioSourceType.MusicSource, musicVolumeSlider.value, PlayerPrefsValues.MusicVolume);
        OnVolumeSliderChanged(AudioSourceType.SFXSource, sfxVolumeSlider.value, PlayerPrefsValues.SFXVolume);
        OnVolumeSliderChanged(AudioSourceType.UISource, uiVolumeSlider.value, PlayerPrefsValues.UIVolume);

        soundPlayer.StartPlayingMainTheme();
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
    private void OnVolumeSliderChanged(AudioSourceType audioSourceType, float value, PlayerPrefsValues playerPrefsValue)
    {
        soundPlayer.SetVolume(audioSourceType, value);

        PlayerPrefs.SetFloat(playerPrefsValue.ToString(), value);
    }
}
