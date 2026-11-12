using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;   // música
    [SerializeField] private AudioSource sfxSource;     // efectos

    // Catálogos: asigna los clips en el Inspector
    [SerializeField] private List<AudioClip> musicClips;
    [SerializeField] private List<AudioClip> sfxClips;

    // Diccionarios internos (nombre → clip)
    private Dictionary<string, AudioClip> musicDict = new();
    private Dictionary<string, AudioClip> sfxDict = new();

    public float MusicVolume => musicSource.volume;
    public float SfxVolume => sfxSource.volume;


    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource.loop = true;
        // Construir los diccionarios a partir de las listas
        foreach (var clip in musicClips)
            if (clip != null) musicDict[clip.name] = clip;
        foreach (var clip in sfxClips)
            if (clip != null) sfxDict[clip.name] = clip;
    }

    private void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume", 1f);
    }

    // --- Música: por nombre ---
    public void PlayMusic(string clipName)
    {
        if (musicDict.TryGetValue(clipName, out var clip))
        {
            // Si ya está sonando este mismo clip, no lo reinicies
            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Música '{clipName}' no encontrada");
        }
    }

    // --- Efecto: por nombre ---
    public void PlaySfx(string clipName)
    {
        if (sfxDict.TryGetValue(clipName, out var clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"SFX '{clipName}' no encontrado");
    }

    // --- Volúmenes ---
    public void SetMusicVolume(float vol)
    {
        musicSource.volume = vol;
        PlayerPrefs.SetFloat("musicVolume", vol);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float vol)
    {
        sfxSource.volume = vol;
        PlayerPrefs.SetFloat("sfxVolume", vol);
        PlayerPrefs.Save();
    }
}
