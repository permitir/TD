using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public SettingsManager settingsManager;

    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("Master", masterVol.value);
        PlayerPrefs.SetFloat("Master", masterVol.value);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("Music", musicVol.value);
        PlayerPrefs.SetFloat("Music", musicVol.value);
        PlayerPrefs.Save();
    }

    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFX", sfxVol.value);
        PlayerPrefs.SetFloat("SFX", sfxVol.value);
        PlayerPrefs.Save();
    }
}
