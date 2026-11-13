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

    public Sprite susSprite;   // sprite de "receta pocha" / quemada
    public Sprite burntSprite; // sprite de cenizas

    public List<IngredientComponent> ingredients = new List<IngredientComponent>();     // Ingredientes del plato
    private RecipeComponent currentRecipe;          // Receta actual
    private ClientComponent client;                // Cliente al que se le va a entregar el plato


    private bool hasBeenBaked = false;//para saber si un plato ha sido cocinado ya
    public bool HasBeenBaked
    {
        get { return hasBeenBaked; }
    }


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            ingredients = new List<IngredientComponent>(GameManager.Instance.plateIngredients);
            if (GameManager.Instance.isBaked)
            {
                if (GameManager.Instance.bakedSprite != null)
                    ShowSprite(GameManager.Instance.bakedSprite);
                else if (GameManager.Instance.bakedRecipe != null)
                    ShowSprite(GameManager.Instance.bakedRecipe.recipeSprite);
                else
                    ShowSprite(burntSprite);
            }
            else
            {
                RebuildPlate();   // mostrar ingredientes apilados
            }
        }
    }

    // Añade un ingrediente al plato (se llama desde DragAndDrop)
    public bool AddIngredient(IngredientComponent ingredient)
    {
        // No permite añadir más de una base ni una flor
        if (ingredient.type == IngredientType.Base || ingredient.type == IngredientType.Flower)
        {
            if (ingredients.Exists(i => i.type == ingredient.type))//Si ya existe ese tipo de ingrediente, devuelve false y no deja  añadirlo al platp
                return false;
        }

        ingredients.Add(ingredient);// Se añade el ingrediente al plato
        RebuildPlate();// Se actualiza visualmente el plato

        GameManager.Instance.SavePlate(ingredients, false, null);   // Guarda en el Manager lo que se ha añadido por si se cambia de escena

        return true; // Se devuelve true si se ha añadido
    }

    // Este método permite actualizar visualmente el plato al añadir un ingrediente
    private void RebuildPlate()
    {
        // YA NO COMPROBAMOS RECETA
        // Primero, comprueba si los ingredientes del plato actual forman una receta llamando al método FindRecipe
        //currentRecipe = recipeBook.FindRecipe(ingredients);

        // CAMBIO: eliminamos solamente los sprites anteriores
        // Antes llamábamos a ClearPlate(), pero ese método también
        // vaciaba la lista ingredientes y se perdía la receta o algo asi
        // entonces ahora se guardan los ingredientes para darselos despues ak cliente
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)// borramos las IMAGENES de los ingredientes, son borrar los ingredientes de la lista, osea borramos lo visual solo
        {
            Destroy(ingredientParent.GetChild(i).gameObject);
        }
             
        foreach (var ing in ingredients) // Se recorre toda la lista de ingredientes
        {
            Sprite spriteToShow = ing.sprite;

            if (ing.type == IngredientComponent.IngredientType.Sauce &&
                ing.plateSprite != null)
            {
                spriteToShow = ing.plateSprite;
            }

            ShowSprite(spriteToShow); // Se llama al método para que los renderice
        }
    }

    // Este método se llama al hornear el plato para cambiar el sprite y que salga la recepta que se haya seguido
    public bool Bake()
    {
        // Se busca si los ingredientes actuales forman una receta
        currentRecipe = recipeBook.FindRecipe(ingredients);

        if (currentRecipe == null)
        {
            hasBeenBaked = true;
            GameManager.Instance.isBurnt = false;
            Debug.Log("No se puede hornear: ingredientes no forman una receta");
            // Borrar ingredientes y mostrar el sprite "pocho"
            for (int i = ingredientParent.childCount - 1; i >= 0; i--)
                Destroy(ingredientParent.GetChild(i).gameObject);

            ShowSprite(susSprite, 0.3f);
            GameManager.Instance.bakedSprite = susSprite;
            GameManager.Instance.SavePlate(ingredients, true, null);

            return false;
        }
        GameManager.Instance.isBurnt = false;
        // Borrar sprites de ingredientes
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
            Destroy(ingredientParent.GetChild(i).gameObject);

        ShowSprite(currentRecipe.recipeSprite, 0.3f);
        GameManager.Instance.bakedSprite = currentRecipe.recipeSprite;
        GameManager.Instance.SavePlate(ingredients, true, currentRecipe);

        return true;
    }

    // Añade el sprite del ingrediente en el plato
    private void ShowSprite (Sprite sprite, float scale = 0.9f)
    {
        // Primero, crea un gameObject vacío para añadirle el sprite correspondiente
        GameObject go = new GameObject("sprite");               // Instancia un objeto con el sprite del ingrediente dentro del plato
        go.transform.SetParent(ingredientParent, false);        // Lo crea en la posición del plato
        var sr = go.AddComponent<SpriteRenderer>();             // Le añade al nuevo GameObject un SpriteRenderer
        sr.sprite = sprite;                             // Le asigna el sprite del ingrediente/receta
        sr.sortingOrder = 10;                           // Para que aparezca siempre por encima del sprite del plato

        // Segundo, lo ajustar visualmente
        go.transform.localPosition = new Vector3(0, 0.4f, -ingredients.Count * 0.1f);       // Lo ajusta un poco hacia arriba del plato
        go.transform.localScale = new Vector3(scale, scale, scale);        // Ajusta el tamaño
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

        if (GameManager.Instance != null && GameManager.Instance.isBurnt)
        { 
            if (client.spawner != null) client.spawner.OnCustomerWrongDelivery();
            client.recipeRandomizer.OnWrongPlate();   // muestra "mal" y restaura el pedido
            ClearPlateAndGameManager();               // limpia el plato y el estado guardado
            return;
        }

        // Buscamos la receta que forman los ingredientes
        currentRecipe = recipeBook.FindRecipe(ingredients);

        if (currentRecipe == null)
        {
            Debug.LogWarning("No se ha encontrado una receta para estos ingredientes.");
            if (client.spawner != null) client.spawner.OnCustomerWrongDelivery();
            client.recipeRandomizer.OnWrongPlate();   // muestra "mal" y restaura el pedido
            ClearPlateAndGameManager();               // limpia el plato y el estado guardado
            return;
        }

        // Buscar la flor añadida al plato
        IngredientComponent currentFlower = ingredients.Find(i => i.type == IngredientType.Flower);

        if (currentFlower == null)
        {
            Debug.LogWarning("El plato no tiene ninguna flor.");
            if (client.spawner != null) client.spawner.OnCustomerWrongDelivery();
            client.recipeRandomizer.OnWrongPlate();   // también se trata como fallo
            ClearPlateAndGameManager();
            return;
        }

        // Comprobar receta y flor
        bool correct = client.ReceivePlate(currentRecipe, currentFlower);

        // Se limpia el estado del plato para que se traspase entre escenas
        ClearPlateAndGameManager();
        currentRecipe = null;
    }

    // Método que descarta el plato al tirarlo a la basura
    public void ThrowAway()
    {
        ClearPlate();
        GameManager.Instance.plateIngredients.Clear();
        GameManager.Instance.isBaked = false;
        GameManager.Instance.bakedRecipe = null;
        GameManager.Instance.bakedSprite = null;
    }

    // Metodo para limpiar el plato y el GameManager
    private void ClearPlateAndGameManager()
    {
        ClearPlate();                                    // vacía la lista local y sprites
        GameManager.Instance.plateIngredients.Clear();
        GameManager.Instance.isBaked = false;
        GameManager.Instance.bakedRecipe = null;
        GameManager.Instance.bakedSprite = null;
        GameManager.Instance.isBurnt = false;            // si usas el booleano
        currentRecipe = null;
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

    public bool HasValidRecipe()//comprobar que la receta es valida
    {
        return recipeBook.FindRecipe(ingredients) != null;
    }

    // Este método se llama cuando se falla en el minijuego, sale un sprite con cenizas en el plato
    public void Burn()
    {
        hasBeenBaked = true;
        GameManager.Instance.isBurnt = true;

        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
            Destroy(ingredientParent.GetChild(i).gameObject);

        ShowSprite(burntSprite, 0.3f);
        GameManager.Instance.bakedSprite = burntSprite;
        GameManager.Instance.SavePlate(ingredients, true, null);

    }
}
