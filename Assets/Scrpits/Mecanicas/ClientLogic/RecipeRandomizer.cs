using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using static RecipeBookComponent;



//LA idea de esto es que el cliente pida una receta aleatoria de la lista de recetas disponibles, y
//luego el jugador tenga que entregar esa receta al cliente. 
//Tambien que en el tesxtbox de al lado salga el nombre de la receta que el cliente pide,
//y que si el jugador entrega la receta correcta, el cliente se vaya feliz, y si no, se vaya enfadao.


public class RecipeRandomizer : MonoBehaviour
{
    
    public RecipeBookComponent recipeBook;
    public GameObject balloon;
    public TextMeshProUGUI recipeText; //Esto será para en el futuro configurar dialogos y esas cosas(?
    
    //public SpriteRenderer clientSprite;
    //public Sprite happySprite;
    //public Sprite angrySprite;

    // Receta que pide el cliente actualmente
    public RecipeComponent requestedRecipe;

    private bool orderCompleted = false;

    private void Start()
    {
        GenerateRandomRecipe();
    }

    // Escoge una receta aleatoria del libro de recetas
    public void GenerateRandomRecipe()
    {
        if (recipeBook == null || recipeBook.recipes.Count == 0)
        {
            Debug.LogWarning("No hay recetas disponibles en el Recipe Book.");
            return;
        }

        int randomIndex = Random.Range(0, recipeBook.recipes.Count);

        requestedRecipe = recipeBook.recipes[randomIndex];

        if (requestedRecipe != null && recipeText != null) {
            recipeText.text = "Quiero " + requestedRecipe.name;
        }
        if (balloon != null)
        {
            balloon.SetActive(true);
        }

        // Mostrar el nombre de la receta solicitada
        Debug.Log("Quiero " + requestedRecipe.name); 

        orderCompleted = false;
    }

    // Comprueba la receta entregada
    public bool CheckRecipe(RecipeComponent deliveredRecipe)
    {
        if (orderCompleted)
            return false;

        if (deliveredRecipe == requestedRecipe)
        {
            // Pedido correcto
            Debug.Log("Cliente contento :D");
            //clientSprite.sprite = happySprite;
            // Pedido correcto
            if (recipeText != null)
            {
                recipeText.text = "muchas gracias... es justo lo que quería...";
            }

            orderCompleted = true;
            return true;
        }
        else
        {
            // Pedido incorrecto
            Debug.Log("Cliente enfadao >:(");
            // clientSprite.sprite = angrySprite;
            // Pedido incorrecto
            if (recipeText != null)
            {
                recipeText.text =
                    "¡Este no es mi pedido! yo no quería eso... Prueba otra vez...";
            }

            return false;
        }
    }
}



