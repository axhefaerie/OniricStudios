using UnityEngine;

public class TrashComponent : MonoBehaviour
{
    // AÑADIR LÓGICA DE, AL TIRAR ALGO, QUE SUEÑE UN SONIDO O ANIMACION O ALGO

    [SerializeField] private string trashSfx;

    public void PlayTrashSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(trashSfx);
    }
}
