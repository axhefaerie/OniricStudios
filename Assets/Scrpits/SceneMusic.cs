using UnityEngine;
using UnityEngine.Audio;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private string musicName;   // ej. "menu_bgm"

    private void Start()
    {
        AudioManager.Instance.PlayMusic(musicName);
    }

}
