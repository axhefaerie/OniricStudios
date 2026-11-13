using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Slider timerBar;
    public float maxTime = 120f;

    private void Start()
    {
        timerBar.minValue = 0f;

        // Usa el máximo centralizado del GameManager
        float max = GameManager.Instance != null ? GameManager.Instance.maxTime : maxTime;
        timerBar.maxValue = max;
        timerBar.wholeNumbers = false;

        float start = GameManager.Instance != null
            ? Mathf.Clamp(GameManager.Instance.remainingTime, 0f, max)
            : max;

        timerBar.SetValueWithoutNotify(start);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        timerBar.SetValueWithoutNotify(
            Mathf.Clamp(GameManager.Instance.remainingTime, 0f, GameManager.Instance.maxTime)
        );
    }
}
