using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("Master", masterVol.value);
    }

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("Music", musicVol.value);
    }

    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFX", sfxVol.value);
    }
}
