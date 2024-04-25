using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class OptionsScreenManager : MonoBehaviour
{
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;

    private void Start()
    {
        // If launched for the first time, set audio to max
        if (PlayerPrefs.GetInt(Paths.FIRST_TIME_LAUNCH) == 0){
            PlayerPrefs.SetInt(Paths.FIRST_TIME_LAUNCH, 1);

            PlayerPrefs.SetFloat(Paths.MUSIC_VOLUME_PATH, 1f);
            PlayerPrefs.SetFloat(Paths.EFFECTS_VOLUME_PATH, 1f);
        }

        Settings.MUSIC_VOULME = PlayerPrefs.GetFloat(Paths.MUSIC_VOLUME_PATH);
        Settings.EFFECT_VOULME = PlayerPrefs.GetFloat(Paths.EFFECTS_VOLUME_PATH);

        musicVolumeSlider.value = Settings.MUSIC_VOULME;
        effectsVolumeSlider.value = Settings.EFFECT_VOULME;

        AudioHandler.Instance.UpdateMusicVolume(Settings.MUSIC_VOULME);
        AudioHandler.Instance.UpdateEffectVolume(Settings.EFFECT_VOULME);

    }

    public void OnMusicVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(Paths.MUSIC_VOLUME_PATH, value);
        Settings.MUSIC_VOULME = value;
        AudioHandler.Instance.UpdateMusicVolume(Settings.MUSIC_VOULME);
    }

    public void OnEffectsVolumeChange(float value)
    {
        PlayerPrefs.SetFloat(Paths.EFFECTS_VOLUME_PATH, value);
        Settings.EFFECT_VOULME = value;
        AudioHandler.Instance.UpdateEffectVolume(Settings.EFFECT_VOULME);
    }
}
