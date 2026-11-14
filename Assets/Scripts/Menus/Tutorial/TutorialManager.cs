using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    public Image tutorialImage;
    public Sprite[] tutorialImages;
    public string gameSceneName = "Game";

    private int currentImage = 0;

    void Start()
    {
        currentImage = 0;
        ShowImage();
    }

    public void NextImage()
    {
        currentImage++;

        if (currentImage >= tutorialImages.Length)
        {
            SkipTutorial();
            return;
        }

        ShowImage();
    }

    void ShowImage()
    {
        tutorialImage.sprite = tutorialImages[currentImage];
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}