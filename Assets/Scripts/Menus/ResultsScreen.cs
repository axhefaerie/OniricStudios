using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI historyText;
    //public TextMeshProUGUI starsText;

    private void Start()
    {
        bool victory = GameManager.Instance.gameResult == "victory";

        resultText.text = victory ? "¡VICTORIA!" : "DERROTA...";
        descriptionText.text = victory ? "Los clientes han quedados muy satisfechos con tu trabajo" : "No has conseguido preparar correctamente los platos.";
        historyText.text = victory ? "Tras atenderlos correctamente, te ayudaron a volver a tu mundo." 
            : "Tu torpeza con los pedidos hizo que los clientes se enfadaran, y como no pudieron comer los platos que querían al final te han comido a ti.";
        //starsText.text = $"{GameManager.Instance.finalStars} / {GameManager.Instance.maxStars} estrellas";
    }

    public void RestartGame()
    {
        // Reiniciar el estado de la partida
        GameManager.Instance.totalStarsEarned = 0;
        GameManager.Instance.totalStarsPossible = 0;
        GameManager.Instance.specialCustomersServed = 0;
        GameManager.Instance.hasCustomer = false;
        GameManager.Instance.remainingTime = GameManager.Instance.maxTime;

        SceneManager.LoadScene("TitleScreen");   // vuelves al inicio
    }
}
