using UnityEngine;

// Clase que contiene la logica de mover el plato
public class DragPlate : Draggable
{
    // Método que se ejecuta cuando se suelta el ratón al estar moviendo el plato   
    protected override void OnDrop()
    {
        // obtenemos la pos del ratón en el mundo
        Vector2 mousePosition = GetMouseWorldPos();

        // buscamos q colliders hay situados en ese punto
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePosition);

        //al tener varios colliders detectados, tenemos que recorrerlos uno por uno
        foreach (Collider2D hit in hits)
        {
        // hay que ignorar los colliders del propio plato, este era el problema detectaba el plato no la papelera
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            //LOGICA DE ENTREGAR EL PLATO AL CLIENTE
            // comprobar si hemos soltado el plato sobre un cliente
            ClientComponent client = hit.GetComponentInParent<ClientComponent>();

            if (client != null)
            {
                PlateComponent plate = GetComponent<PlateComponent>();

                if (plate != null)
                {
                    plate.Delivery(client);
                }

                ReturnToOrigin();
                return;
            }

            //LÖGICA DE TIRAR PLATO A LA BASURA
            // Comprobar si hemos soltado el plato sobre la papelera
            TrashComponent trash = hit.GetComponentInParent<TrashComponent>();

            if (trash != null)
            {
                trash.PlayTrashSound();
                PlateComponent plate = GetComponent<PlateComponent>();

                if (plate != null)
                {
                    plate.ThrowAway();
                }

                ReturnToOrigin();
                return;
            }
        }

        // Si no se ha soltado sobre ningún objetivo válido,
        // devolver el plato a su posición original
        ReturnToOrigin();
    }

}


