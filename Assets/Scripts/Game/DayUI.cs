using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;

    [Header("Display Settings")]
    [SerializeField] private string dayPrefix = "Day ";

    private void Awake()
    {
        if (dayText == null)
            dayText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogError("GameTimeManager bulunamadý!");
            return;
        }

        GameTimeManager.Instance.OnDayChanged += UpdateDay;

        UpdateDay(GameTimeManager.Instance.CurrentDay);
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayChanged -= UpdateDay;
        }
    }

    private void UpdateDay(int day)
    {
        dayText.text = $"{dayPrefix}{day}";
    }
}