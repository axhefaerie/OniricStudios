using System.Collections.Generic;
using UnityEngine;

// Esta clase permite que se guarde entre escenas el cliente y el plato (para que al cambiar entre preparación y horno sea lo mismo)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // El cliente actual (se guarda y no se regenera)
    public RecipeComponent requestedRecipe;      // la receta que pide el cliente
    public Sprite customerSprite;                // el aspecto del cliente
    public bool hasCustomer = false;             // si hay un cliente esperando
    public float remainingTime;

    // El plato: ingredientes que se van juntando
    public List<IngredientComponent> plateIngredients = new List<IngredientComponent>();
    public RecipeComponent bakedRecipe;          // la receta ya horneada (null si aún no)
    public bool isBaked = false;                 // si el plato ya pasó por el horno

    public CustomerType customerType;
    public string recipeText;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Guarda el estado del plato desde el PlateComponent
    public void SavePlate(List<IngredientComponent> ingredients, bool baked, RecipeComponent recipe)
    {
        plateIngredients = new List<IngredientComponent>(ingredients);
        isBaked = baked;
        bakedRecipe = recipe;
    }
}

