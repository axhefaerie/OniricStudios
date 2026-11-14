using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RecipeBook : MonoBehaviour
{
    public RecipeBookComponent recipeBook; // Referencia al libro de recetas
    public Image recipeName; // NOmbre de la receta
    public Image recipeImage; // Imagen receta
    public TMP_Text recipeDescription; // Descripcion receta
    public Image recipeIngredientsImage;

    public GameObject recipeContent;
    //public Transform ingredientsContainer; // Contenedor donde va a mostrarse
    //public GameObject ingredientPrefab; // Prefab del nombre y el sprite de cada ingrediente
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
        if (currentPage < recipeBook.recipes.Count)
        {
            currentPage++;

            if (!bookOpened) // Animacion de abrir libro
            {             
                bookOpened = true;
                bookAnimator.Play("AbrirLibro", 0, 0f);
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

    public void ResetBook()
    {
        currentPage = 0;
        bookOpened = false;

        StopAllCoroutines();

        // Ocultar el contenido
        recipeContent.SetActive(false);

        // Borrar los ingredientes de la última receta
        /*
        foreach (Transform child in ingredientsContainer)
        {
            Destroy(child.gameObject);
        }
        */

        // Borrar el texto y la imagen anteriores
        recipeName.sprite = null;
        recipeImage.sprite = null;
        recipeDescription.text = "";

        recipeIngredientsImage.sprite = null;

        // Reiniciar completamente la animación
        bookAnimator.Rebind();
        bookAnimator.Update(0f);
        bookAnimator.Play("Cerrado", 0, 0f);
        bookAnimator.Update(0f);
    }

    void ShowRecipe()
    {
        recipeContent.SetActive(currentPage > 0);

        if (currentPage == 0) return;

        RecipeComponent recipe = recipeBook.recipes[currentPage - 1];

        recipeName.sprite = recipe.recipeName;
        recipeImage.sprite = recipe.bookSprite;
        recipeDescription.text = recipe.recipeCharacteristics;

        recipeIngredientsImage.sprite = recipe.ingredientsSprite;
        recipeIngredientsImage.preserveAspect = true;

        /*
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
        */
        //AddIngredient(recipe.flowerIngredient);
    }

    /*
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
    */
}
