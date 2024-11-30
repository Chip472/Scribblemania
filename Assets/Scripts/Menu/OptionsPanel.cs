using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Slider soundEffectSlider;

    private Resolution[] resolutions;

    private void Start()
    {
        // Resolution Dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Quality Dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Sound Effect Slider
        soundEffectSlider.value = AudioListener.volume;

        LoadSettings();
    }

    private void LoadSettings()
    {
        // Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;

        // Resolution
        int resolutionIndex = PlayerPrefs.GetInt("Resolution", resolutions.Length - 1);
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);
        SetResolution(resolutionIndex);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Quality
        int qualityIndex = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityDropdown.value = qualityIndex;
        qualityDropdown.RefreshShownValue();

        // Volume
        float volume = PlayerPrefs.GetFloat("Volume", 1f); // Default to full volume
        AudioListener.volume = volume;
        soundEffectSlider.value = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetVolume(float sliderValue)
    {
        AudioListener.volume = sliderValue;
        PlayerPrefs.SetFloat("Volume", sliderValue);
    }
}
