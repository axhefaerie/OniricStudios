using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CustomerType", menuName = "Scriptable Objects/CustomerType")]
public class CustomerType : ScriptableObject
{
    public string customerName;
    public Sprite sprite;       // aspecto
    public bool isSpecial;  // true si es especial

    // PEDIDOS: junta lo que pide el cliente con la receta (así se puede seleccionar lo que dicen los especiales al pedir)
    public List<Preference> preferences;

    // Diálogos que puede decir cada personaje al hacer varias acciones
    public List<string> enterLines;      // al aparecer
    public List<string> correctLines;    // plato correcto
    public List<string> wrongLines;      // plato incorrecto
    public List<string> timeoutLines;    // se acaba el tiempo

    public string GetRandomLine(List<string> lines)
    {
        if (lines == null || lines.Count == 0) return "";
        return lines[Random.Range(0, lines.Count)];
    }
}
