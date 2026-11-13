using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static RecipeComponent;
using static RecipeRandomizer;

// EL CLIENTE PIDE UN PLATO Y UNA FLOR :)

public class ClientComponent : MonoBehaviour
{
    public RecipeRandomizer recipeRandomizer;
    public CustomerSpawner spawner;

    private void Start()
    {
        if (spawner == null)
        {
            spawner = FindObjectOfType<CustomerSpawner>();
        }
    }
    public bool ReceivePlate(RecipeComponent deliveredRecipe, IngredientComponent deliveredFlower)
    {
        // Comprobamos que existe una receta entregada
        if (deliveredRecipe == null)
        {
            Debug.Log("El plato no tiene ninguna receta");
            
            return false;
        }

        // Comprobamos que hay una flor SOLO si el cliente la pide
        if (deliveredFlower == null && recipeRandomizer != null && recipeRandomizer.WantsFlower())
        {
            Debug.Log("El plato no tiene ninguna flor");
            
            return false;
        }

        // Comprobamos que existe una receta pedida
        if (recipeRandomizer == null ||
            recipeRandomizer.requestedRecipe == null)
        {
            Debug.LogWarning("El cliente no tiene ninguna receta asignada.");
            return false;
        }

        // Comprueba el pedido y cambia el texto del bocadillo
        bool correctOrder = recipeRandomizer.CheckRecipe(deliveredRecipe, deliveredFlower);

        if (correctOrder)
        {
            Debug.Log("Cliente contento :D");

            if (spawner != null)
            {
                spawner.OnCustomerServed();
            }

            return true;
        }
        else
        {
            Debug.Log("Cliente enfadado >:(");
            return false;
        }
    }
}

