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

    }
    private void ClosePanel()
    {
        
    }
}
