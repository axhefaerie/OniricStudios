
using UnityEngine;

public class bar : MonoBehaviour //maneja la lógca del minijuego de la barra
{
    private float MoveSpeed = 5.0f;
    public float speedIncrease = 2.0f;
    private int direction = 1;

    public OvenComponent oven; // horno
    private int hits = 0;      // aciertos conseguidos
    public int hitsNeeded = 3; // aciertos necesarios
    private bool finished = false;// 3 aciertos hechos

    public RectTransform targetZone; // zona acierto
    public RectTransform timingBar;  // barra completa



    void FixedUpdate()
    {
        if (finished)
            return;
        // si llega al límite derecho, cambia de dirección
        if (transform.position.x >= 3.75f)
            direction = -1;

        // si llega al límite izquierdo, cambia de dirección
        if (transform.position.x <= -3.75f)
            direction = 1;

        // con esto se mueve la barita del medio
        transform.position = new Vector3(
            transform.position.x + MoveSpeed * Time.fixedDeltaTime * direction,
            transform.position.y,
            transform.position.z
        );
    }

    private void MoveTargetZone()
    {
        // Calculamos el espacio disponible para mover la zona verde
        float limit = (timingBar.rect.width - targetZone.rect.width) / 2f;

        // Elegimos una posición aleatoria dentro de la barra
        float newX = Random.Range(-limit, limit);

        // Aplicamos el desplazamiento
        targetZone.anchoredPosition = new Vector2(
            newX,
            targetZone.anchoredPosition.y
        );

    }

    private void Update()
    {
        // si el minijuego ha terminado, no hacemos nada
        if (finished)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //   comrpbamos la posición del indicador y de la zona verde
            float indicatorX = transform.position.x;
            float targetX = targetZone.position.x;

            // comprobar si el indicador coincide con la zona verde
            if (Mathf.Abs(indicatorX - targetX) <=
                (targetZone.rect.width / 2f))
            {
                hits++;

                MoveSpeed += speedIncrease;
                MoveTargetZone();

                Debug.Log("Hit: " + hits);

                // Comprobamos si hemos conseguido los aciertos necesarios
                if (hits >= hitsNeeded)
                {
                    finished = true;

                    oven.FinishCooking();
                }
            }
            else
            {
                Debug.Log("Miss");
            }
        }
    }

}
