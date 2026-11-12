using UnityEngine;

[CreateAssetMenu(fileName = "Preference", menuName = "Scriptable Objects/Preference")]
public class Preference : ScriptableObject
{
    public string preferenceText;              // "Me apetece algo dulce"
    public RecipeComponent preferenceRecipe;   // la ÚNICA receta ligada (galletas, por ejemplo)
}
