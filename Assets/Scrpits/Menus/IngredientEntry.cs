using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Este script se encarga de recibir un ingrediente y mostrar su nombre y su sprite

public class IngredientEntry : MonoBehaviour
{
    public Image ingredientImage;
    public TMP_Text ingredientName;

    public void SetIngredient(IngredientComponent ingredient)
    {
        ingredientImage.sprite = ingredient.sprite;
        ingredientImage.preserveAspect = true; // Para que la imagen no se deforme
        ingredientName.text = ingredient.name;
    }
}
