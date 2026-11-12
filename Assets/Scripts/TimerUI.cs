using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Slider timerBar;

    private void Start()
    {
        timerBar.minValue = 0;
        timerBar.maxValue = 120;
        timerBar.wholeNumbers = false;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        timerBar.SetValueWithoutNotify(
            Mathf.Clamp(GameManager.Instance.remainingTime, 0, 120)
        );
    }
}
