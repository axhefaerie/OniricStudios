using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContactsApp : MonoBehaviour
{
    public CustomerDatabase database;

    public GameObject listScreen;
    public RectTransform content;
    public GameObject rowPrefab;

    public GameObject detailScreen;
    public Image detailPhoto;
    public TextMeshProUGUI detailName;
    public GameObject detailCrown;

    private void OnEnable()
    {
        ShowList();
    }

    // ---------- Pantalla 1 ----------
    public void ShowList()
    {
        listScreen.SetActive(true);
        detailScreen.SetActive(false);
        RefreshList();
    }

    public void RefreshList()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        if (GameManager.Instance == null) return;

        List<CustomerType> unlocked = new List<CustomerType>();
        foreach (string name in GameManager.Instance.unlockedCustomers)
        {
            CustomerType c = database.FindByName(name);
            if (c != null) unlocked.Add(c);
        }

        // Especiales primero, y dentro de cada grupo por orden de desbloqueo
        unlocked.Sort((a, b) =>
        {
            if (a.isSpecial != b.isSpecial) return a.isSpecial ? -1 : 1;
            return GameManager.Instance.unlockedCustomers.IndexOf(a.customerName)
                 .CompareTo(GameManager.Instance.unlockedCustomers.IndexOf(b.customerName));
        });

        foreach (CustomerType c in unlocked)
        {
            GameObject go = Instantiate(rowPrefab, content);
            go.GetComponent<CustomerRow>().Setup(c, this);
        }
    }

    // ---------- Pantalla 2 ----------
    public void OpenDetail(CustomerType c)
    {
        listScreen.SetActive(false);
        detailScreen.SetActive(true);

        detailPhoto.sprite = c.sprite;
        detailName.text = c.customerName;
        detailCrown.SetActive(c.isSpecial);
    }

    // Botón atrás de la ficha
    public void BackToList()
    {
        ShowList();
    }
}
