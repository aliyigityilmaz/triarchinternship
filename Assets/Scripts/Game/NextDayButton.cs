using UnityEngine;
using UnityEngine.UI;

public class NextDayButton : MonoBehaviour
{
    [SerializeField] private Button nextDayButton;

    private void Awake()
    {
        if (nextDayButton == null)
            nextDayButton = GetComponent<Button>();
    }

    private void Start()
    {
        if (GameTimeManager.Instance == null)
        {
            Debug.LogError("GameTimeManager bulunamadý!");
            return;
        }

        nextDayButton.onClick.AddListener(StartNextDay);

        GameTimeManager.Instance.OnDayFinished += ShowButton;
        GameTimeManager.Instance.OnDayStarted += HideButton;

        UpdateButtonState();
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance == null)
            return;

        GameTimeManager.Instance.OnDayFinished -= ShowButton;
        GameTimeManager.Instance.OnDayStarted -= HideButton;

        nextDayButton.onClick.RemoveListener(StartNextDay);
    }

    private void ShowButton()
    {
        gameObject.SetActive(true);
    }

    private void HideButton()
    {
        gameObject.SetActive(false);
    }

    private void UpdateButtonState()
    {
        gameObject.SetActive(
            GameTimeManager.Instance != null &&
            GameTimeManager.Instance.CanStartNextDay()
        );
    }

    private void StartNextDay()
    {
        GameTimeManager.Instance.StartNextDay();
    }
}