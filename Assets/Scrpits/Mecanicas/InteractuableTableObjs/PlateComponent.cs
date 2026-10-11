using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static IngredientComponent;
using static ClientComponent;

// Esta clase maneja la lógica del plato donde se van añadiendo los ingredientes y tal. 
// Maneja la lógica de comprobar que los ingredientes sean una receta (para cambiar su sprite)
// Tirar la comida y que se restaure el sprite del plato y la lista de ingredientes
// Dar el plato al cliente
// Añadir un ingrediente
public class PlateComponent : MonoBehaviour
{
    public RecipeBookComponent recipeBook;      // Libro de recetas
    public Transform ingredientParent;          // Lugar donde van a colocarse los sprites de los ingredientes

    public List<IngredientComponent> ingredients = new List<IngredientComponent>();     // Ingredientes del plato
    private RecipeComponent currentRecipe;          // Receta actual
    private ClientComponent client;                // Cliente al que se le va a entregar el plato

    // Añade un ingrediente al plato (se llama desde DragAndDrop)
    public bool AddIngredient(IngredientComponent ingredient)
    {
        //No permite añadir más de una base
        if (ingredient.type == IngredientType.Base || ingredient.type == IngredientType.Flower)
        {
            if (ingredients.Exists(i => i.type == ingredient.type))//Si ya existe ese tipo de ingrediente, devuelve false y no deja  añadirlo al platp
                return false;
        }

        ingredients.Add(ingredient);// Se añade el ingrediente al plato
        RebuildPlate();// Se actualiza visualmente el plato
        return true; // Se devuelve true si se ha añadido
    }

    // Este método permite actualizar visualmente el plato al añadir un ingrediente
    private void RebuildPlate()
    {
        // Primero, comprueba si los ingredientes del plato actual forman una receta llamando al método FindRecipe
        currentRecipe = recipeBook.FindRecipe(ingredients);

        // CAMBIO: eliminamos solamente los sprites anteriores
        // Antes llamábamos a ClearPlate(), pero ese método también
        // vaciaba la lista ingredientes y se perdía la receta o algo asi
        // entonces ahora se guardan los ingredientes para darselos despues ak cliente
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)// borramos las IMAGENES de los ingredientes, son borrar los ingredientes de la lista, osea borramos lo visual solo
        {
            Destroy(ingredientParent.GetChild(i).gameObject);
        }
        if (currentRecipe != null) {        // Si hay correspondencia (se ha formado un plato con los ingredientes de una receta)
                      // Se vacían los sprites de los ingredientes
            ShowSprite(currentRecipe.recipeSprite);     // Se llama al método ShowSprite para que aparezca el sprite de la receta final
        }
        else            // Si no hay una receta terminada con los ingredientes del plato, se muestran los ingreientes sin más
        {
            foreach (var ing in ingredients)        // Se recorre toda la lista de ingredientes
                ShowSprite(ing.sprite);         // Se llama al método para que los renderice
        }
    }

    // Añade el sprite del ingrediente en el plato
    private void ShowSprite (Sprite sprite)
    {
        // Primero, crea un gameObject vacío para añadirle el sprite correspondiente
        GameObject go = new GameObject("sprite");               // Instancia un objeto con el sprite del ingrediente dentro del plato
        go.transform.SetParent(ingredientParent, false);        // Lo crea en la posición del plato
        var sr = go.AddComponent<SpriteRenderer>();             // Le añade al nuevo GameObject un SpriteRenderer
        sr.sprite = sprite;                             // Le asigna el sprite del ingrediente/receta
        sr.sortingOrder = 10;                           // Para que aparezca siempre por encima del sprite del plato

        // Segundo, lo ajustar visualmente
        go.transform.localPosition = new Vector3(0, 0.4f, -ingredients.Count * 0.1f);       // Lo ajusta un poco hacia arriba del plato
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);        // Ajusta el tamaño
    }

    // Método que se llama al entregar el plato y qu7e comprueba si la receta está bien o mal con la que pide el cliente
// Entrega el plato al cliente
    
    public void Delivery(ClientComponent client)
    {
        if (client == null)
        {
            Debug.Log("No se ha encontrado el cliente");
            return;
        }

        // Buscamos la receta que forman los ingredientes
        currentRecipe = recipeBook.FindRecipe(ingredients);

        if (currentRecipe == null)
        {
            Debug.LogWarning("No se ha encontrado una receta para estos ingredientes.");

            return;
        }


        bool correct = client.ReceivePlate(currentRecipe);

        if (correct)
        {
            ClearPlate();
            currentRecipe = null;
        }
        else
        {
            ClearPlate();
            currentRecipe = null;
        }
    }

    // Método que descarta el plato al tirarlo a la basura
    public void ThrowAway()
    {
        ClearPlate();// Vacía los ingredientes y elimina sus imágenes
    }

    // Método que resetea el plato
    private void ClearPlate()
    {
        ingredients.Clear();    // Se vacía la lista

        // Eliminar las imágenes de los ingredientes
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
        {
            Destroy(ingredientParent.GetChild(i).gameObject);
        }

        // Reiniciar la receta actual
        currentRecipe = null;
    }
}
