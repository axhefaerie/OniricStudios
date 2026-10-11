using UnityEngine;

public class TrashComponent : MonoBehaviour
{
    // AÑADIR LÓGICA DE, AL TIRAR ALGO, QUE SUEÑE UN SONIDO O ANIMACION O ALGO

    public AudioSource audioSource;
    public AudioClip trashSound;

    public void PlayTrashSound()
    {
        if (audioSource != null && trashSound != null)
        {
            audioSource.PlayOneShot(trashSound);
        }
    }
}
