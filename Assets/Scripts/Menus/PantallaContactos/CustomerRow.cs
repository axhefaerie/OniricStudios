using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerRow : MonoBehaviour
{
    public Image thumb;
    public TextMeshProUGUI nameText;
    public Button button;

    public void Setup(CustomerType c, ContactsApp app)
    {
        thumb.sprite = c.iconMobileSprite;
        nameText.text = c.customerName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => app.OpenDetail(c));
    }
}