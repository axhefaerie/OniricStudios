using System.Collections.Generic;
using UnityEngine;

// Esta clase permite que se guarde entre escenas el cliente y el plato (para que al cambiar entre preparación y horno sea lo mismo)
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // El cliente actual (se guarda y no se regenera)
    public RecipeComponent requestedRecipe;      // la receta que pide el cliente
    public IngredientComponent requestedFlower; // la flor que pide el cliente
    public string orderText;

    // CLIENTE
    public Sprite customerSprite;                // el aspecto del cliente
    public bool hasCustomer = false;             // si hay un cliente esperando
    public CustomerType customerType;
    public string recipeText;

    // TEMPORIZADOR
    public float maxTime = 120f;
    public float remainingTime = 120f;
    public bool timerRunning = false;
    public bool customerTimedOut = false;

    // PLATO: ingredientes que se van juntando
    public List<IngredientComponent> plateIngredients = new List<IngredientComponent>();
    public RecipeComponent bakedRecipe;          // la receta ya horneada (null si aún no)
    public bool isBaked = false;                 // si el plato ya pasó por el horno
    public Sprite bakedSprite;                 // Guarda el sprite del plato formado en el horno
    public bool isBurnt = false;

    // CONTADOR CLIENTES NORMALES
    public int normalCount = 0;
    public int nextNormalsTarget = 0;

    // DESBLOQUEO CLIENTES PARA EL MÓVIL
    public List<string> unlockedCustomers = new List<string>(); // Lista que guarda los clientes que se van desbloqueando

    // CONTADOR ESTRELLAS
    public int totalStarsEarned = 0;   // estrellas conseguidas
    public int totalStarsPossible = 0; // estrellas máximas posibles de la partida (cada cliente atendido suma 3)
    public int specialCustomersServed = 0;   // especiales ya atendidos
    public int totalSpecialCustomers = 0;    // total de especiales en la partida

    // PANTALLA DE RESULTADOS FINAL
    public string gameResult;   // "victory" o "defeat"
    public int finalStars;
    public int maxStars;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //para que el temporizador funcione aunque se cambie de escena
    private void Update()
    {
        if (!timerRunning) return;
        if (remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            timerRunning = false;
            customerTimedOut = true;   // el CustomerSpawner lo detecta y echa al cliente
        }
    }

    // Guarda el estado del plato desde el PlateComponent
    public void SavePlate(List<IngredientComponent> ingredients, bool baked, RecipeComponent recipe)
    {
        plateIngredients = new List<IngredientComponent>(ingredients);
        isBaked = baked;
        bakedRecipe = recipe;
        // bakedSprite = finalSprite;
    }

    public void UnlockCustomer(CustomerType type)
    {
        if (type == null) return;
        if (!unlockedCustomers.Contains(type.customerName))
            unlockedCustomers.Add(type.customerName);
    }

    public bool IsSpecialCustomer(CustomerType type)
    {
        // Asume que CustomerType tiene un bool isSpecial
        return type != null && type.isSpecial;
    }

}

