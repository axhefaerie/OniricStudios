using UnityEngine;

// EL CLIENTE PIDE UN PLATO Y UNA FLOR :)
public class ClientComponent : MonoBehaviour
{
    public RecipeRandomizer recipeRandomizer;
    public CustomerSpawner spawner;

    public SpriteRenderer sr;
    private Sprite normalSprite;
    private ClientBlink blink;

    private void Awake()
    {
       // sr = GetComponent<SpriteRenderer>();
        if (sr != null) { normalSprite = sr.sprite; }
        blink = GetComponent<ClientBlink>();
    }

    private void Start()
    {
        if (spawner == null) { spawner = FindObjectOfType<CustomerSpawner>(); }
    }

    public bool ReceivePlate(RecipeComponent deliveredRecipe, IngredientComponent deliveredFlower)
    {
        // Comprobamos que existe una receta entregada
        if (deliveredRecipe == null)
            return false;
        
        // Comprobamos que hay una flor SOLO si el cliente la pide
        if (deliveredFlower == null && recipeRandomizer != null && recipeRandomizer.WantsFlower())
            return false;

        // Comprobamos que existe una receta pedida
        if (recipeRandomizer == null || recipeRandomizer.requestedRecipe == null)
            return false;

        // Comprueba el pedido y cambia el texto del bocadillo
        bool correctOrder = recipeRandomizer.CheckRecipe(deliveredRecipe, deliveredFlower);

        if (correctOrder)
        {
            if (spawner != null)
            {
                spawner.OnCustomerServed();
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetEating(bool eating)
    {
        if (sr == null) {return; }

        var type = recipeRandomizer != null ? recipeRandomizer.customerType : null;
        if (type == null) {return; }

        if (eating && type.eatSprite != null)
        {
            if (blink != null) {blink.SetPaused(true); }
            sr.sprite = type.eatSprite;
        }
        else
        {
            if (normalSprite != null) { sr.sprite = normalSprite; }
            if (blink != null) {blink.SetPaused(false); }
        }
    }

    public void SetNormalSprite(Sprite s)
    {
        if (s != null) normalSprite = s;
    }
}

