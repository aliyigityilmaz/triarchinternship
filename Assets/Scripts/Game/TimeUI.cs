using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        if (timeText == null)
            timeText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogError("GameTimeManager bulunamadý!");
            return;
        }

        GameTimeManager.Instance.OnTimeChanged += UpdateTime;

        UpdateTime(GameTimeManager.Instance.CurrentTime);
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnTimeChanged -= UpdateTime;
        }
    }

    private void UpdateTime(float time)
    {
        timeText.text = GameTimeManager.Instance.GetFormattedTime();
    }
}