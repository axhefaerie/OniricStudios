using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientComponent;
using static IngredientComponent;

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

    private Vector3 plateHomePos;
    public Transform counterPoint;      // punto de la encimera donde se queda el plato
    public float eatingDuration = 2f;

    private bool hasBeenBaked = false;//para saber si un plato ha sido cocinado ya
    public bool HasBeenBaked
    {
        get { return hasBeenBaked; }
    }
    private bool isBeingDelivered = false;
    public bool IsBeingDelivered => isBeingDelivered;

    private void Start()
    {
        plateHomePos = transform.position;

        if (GameManager.Instance != null)
        {
            ingredients = new List<IngredientComponent>(GameManager.Instance.plateIngredients);
            transform.localScale = Vector3.one;
            if (GameManager.Instance.isBaked)
            {
                hasBeenBaked = true;
                if (GameManager.Instance.bakedSprite != null)
                    ShowSprite(GameManager.Instance.bakedSprite);
                else if (GameManager.Instance.bakedRecipe != null)
                    ShowSprite(GameManager.Instance.bakedRecipe.recipeSprite);
                else
                    ShowSprite(burntSprite);

                HidePlateSprite();
            }
            else
            {
                hasBeenBaked = false;
                RebuildPlate();   // mostrar ingredientes apilados
            }
        }
    }

    // Añade un ingrediente al plato (se llama desde DragAndDrop)
    public bool AddIngredient(IngredientComponent ingredient)
    {
        // Si el plato está cocinado NO se pueden añadir más ingredientes
        if (hasBeenBaked) { return false; }
        if (GameManager.Instance != null && GameManager.Instance.isBaked) { return false; }

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
        // Si el plato está vacío no se puede cocinar
        if (ingredients.Count == 0) return false;

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

            ShowSprite(susSprite);

            GameManager.Instance.bakedSprite = susSprite;
            GameManager.Instance.SavePlate(ingredients, true, null);

            return false;
        }
        GameManager.Instance.isBurnt = false;
        // Borrar sprites de ingredientes
        for (int i = ingredientParent.childCount - 1; i >= 0; i--)
            Destroy(ingredientParent.GetChild(i).gameObject);

        ShowSprite(currentRecipe.recipeSprite);

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
        go.transform.localPosition = new Vector3(0, 0.2f, -ingredients.Count * 0.1f);       // Lo ajusta un poco hacia arriba del plato
                                                                                            // Escala base del plato, consistente en todas las escenas
        float parentScale = ingredientParent.lossyScale.x;
        float worldScale = scale / Mathf.Max(parentScale, 0.0001f);
        go.transform.localScale = new Vector3(worldScale, worldScale, worldScale);
    }

    // Método que se llama al entregar el plato y qu7e comprueba si la receta está bien o mal con la que pide el cliente
    // Entrega el plato al cliente y devuelve si ha sido correcto o incorrecto el pedido
    
    public bool Delivery(ClientComponent client)
    {
        if (client == null) { return false; }
        if (client.spawner != null && client.spawner.IsChangingCustomer) return false;
        if (GameManager.Instance != null && GameManager.Instance.isBurnt)
        {
            FailDelivery(client);
            ClearPlateAndGameManager();
            return false;
        }

        // Buscamos la receta que forman los ingredientes
        currentRecipe = recipeBook.FindRecipe(ingredients);
        if (currentRecipe == null)
        {
            FailDelivery(client);
            ClearPlateAndGameManager();
            return false;
        }

        // Buscar la flor añadida al plato
        IngredientComponent currentFlower = ingredients.Find(i => i.type == IngredientType.Flower);
        if (currentFlower == null && client.recipeRandomizer.WantsFlower())
        {
            FailDelivery(client);
            ClearPlateAndGameManager();
            return false;
        }

        // Comprobar receta y flor
        bool correct = client.ReceivePlate(currentRecipe, currentFlower);

        // Si el pedido es incorrecto, se tira directamente
        if (!correct)
        {
            FailDelivery(client);
            ClearPlateAndGameManager();
            currentRecipe = null;
            return false;
        }

        // Si el pedido es correcto, se mueve a la encimera
        MoveToCounter();        // Se mueve aa la encimera
        HidePlateSprite();          // Se oculta el sprite del plato (el plato vacio no el de la receta)
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        isBeingDelivered = true;
        StartCoroutine(HideAfterEating());      // Se empieza la corrutina de 'comer'
        return true;
    }

    private void FailDelivery(ClientComponent client)
    {
       // client.recipeRandomizer.OnWrongPlate();          // 1.º: frase de error + RestoreOrderText
        if (client.spawner != null) client.spawner.OnCustomerWrongDelivery();    // 2.º: si es el 3.º, pisa con la frase de despedida
    }

    // Método que descarta el plato al tirarlo a la basura 
    public void ThrowAway()
    {
        ClearPlate();
        GameManager.Instance.plateIngredients.Clear();
        GameManager.Instance.isBaked = false;
        GameManager.Instance.bakedRecipe = null;
        GameManager.Instance.bakedSprite = null;
        hasBeenBaked = false;
    }

    // Metodo para limpiar el plato y el GameManager (se vacía y se limpian TODAS las variables)
    private void ClearPlateAndGameManager()
    {
        ClearPlate();                                    // vacía la lista local y sprites
        GameManager.Instance.plateIngredients.Clear();
        GameManager.Instance.isBaked = false;
        GameManager.Instance.bakedRecipe = null;
        GameManager.Instance.bakedSprite = null;
        GameManager.Instance.isBurnt = false;            // si usas el booleano
        currentRecipe = null;
        hasBeenBaked = false;
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
        hasBeenBaked = false;
        RebuildPlate();
        ShowPlateSprite();   // el plato vuelve a aparecer al limpiarlo
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

        ShowSprite(burntSprite);

        // El plato quemado NO puede volver a cocinarse: vaciar ingredientes y marcar cocinado
        ingredients.Clear();
        GameManager.Instance.plateIngredients.Clear();
        GameManager.Instance.SavePlate(ingredients, true, null);
    }

    // Método que mueve el plato con la receta a la encimera para que el cliete se lo 'coma'
    public void MoveToCounter()
    {
        if (counterPoint == null) return;
        transform.position = counterPoint.position;
        transform.localScale = Vector3.one;
    }

    // Cuando termina de 'comer' desaparece el plato de la encimera
    private IEnumerator HideAfterEating()
    {
        yield return new WaitForSeconds(eatingDuration);

        // Se limpia el plato después de que se lo coma
        ClearPlateAndGameManager();
        transform.position = plateHomePos;
        ShowPlateSprite();  // Se vuelve a mostrar el plato en suposición original (vacio)

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        isBeingDelivered = false;
    }

    // Oculta el plato al cocinarlo para que solo salga la receta
    public void HidePlateSprite()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }

    // Vuelve a mostrar el sprite del plato
    public void ShowPlateSprite()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;
    }

}
