using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VRUIManager : MonoBehaviour
{

    [Header("Ui tings")]
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button quitButton;
    public UnityEngine.UI.Button settingsButton;
    public UnityEngine.UI.Slider fovSlider;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public Camera vrCamera;

    [Header("Audio")]
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;

    [Header("Canvas")]
    public GameObject Canvas1;
    public GameObject Canvas2;
    private bool isCanvasSettingsActive = false;


    private int currentMusicIndex = 0;
    private int currentSFXIndex = 0;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnCanvasChange);

        if (fovSlider != null)
            fovSlider.onValueChanged.AddListener(UpdateFOV);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateFOV(float value)
    {
        if (vrCamera != null)
            vrCamera.fieldOfView = value;
    }

    public void UpdateMusicVolume(float value)
    {
        foreach (var music in musicSources)
        {
            music.volume = value;
        }
    }

    public void UpdateSFXVolume(float value)
    {
        foreach (var sfx in sfxSources)
        {
            sfx.volume = value;
        }
    }

    public void NextMusic()
    {
        if (musicSources.Length > 0)
        {
            musicSources[currentMusicIndex].Stop();
            currentMusicIndex = (currentMusicIndex + 1) % musicSources.Length;
            musicSources[currentMusicIndex].Play();
        }
    }

    public void NextSFX()
    {
        if (sfxSources.Length > 0)
        {
            sfxSources[currentSFXIndex].Stop();
            currentSFXIndex = (currentSFXIndex + 1) % sfxSources.Length;
            sfxSources[currentSFXIndex].Play();
        }
    }

    public void OnCanvasChange()
    {
        isCanvasSettingsActive = !isCanvasSettingsActive;

        if (isCanvasSettingsActive)
        {
            Canvas1.SetActive(true);
            Canvas2.SetActive(false);
        }
        else
        {
            Canvas1.SetActive(false);
            Canvas2.SetActive(true);
        }
    }


}
