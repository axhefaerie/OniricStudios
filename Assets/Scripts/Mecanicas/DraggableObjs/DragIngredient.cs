using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


// Esta clase permite arrastrar los ingredientes al plato para ir añadiendolos
public class DragIngredient : Draggable
{
    public IngredientComponent ingredientData;  // Info del ScriptableObject
 
    protected override void Start()
    {
        base.Start();           // Se ejecuta el método Start de Draggable (por si acaso)
        GetComponent<SpriteRenderer>().sprite = ingredientData.sprite;     // Coge el sprite del ScriptableObject (del ingrediente)
    }

    // Método que se ejecuta cuando se suelta el ratón al estar moviendo un ingrediente
    protected override void OnDrop()
    {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPos(), Vector2.zero);     // Se comprueba si el collider del ingrediente ha dado con otro collider
        if (hit.collider != null)
        {
            PlateComponent plate = hit.collider.GetComponent<PlateComponent>();     // Se comprueba si ha sido con el collider del plato
            if (plate != null)
            {
                if (plate.AddIngredient(ingredientData))        // Se llama al método AddINgredient para añadirlo al plato
                {                                               // Devuelve true si se ha conseeguido añadir
                    ReturnToOrigin();                       // Se devuelve el sprite a su posición original para poder volver a usarlo
                    return;
                }
            }
        }
        ReturnToOrigin();       // Si no se ha coincidido con el plato o no se ha podido añadir al plato, igualmente el sprite vuelve a su posición
    }  
}
