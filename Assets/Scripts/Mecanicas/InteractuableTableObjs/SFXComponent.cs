using UnityEngine;
using UnityEngine.EventSystems;

public class SFXComponent : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración del sonido")]
    [SerializeField] private string sfxSound;

    [Header("Reproducción automática")]
    [SerializeField] private bool playOnClick = false;

    // Reproduce el efecto de sonido
    public void PlaySfxSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(sfxSound);
    }

    // Se ejecuta al hacer click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (playOnClick)
        {
            PlaySfxSound();
        }
    }
}