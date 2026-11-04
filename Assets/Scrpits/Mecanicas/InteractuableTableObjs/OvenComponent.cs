using UnityEngine;

public class OvenComponent : MonoBehaviour
{
    // Método cuando se lleva el plato al horno para cocinar
    public void BakePlate(PlateComponent plate)
    {
        // Llama al método de Bake del plato para cambiar el sprite de la receta
        if (plate == null) return;
        plate.Bake();   // true/false según si se formó la receta
    }
}
