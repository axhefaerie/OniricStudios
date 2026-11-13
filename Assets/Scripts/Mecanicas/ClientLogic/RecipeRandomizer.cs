using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    // Flores que pueden pedir los clientes
    public List<IngredientComponent> availableFlowers = new List<IngredientComponent>();

    // PEDIDO ACTUAL:
    public RecipeComponent requestedRecipe;
    public IngredientComponent requestedFlower;

    private bool orderCompleted = false;

    private string orderText;       // Guarda el texto de pedido

    private void Start()
    {
        if (GameManager.Instance != null &&
        GameManager.Instance.requestedRecipe != null &&
        GameManager.Instance.requestedFlower != null)
        {
            // Carga los datos del pedido entre escenas para que no se pierdan
            requestedRecipe = GameManager.Instance.requestedRecipe;
            requestedFlower = GameManager.Instance.requestedFlower;
            orderText = GameManager.Instance.orderText;
            DisplayOrder();
        }
        else
        {
            // Si es un cliente nuevo, genera una receta aleatoria
            GenerateRandomRecipe();
        }
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

        if (availableFlowers == null || availableFlowers.Count == 0)
        {
            Debug.LogWarning("No hay flores disponibles para los pedidos");
            return;
        }

        // Elegir UNA preferencia al azar (normal y especial usan lo mismo)
        Preference chosen = customerType.preferences[Random.Range(0, customerType.preferences.Count)];
        requestedRecipe = chosen.preferenceRecipe;

        // Elegir la flor atribuida a cada cliente
        if (customerType.assignedFlower != null)
        {
            requestedFlower = customerType.assignedFlower;
        }
        else
        {
            requestedFlower = null;   // este cliente no pide flor (p. ej. Santiago)
        }

        // Construir el texto del pedido
        orderText = chosen.preferenceText + requestedRecipe.name;

        if (requestedFlower != null)
            orderText += " y " + requestedFlower.name;

        // Se guarda la selección en el GameManager para que persista entre escenas
        if (GameManager.Instance != null)
        {
            GameManager.Instance.requestedRecipe = requestedRecipe;
            GameManager.Instance.requestedFlower = requestedFlower;
            GameManager.Instance.orderText = orderText;
        }

        // Lo único que cambia: el texto que muestra
        // Mostrar el pedido
        DisplayOrder();

        orderCompleted = false;
    }

    private void DisplayOrder()
    {
        if (recipeText != null)
            recipeText.text = orderText;
        if (balloon != null) balloon.SetActive(true);
    }

    // Comprueba la receta entregada
    public bool CheckRecipe(RecipeComponent deliveredRecipe, IngredientComponent deliveredFlower)
    {
        if (orderCompleted)
            return false;

        bool correctRecipe = deliveredRecipe.name == requestedRecipe.name;
        bool correctFlower;

        // Se comprueba si hay flor (hay clientes que no tienen flor atribuida)
        if (requestedFlower == null)
            correctFlower = deliveredFlower == null;      
        else
            correctFlower = deliveredFlower != null && deliveredFlower.name == requestedFlower.name;


        if (correctRecipe && correctFlower)
        {
            if (recipeText != null)
                recipeText.text = customerType.GetRandomLine(customerType.correctLines);

            orderCompleted = true;
            return true;
        }
        // Equivocado: muestra "mal" y luego vuelve al pedido
        if (recipeText != null)
        {
            recipeText.text = customerType.GetRandomLine(customerType.wrongLines);
            StartCoroutine(RestoreOrderText());
        }

        return false;
    }

    public void OnWrongPlate()
    {
        if (recipeText != null)
        {
            recipeText.text = customerType.GetRandomLine(customerType.wrongLines);
            StartCoroutine(RestoreOrderText());
        }
    }

    public void ShowWrongTooMany()
    {
        StopAllCoroutines();   // cancela RestoreOrderText

        if (recipeText != null && customerType != null &&
            customerType.wrongDeliveryLines != null &&
            customerType.wrongDeliveryLines.Count > 0)
        {
            recipeText.text = customerType.wrongDeliveryLines[Random.Range(0, customerType.wrongDeliveryLines.Count)];
        }
        else if (recipeText != null)
        {
            recipeText.text = "Que tardón... Me piro";
        }
    }

    // Indica si el cliente ha pedido flor
    public bool WantsFlower()
    {
        return requestedFlower != null;
    }

    public void CancelRestore()
    {
        StopAllCoroutines();   // cancela RestoreOrderText
    }

    private IEnumerator RestoreOrderText()
    {
        yield return new WaitForSeconds(2f);   // 2 segundos mostrando "mal"
        if (!orderCompleted && recipeText != null)
            recipeText.text = orderText;
    }
}
