using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public SettingsManager settingsManager;

    public Slider masterVol, musicVol, sfxVol;
    public AudioMixer mainAudioMixer;

    public void LoadAudioSettings()
    {
        // Get saved volumes from SaveSystem
        float masterVolume = SaveSystem.instance.GetMasterVolume();
        float musicVolume = SaveSystem.instance.GetMusicVolume();
        float sfxVolume = SaveSystem.instance.GetSFXVolume();

        // Set mixer values
        mainAudioMixer.SetFloat("Master", masterVolume);
        mainAudioMixer.SetFloat("Music", musicVolume);
        mainAudioMixer.SetFloat("SFX", sfxVolume);

        // Set slider positions
        if (masterVol != null) masterVol.value = masterVolume;
        if (musicVol != null) musicVol.value = musicVolume;
        if (sfxVol != null) sfxVol.value = sfxVolume;
    }

    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("Master", masterVol.value); // setting volume to float
        SaveSystem.instance.SetMasterVolume(masterVol.value); // saving
    }

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("Music", musicVol.value); // setting volume to float
        SaveSystem.instance.SetMusicVolume(musicVol.value); // saving
    }

    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFX", sfxVol.value); // setting volume to float
        SaveSystem.instance.SetSFXVolume(sfxVol.value); // saving

    }
}
