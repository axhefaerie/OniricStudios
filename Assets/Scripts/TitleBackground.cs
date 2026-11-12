using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleBackground : MonoBehaviour
{
    public Image background;          // el Image del Canvas (o usa SpriteRenderer si es 2D)
    public Sprite darkSprite;         // escenario sin luces (default)
    public Sprite dimSprite;          // casi luz
    public Sprite litSprite;          // luz

    public float minWait = 5f;        // espera mínima antes de la secuencia
    public float maxWait = 12f;       // espera máxima
    public float dimDuration = 0.35f; // cuánto dura "casi luz"
    public float litDuration = 0.25f; // cuánto dura "luz"
    public float fadeDuration = 0.15f; // fundido entre imágenes (0 = corte seco)

    private void Start()
    {
        if (background != null && darkSprite != null)
            background.sprite = darkSprite;

        StartCoroutine(FlashLoop());
    }

    private IEnumerator FlashLoop()
    {
        while (true)
        {
            // Espera aleatoria con la escena oscura
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            // Secuencia: casi luz → luz → vuelta a oscuro
            yield return SetSprite(dimSprite, dimDuration);
            yield return SetSprite(litSprite, litDuration);
            yield return SetSprite(darkSprite, fadeDuration);
        }
    }

    // Cambia el sprite (con fundido opcional) y espera 'hold' segundos
    private IEnumerator SetSprite(Sprite target, float hold)
    {
        if (target == null) yield break;

        if (fadeDuration > 0f)
        {
            float t = 0f;
            Color c = background.color;

            // Funde a transparente
            while (t < fadeDuration * 0.5f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, t / (fadeDuration * 0.5f));
                background.color = c;
                yield return null;
            }

            background.sprite = target;

            // Funde de vuelta a opaco
            t = 0f;
            while (t < fadeDuration * 0.5f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / (fadeDuration * 0.5f));
                background.color = c;
                yield return null;
            }

            c.a = 1f;
            background.color = c;
        }
        else
        {
            background.sprite = target;
        }

        yield return new WaitForSeconds(hold);
    }
}
