using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Inicializar los sliders con el volumen guardado
        if (musicSlider != null)
            musicSlider.value = AudioManager.Instance.MusicVolume;
        if (sfxSlider != null)
            sfxSlider.value = AudioManager.Instance.SfxVolume;

        // Conectar los eventos
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }
}
