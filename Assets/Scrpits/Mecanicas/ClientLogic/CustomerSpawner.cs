using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Sprite[] customerSprites;
    public float spawnDelay = 0.5f;

    private GameObject currentCustomer;
    private int actualCustomer;
    private int oldCustomer;

    private void Start()
    {
        SpawnCustomer();
    }

    // Este metodo genera aleatoriamente un cliente
    public void SpawnCustomer()
    {
        currentCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);

        if (customerSprites.Length > 0)
        {
            var sr = currentCustomer.GetComponent<SpriteRenderer>();    // Se coge el sprite del current customer
            actualCustomer = Random.Range(0, customerSprites.Length);
            while(actualCustomer == oldCustomer)
            {
                actualCustomer = Random.Range(0, customerSprites.Length);
            }
            sr.sprite = customerSprites[actualCustomer];
        }
    }

    public void OnCustomerServed()
    {
        StartCoroutine(ChangeCustomerAfterDelay());
    }

    private IEnumerator ChangeCustomerAfterDelay()
    {
        // Espera 2 segundos para que se lea el agradecimiento
        yield return new WaitForSeconds(2f);

        Destroy(currentCustomer);
        SpawnCustomer();
    }
}
