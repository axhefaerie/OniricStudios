using UnityEngine;
using TMPro;

public class IngredientNames : MonoBehaviour //con esta clase hacemos que se vea el nombre del ingrediente al pasar el raton por encima de él
{
    public string ingredientName;
    public TMP_Text nameText;

    // distancia a la que aparece el nombre por encima del ingrediente por si la queremos cambiar al no verse bien o algo asi
    public float heightOffset = 0.7f;

    private void OnMouseEnter()
    {
        // le cambiamos el nombre
        nameText.text = ingredientName;

        // texto encima del ingrediente
        nameText.transform.position = transform.position +
            Vector3.up * heightOffset;

        // Mostramos el texto
        nameText.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        // Ocultamos el texto
        nameText.gameObject.SetActive(false);
    }
}