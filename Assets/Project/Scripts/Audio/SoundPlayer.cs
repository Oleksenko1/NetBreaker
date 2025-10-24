using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NetBreaker.Assets.Project.Scripts.Audio;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;
    private SFXLoader sfxLoader;
    private Dictionary<AudioSourceType, AudioSource> audioSources = new Dictionary<AudioSourceType, AudioSource>();
    void Awake()
    {
        sfxLoader = new SFXLoader();
        sfxLoader.Initialize();

        audioSources.Add(AudioSourceType.SFXSource, sfxSource);
        audioSources.Add(AudioSourceType.UISource, uiSource);
    }
    public void PlayDefaultSFX(SFXType sFXType, float volume = 1f)
    {
        PlaySFXAsync(sFXType, volume, AudioSourceType.SFXSource).Forget();
    }
    public void PlayUI_SFX(SFXType sFXType, float volume = 1f)
    {
        PlaySFXAsync(sFXType, volume, AudioSourceType.UISource).Forget();
    }

    private async UniTask PlaySFXAsync(SFXType sfxType, float volume, AudioSourceType audioSourceType)
    {
        AudioClip clip = await sfxLoader.GetClipAsync(sfxType);
        AudioSource audioSource = audioSources[audioSourceType];

        audioSource.PlayOneShot(clip, volume);
    }
}
public enum AudioSourceType
{
    SFXSource,
    UISource
}
