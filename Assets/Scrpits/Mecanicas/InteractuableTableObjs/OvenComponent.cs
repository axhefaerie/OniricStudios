using UnityEngine;

public class OvenComponent : MonoBehaviour
{

    public GameObject minigamePanel; //referencia al panel donde aparece el minijuego
    public Animator doorsAnimator; // referencia al animator de las puertas

    private PlateComponent currentPlate;//plato que hemos introducido en el horno
    private bool isCooking = false;// horno ocupado
    private bool isReady = false;//horno vacio/plato terminado para recoger


    // Método cuando se lleva el plato al horno para cocinar
    public void BakePlate(PlateComponent plate)
    {
        // Llama al método de Bake del plato para cambiar el sprite de la receta
        if (plate == null || plate.HasBeenBaked || isCooking || isReady) return;

        // Comprobamos que los ingredientes forman una receta
       // if (!plate.HasValidRecipe())
     //   {
      //      Debug.LogWarning("Este plato no contiene una receta válida.");
      //      return;
      //  }

        currentPlate = plate; //se acepta el plato recibido
        isCooking = true; //el horno está cocinando 

        currentPlate.gameObject.SetActive(false);//desaparece el plato temporalmente mientras cocina(aparece el fueguito)

        doorsAnimator.SetTrigger("Cerrar"); // Cerrar puertas horno

        minigamePanel.SetActive(true);
        //plate.Bake(); // true/false según si se formó la receta
    }

    public void FinishCooking() //que el horno termine cuando el minijuego se logre pasar
    {
        doorsAnimator.SetTrigger("Abrir"); // abrir puertas horno

        if (!isCooking || currentPlate == null) //comprueba que hay un plato cocinandose
            return;

        bool baked = currentPlate.Bake(); // guarda el resultado de baked que puede ser verdadero(cocinado) o falso(no se ha logrado cocinar)

        if (baked) //si la receta es válida salen las animaciones se acaba el minijuego etc
        {
            isCooking = false;
            isReady = true;

            minigamePanel.SetActive(false);

            currentPlate.transform.position = transform.position; //vuelve a mostrar plato en el horno

            // Actualizar su posición original para poder arrastrarlo desde el horno
            currentPlate.GetComponent<Draggable>().SetOriginalPosition(transform.position);

            currentPlate.gameObject.SetActive(true);
        }

        else
        {
            // Si no existe una receta válida, devolver el plato
            Debug.LogWarning("No se puede cocinar este plato.");

            minigamePanel.SetActive(false);

            currentPlate.gameObject.SetActive(true);
            currentPlate = null;
            isCooking = false;
        }
    }

    public void TakePlate()// avisamos al horno de que se ha recogido el plato
    {
        if (!isReady)
            return;

        currentPlate = null;
        isReady = false;
    }


}
