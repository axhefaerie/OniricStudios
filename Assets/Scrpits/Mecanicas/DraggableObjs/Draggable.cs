using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Draggable : MonoBehaviour
{
    private Camera camera;              // Camara
    private Vector3 offset;             // vector de movimiento
    private Vector3 originalPosition;   // posición original del ingrediente en la mesa
    private bool isDragging = false;    // Bool que indica si está siendo arrastrado un objecto

    protected virtual void Start()
    {
        camera = Camera.main;                       // Se coge la cámara
        originalPosition = transform.position;      // Se asigna la posición inicial como la posición inicial del sprite
       
    }

    // MÉTODOS
    // Se ejecuta cuando se pulsa el botón del ratón (mantener)
    private void OnMouseDown()
    {
        isDragging = true;          // El bool se passa a true, para indicar que está siendo arrastrado
        offset = transform.position - GetMouseWorldPos();    // Se calcula el desplazamietno inicial
    }

    // Mientras se mantenga el botón pulsado, se mueve el ingrediente
    private void OnMouseDrag()
    {
        if (isDragging)         // Mientras el boton del ratón está pulsado, el sprite se debe mover con el ratón
            transform.position = GetMouseWorldPos() + offset;           // Se cambia su posición en base al ratón
    }

    // Se ejecuta cuando se libera el botón del ratón
    private void OnMouseUp()
    {
        isDragging = false;         // Cuando se suelta el ratón, se pasa a false el bool
        OnDrop();
    }

    // Cada objeto define su comportamiento al soltar
    protected abstract void OnDrop();

    // Método que devuelve el ingrediente a su posición original
    protected void ReturnToOrigin()
    {
        transform.position = originalPosition;      // Mueve el sprite a la posición original
    }

    public void SetOriginalPosition(Vector3 newPosition) //permite modificar la posición a la que vuelve el
                                                         //plato cuando se terminar de arrastrar al horno
    {
        originalPosition = newPosition;
    }

    // Devuelve la posición del ratón para arrastrarlo correctamente
    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mouse = Input.mousePosition;        // Coge la posicón del ratón
        mouse.z = transform.position.z - camera.transform.position.z;       // Se calcula la posición
        return camera.ScreenToWorldPoint(mouse);        // Se devuelve la posición en base a la cámara
    }
}
