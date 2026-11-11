using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBook : MonoBehaviour
{
    public RecipeBookComponent recipeBook; // Referencia al libro de recetas
    public TMP_Text recipeName; // NOmbre de la receta
    public Image recipeImage; // Imagen receta
    public Transform ingredientsContainer; // Contenedor donde va a mostrarse
    public GameObject ingredientPrefab; // Prefab del nombre y el sprite de cada ingrediente

    private int currentPage = 0;

    void Start()
    {
        ShowRecipe();
    }

    public void NextPage() // Para pasar a la siguiente pag
    {
        if (currentPage < recipeBook.recipes.Count - 1)
        {
            currentPage++;
            ShowRecipe();
        }
    }

    public void PreviousPage() // Para ir a la anterior pag
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowRecipe();
        }
    }

    void ShowRecipe()
    {
        RecipeComponent recipe = recipeBook.recipes[currentPage];

        recipeName.text = recipe.name;
        recipeImage.sprite = recipe.recipeSprite;

        foreach (Transform child in ingredientsContainer)
        {
            Destroy(child.gameObject);
        }

        AddIngredient(recipe.baseIngredient);

        foreach (IngredientComponent ingredient in recipe.sauceIngredients)
        {
            AddIngredient(ingredient);
        }

        foreach (IngredientComponent ingredient in recipe.toppingIngredients)
        {
            AddIngredient(ingredient);
        }

        AddIngredient(recipe.flowerIngredient);
    }

    void AddIngredient(IngredientComponent ingredient)
    {
        if (ingredient == null) return;

        GameObject newIngredient = Instantiate(
            ingredientPrefab,
            ingredientsContainer
        );

        newIngredient.GetComponent<IngredientEntry>()
            .SetIngredient(ingredient);
    }
}
