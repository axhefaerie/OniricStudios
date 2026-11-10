using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

// Esta clase representa UNA receta del libro de recetas
[CreateAssetMenu(fileName = "RecipeComponent", menuName = "Scriptable Objects/RecipeComponent")]
public class RecipeComponent : ScriptableObject
{
    public string name;              // Nombre de la receta
    public Sprite recipeSprite;      // Sprite de la receta final

    // Ingredientes que forman una receta
    // Una receta está compuesta por: 1 base, X salsas, X toppings y 1 flor.
    public IngredientComponent baseIngredient;
    // Como el número de salsas (o toppings) no es fijo, se usa una lista
    public List<IngredientComponent> sauceIngredients = new List<IngredientComponent>();
    public List<IngredientComponent> toppingIngredients = new List<IngredientComponent> ();
    public IngredientComponent flowerIngredient;

    // MÉTODOS
    // Este método comprueba si una lista de ingredientes coincide con esta receta
    public bool Matches(List<IngredientComponent> list)
    {
        // Primero, se agrupa el listado de ingredientes proveniente por tipo
        // Se crean distintas listas que irán guardando los ingredientes provenientes de cada tipo en su lista respectiva para poder comparar después
        var bases = new List<IngredientComponent> ();
        var sauces = new List<IngredientComponent> ();
        var toppings = new List<IngredientComponent> ();
        var flowers = new List<IngredientComponent> ();

        // Se van separando los ingredientes según su tipo
        foreach (var ingredient in list) {
            switch (ingredient.type) {
                case IngredientComponent.IngredientType.Base: bases.Add(ingredient); break;
                case IngredientComponent.IngredientType.Sauce: sauces.Add(ingredient); break;
                case IngredientComponent.IngredientType.Topping: toppings.Add(ingredient); break;
                case IngredientComponent.IngredientType.Flower: flowers.Add(ingredient); break;
            }
        }

        // Se comprueba que la receta tiene exactamente 1 base y 1 flor Y se comprueba que la base y la flor sean la misma que la de la receta
        if (bases.Count != 1 || flowers.Count != 1 || bases[0] != baseIngredient || flowers[0] != flowerIngredient) { return false; }

        return SameSet(sauces, this.sauceIngredients) && SameSet(toppings, this.toppingIngredients);
    }

    public bool SameSet(List<IngredientComponent> a, List<IngredientComponent> b)
    {
        // Se comprueba que tenga el mismo número de salsas y toppings
        if (a.Count != b.Count) return false;

        // Si tienen el mismo número, se va comprobando para cada item si está dentro de los ítems de la receta
        foreach(var item in a) {
             if(!b.Contains(item)) return false;    // Si un item NO está dentro de los ingredientes de la receta, devuelve false
        }
        // Si tiene el mismo número de ingredientes, y son el mismo, devuelve true
        return true;
    }
}
