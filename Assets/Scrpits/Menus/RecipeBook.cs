using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RecipeBook : MonoBehaviour
{
    public RecipeBookComponent recipeBook; // Referencia al libro de recetas
    public TMP_Text recipeName; // NOmbre de la receta
    public Image recipeImage; // Imagen receta
    public GameObject recipeContent;
    public Transform ingredientsContainer; // Contenedor donde va a mostrarse
    public GameObject ingredientPrefab; // Prefab del nombre y el sprite de cada ingrediente
    public Animator bookAnimator; // Animator del libro

    private int currentPage = 0;
    private bool bookOpened = false;
    public float animationDelay = 0.5f; // Tiempo que tarda el texto en mostrarse tras abrirse el libro

    void Start()
    {
        ShowRecipe();
    }

    public void NextPage() // Para pasar a la siguiente pag
    {
        if (currentPage < recipeBook.recipes.Count - 1)
        {
            currentPage++;

            if (!bookOpened) // Animacion de abrir libro
            {
                bookAnimator.Play("AbrirLibro", 0, 0f);
                bookOpened = true;
                StartCoroutine(ShowRecipeAfterDelay());
            }
            else
            {
                ShowRecipe();
            }
        }
    }

    // Corrutina para mostrar el texto
    IEnumerator ShowRecipeAfterDelay()
    {
        yield return new WaitForSeconds(animationDelay);
        ShowRecipe();
    }

    public void PreviousPage() // Para ir a la anterior pag
    {
        if (currentPage > 0)
        {
            currentPage--;

            if (currentPage == 0) // Animacion cerrar libro
            {
                bookAnimator.Play("CerrarLibro", 0, 0f);
                bookOpened = false;
            }

            ShowRecipe();
        }
    }

    void ShowRecipe()
    {
        recipeContent.SetActive(currentPage > 0);

        if (currentPage == 0) return;

        RecipeComponent recipe = recipeBook.recipes[currentPage - 1];

        recipeName.text = recipe.name;
        recipeImage.sprite = recipe.bookSprite;

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

        //AddIngredient(recipe.flowerIngredient);
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
