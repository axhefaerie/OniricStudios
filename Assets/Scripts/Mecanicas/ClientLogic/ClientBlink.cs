using UnityEngine;

// Este script maneja la logica del pestañeo (animación idle) de los personajes
public class ClientBlink : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite openSprite;      // sprite con ojos abiertos (opcional: si es null usa el actual)
    public Sprite closedSprite;    // sprite con ojos cerrados

    public float minInterval = 2f;  // tiempo minimo de intervalo entre parpadeos
    public float maxInterval = 5f;  // tiempo maximo
    public float blinkDuration = 0.12f;   // lo que dura el parpadeo

    // estado del parpadeo
    private float nextBlinkTime;
    private float blinkEndTime;
    private bool isBlinking;
    private bool paused;

    private void Awake()
    {
        if (spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }    // Pilla el sprite renderer del cliente 
        nextBlinkTime = Time.time + Random.Range(minInterval, maxInterval); // El proximo parpadeo
    }

    private void Start()
    {
        var randomizer = GetComponent<RecipeRandomizer>();
        if (randomizer != null && randomizer.customerType != null)
            SetSprites(randomizer.customerType.sprite, randomizer.customerType.blinkSprite);
    }

    private void Update()
    {
        if (paused) return;
        if (closedSprite == null || spriteRenderer == null) return;

        if (!isBlinking)
        {
            if (Time.time >= nextBlinkTime)
            {
                spriteRenderer.sprite = closedSprite;
                isBlinking = true;
                blinkEndTime = Time.time + blinkDuration;
            }
        }
        else if (Time.time >= blinkEndTime)
        {
            spriteRenderer.sprite = openSprite;
            isBlinking = false;
            nextBlinkTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    // Se para mientras hay otro sprite (comer/emocion)
    public void SetPaused(bool value)
    {
        paused = value;

        if (value)
        {
            // Si estaba a medias, lo damos por terminado sin escribir sprite
            isBlinking = false;
        }
        else
        {
            // Al reanudar, espera un intervalo completo antes del próximo parpadeo
            nextBlinkTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }


    // Establece los sprites
    public void SetSprites(Sprite open, Sprite closed)
    {
        openSprite = open;
        closedSprite = closed;
        if (spriteRenderer != null && open != null && !paused) {spriteRenderer.sprite = open; }
    }
}
