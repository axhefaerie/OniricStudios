using UnityEngine;

// Esta clase maneja las propeidades de los ingredientes
[CreateAssetMenu(fileName = "IngredientComponent", menuName = "Scriptable Objects/IngredientComponent")]
public class IngredientComponent : ScriptableObject
{
    public int id;              // Identificador para comprobar la receta   
    public string name;         // Nombre del ingrediente
    public IngredientType type;     // Indicador del tipo de ingrediente 
    public bool cooked = false;         // Bool que indica si un ingrediente ha sido cocinado
    
    public Sprite sprite;               // Sprite de cada ingrediente
    public Sprite plateSprite;  // Sprite que aparece en el plato

    // Enumerador de los distintos tipos de ingrediente
    public enum IngredientType
    {
        Base,
        Sauce,
        Topping,
        Flower
    }

    // Constructor
    public IngredientComponent(int _id, string _name, IngredientType _type, bool _cooked)
    {
        id = _id;
        name = _name;
        type = _type;
        cooked = _cooked;
    }
}
