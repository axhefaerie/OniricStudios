
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;

    public List<CustomerType> normalCustomers;     // los normales disponibles
    public List<CustomerType> specialCustomers;    // los especiales disponibles

    public int minNormals = 2;    // mínimo de normales antes de un especial
    public int maxNormals = 4;    // máximo


    public float spawnDelay = 0.5f;
    public TextMeshProUGUI recipeText; //configurar dialogos y esas cosas(?

    public Slider timerBar;
    public float maxTime = 120f;        // 2 min

    private GameObject currentCustomer;     // Cliente actual

    // Variables del tiempo
    private float currentTime;
    private bool timerRunning = false;
    private bool changingCustomer = false;

    private bool spawning = false;      // Guarda si está cambiandose el cliente para uqe no se instancien varios a alvez

    public int maxWrongDeliveries = 3;   // intentos máximos fallidos
    private int wrongDeliveries = 0;     // fallos acumulados del cliente actual

    private void Start()
    {
        if (GameManager.Instance.nextNormalsTarget <= 0)
            GameManager.Instance.nextNormalsTarget = Random.Range(minNormals, maxNormals + 1);

        if (GameManager.Instance != null && GameManager.Instance.hasCustomer)
            SpawnSavedCustomer();
        else
            SpawnCustomer();
    }

    // Restaura el cliente que estaba previamente esperando
    // Restaura el cliente que ya estaba esperando al volver a la escena
    private void SpawnSavedCustomer()
    {
        if (currentCustomer != null)
            Destroy(currentCustomer);

        CustomerType type = GameManager.Instance.customerType;
        Vector3 pos = type != null ? type.spawnPosition : spawnPoint.position;

        currentCustomer = Instantiate(customerPrefab, pos, Quaternion.identity);

        // Restaurar el tipo de cliente guardado
        var randomizer = currentCustomer.GetComponent<RecipeRandomizer>();
        randomizer.customerType = GameManager.Instance.customerType;

        // Restaurar el sprite guardado
        currentCustomer.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.customerSprite;

        // Restaurar la receta pedida SIN generar una nueva
        randomizer.requestedRecipe = GameManager.Instance.requestedRecipe;

        // Restaurar el texto del globo (preferencia o nombre de receta)
        recipeText.text = GameManager.Instance.recipeText;

        // Restaurar el temporizador donde estaba
        currentTime = GameManager.Instance.remainingTime;
        timerBar.maxValue = maxTime;
        timerBar.value = currentTime;

        timerRunning = true;
        changingCustomer = false;
    }

    // Genera aleatoriamente un cliente
    public void SpawnCustomer()
    {
        if (spawning) return;
        spawning = true;

        wrongDeliveries = 0;

        if (currentCustomer != null)
            Destroy(currentCustomer);

        CustomerType type = PickCustomerType();
        if (type == null)
        {
            spawning = false;  
            return;
        }

        Vector3 pos = type.spawnPosition;
        currentCustomer = Instantiate(customerPrefab, pos, Quaternion.identity);

        // Sprite desde el CustomerType
        currentCustomer.GetComponent<SpriteRenderer>().sprite = type.sprite;

        // Asignar el tipo ANTES de generar la receta
        var randomizer = currentCustomer.GetComponent<RecipeRandomizer>();
        randomizer.customerType = type;
        randomizer.GenerateRandomRecipe();

        // Guardar estado para el cambio de escena
        GameManager.Instance.customerType = type;
        GameManager.Instance.customerSprite = type.sprite;
        GameManager.Instance.requestedRecipe = randomizer.requestedRecipe;
        GameManager.Instance.recipeText = recipeText.text;
        GameManager.Instance.hasCustomer = true;

        // Reiniciar temporizador
        currentTime = maxTime;
        timerBar.maxValue = maxTime;
        timerBar.value = maxTime;
        GameManager.Instance.remainingTime = currentTime;

        timerRunning = true;
        changingCustomer = false;
        spawning = false;
    }

    // Decide si toca normal o especial (2-4 normales → 1 especial)
    private CustomerType PickCustomerType()
    {
        bool canSpecial = specialCustomers != null && specialCustomers.Count > 0;
        bool canNormal = normalCustomers != null && normalCustomers.Count > 0;

        if (!canNormal && !canSpecial) return null;
        if (!canNormal) return specialCustomers[Random.Range(0, specialCustomers.Count)];
        if (!canSpecial) return normalCustomers[Random.Range(0, normalCustomers.Count)];

        var gm = GameManager.Instance;
        if (gm == null)
            return normalCustomers[Random.Range(0, normalCustomers.Count)];

        if (gm.normalCount >= gm.nextNormalsTarget)
        {
            gm.normalCount = 0;
            gm.nextNormalsTarget = Random.Range(minNormals, maxNormals + 1);
            return specialCustomers[Random.Range(0, specialCustomers.Count)];
        }

        gm.normalCount++;
        return normalCustomers[Random.Range(0, normalCustomers.Count)];
    }

    private void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;
            timerBar.value = currentTime;

            // Guardar el tiempo restante para que sobreviva al cambio de escena
            if (GameManager.Instance != null)
                GameManager.Instance.remainingTime = currentTime;

            if (currentTime <= 0)
            {
                timerRunning = false;
                OnCustomerTimeOut();
            }
        }
    }


    // Se llama cuando el cliente recibe su pedido
    public void OnCustomerServed()
    {
        if (changingCustomer) return;

        timerRunning = false;
        changingCustomer = true;

        // Liberar el cliente guardado
        GameManager.Instance.hasCustomer = false;

        var randomizer = currentCustomer.GetComponent<RecipeRandomizer>();
        GameManager.Instance.UnlockCustomer(randomizer.customerType);

        StartCoroutine(ChangeCustomerAfterDelay());
    }

    // Se llama cuando se entrega mal un pedido (si se llama 3 veces con elmismo cliente, se va)
    public void OnCustomerWrongDelivery()
    {
        if (changingCustomer) return;

        wrongDeliveries++;

        if (wrongDeliveries >= maxWrongDeliveries)
        {
            // Se acabaron los intentos: el cliente se va
            OnCustomerWrongTooMany();   
        }
        // si no llega a 3, no pasa nada: el cliente sigue esperando
    }

    // Se llama cuando el jugador se equivoca 3 veces con elmismo cliente
    private void OnCustomerWrongTooMany()
    {
        if (changingCustomer) return;
        changingCustomer = true;

        var randomizer = currentCustomer.GetComponent<RecipeRandomizer>();
        if (randomizer != null)
            randomizer.StopAllCoroutines();   // cancela RestoreOrderText y cualquier otra

        CustomerType type = GameManager.Instance != null
            ? GameManager.Instance.customerType
            : (randomizer != null ? randomizer.customerType : null);

        if (type != null && type.wrongDeliveryLines != null && type.wrongDeliveryLines.Count > 0)
            recipeText.text = type.wrongDeliveryLines[Random.Range(0, type.wrongDeliveryLines.Count)];
        else
            recipeText.text = "Que tardon... Me piro";

        GameManager.Instance.hasCustomer = false;
        StartCoroutine(CustomerLeavesAfterDelay());
    }

    // Se llama cuando se agota el tiempo
    private void OnCustomerTimeOut()
    {
        if (changingCustomer) return;

        changingCustomer = true;

        recipeText.text = "Que tardon... Me piro";

        // Liberar el cliente guardado
        GameManager.Instance.hasCustomer = false;

        StartCoroutine(CustomerLeavesAfterDelay());
    }

    private IEnumerator CustomerLeavesAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        Destroy(currentCustomer);

        SpawnCustomer();
    }

    private IEnumerator ChangeCustomerAfterDelay()
    {
        // Esperar 2 segundos  para que se lea el agradecimiento
        yield return new WaitForSeconds(2f);

        Destroy(currentCustomer);

        SpawnCustomer();
    }
}
