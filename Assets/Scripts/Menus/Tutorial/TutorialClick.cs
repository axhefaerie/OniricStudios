using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialClick : MonoBehaviour, IPointerClickHandler
{
    public TutorialManager tutorialManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        tutorialManager.NextImage();
    }
}