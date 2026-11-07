
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Clientes")]
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Sprite[] customerSprites;
    public float spawnDelay = 0.5f;
    public TextMeshProUGUI recipeText; //configurar dialogos y esas cosas(?


    [Header("Temporizador")]
    public Slider timerBar;
    public float maxTime = 30f;

    private GameObject currentCustomer;
    private int actualCustomer;
    private int oldCustomer = -1;

    private float currentTime;
    private bool timerRunning = false;
    private bool changingCustomer = false;

    private void Start()
    {
        SpawnCustomer();
    }

    // Genera aleatoriamente un cliente
    public void SpawnCustomer()
    {
        currentCustomer = Instantiate(
            customerPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        if (customerSprites.Length > 0)
        {
            var sr = currentCustomer.GetComponent<SpriteRenderer>();

            actualCustomer = Random.Range(0, customerSprites.Length);

            while (actualCustomer == oldCustomer && customerSprites.Length > 1)
            {
                actualCustomer = Random.Range(0, customerSprites.Length);
            }

            oldCustomer = actualCustomer;

            sr.sprite = customerSprites[actualCustomer];
        }

        // Reiniciar temporizador
        currentTime = maxTime;
        timerBar.maxValue = maxTime;
        timerBar.value = maxTime;

        timerRunning = true;
        changingCustomer = false;
    }

    private void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            // Actualizar la barra
            timerBar.value = currentTime;

            // Si se acaba el tiempo
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
        if (changingCustomer)
            return;

        timerRunning = false;
        changingCustomer = true;

        StartCoroutine(ChangeCustomerAfterDelay());
    }

    // Se llama cuando se agota el tiempo
    private void OnCustomerTimeOut()
    {
        if (changingCustomer)
            return;

        changingCustomer = true;



        // Mostrar mensaje de despedida
        recipeText.text =
                    "Que tardon... Me piro";



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
