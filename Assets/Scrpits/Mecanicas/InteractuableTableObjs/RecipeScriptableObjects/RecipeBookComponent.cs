using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// Esta clase representa el libro de recetas entero
[CreateAssetMenu(fileName = "RecipeBookComponent", menuName = "Scriptable Objects/RecipeBookComponent")]
public class RecipeBookComponent : ScriptableObject
{
    // Lista con todas las recetas
    public List<RecipeComponent> recipes = new List<RecipeComponent>();

    // MÉTODOS
    // Este método comprueba para cada receta si los ingredientes seleccionados coinciden
    public RecipeComponent FindRecipe(List<IngredientComponent> ingredients)
    {
        foreach (var recipe in recipes) {       // Recorre todas las recetas de la lista
            if (recipe.Matches(ingredients)) return recipe;     // Si todos los ingredientes coinciden, devuelve la receta
        }
        return null;        // Si no, no devuelve nada
    }
}
