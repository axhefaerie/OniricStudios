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

    public CustomerType customerType;
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
        if (customerType == null)
        {
            Debug.LogWarning("Cliente sin CustomerType");
            return;
        }

        if (customerType.preferences == null || customerType.preferences.Count == 0)
        {
            Debug.LogWarning("Cliente sin preferencias");
            return;
        }

        // Elegir UNA preferencia al azar (normal y especial usan lo mismo)
        Preference chosen = customerType.preferences[Random.Range(0, customerType.preferences.Count)];
        requestedRecipe = chosen.preferenceRecipe;

        // Lo único que cambia: el texto que muestra
        if (customerType.isSpecial)
            recipeText.text = chosen.preferenceText;              // "Me apetece algo dulce"
        else
            recipeText.text = "Quiero " + requestedRecipe.name;   // "Quiero Galletas"

        if (balloon != null) balloon.SetActive(true);
        orderCompleted = false;
    }

    // Comprueba la receta entregada
    public bool CheckRecipe(RecipeComponent deliveredRecipe)
    {
        if (orderCompleted)
            return false;

        if (deliveredRecipe == requestedRecipe)
        {
            if (recipeText != null)
                recipeText.text = customerType.GetRandomLine(customerType.correctLines);

            orderCompleted = true;
            return true;
        }
        if (recipeText != null)
            recipeText.text = customerType.GetRandomLine(customerType.wrongLines);

        return false;
    }
}
