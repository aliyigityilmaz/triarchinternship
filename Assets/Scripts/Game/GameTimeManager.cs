using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Day Settings")]
    [SerializeField] private int startingDay = 1;

    [SerializeField] private float dayDurationInSeconds = 240f; // 4 minutes

    [Header("Working Hours")]
    [SerializeField] private int startHour = 9;
    [SerializeField] private int endHour = 19;

    public int CurrentDay { get; private set; }
    public float CurrentTime { get; private set; }

    public bool IsDayActive { get; private set; }
    public bool IsDayFinished { get; private set; }

    public int CurrentHour => Mathf.FloorToInt(CurrentTime);
    public int CurrentMinute =>
        Mathf.FloorToInt((CurrentTime - CurrentHour) * 60f);

    public float DayProgress
    {
        get
        {
            float totalMinutes = (endHour - startHour) * 60f;
            float currentMinutes = (CurrentTime - startHour) * 60f;

            return Mathf.Clamp01(currentMinutes / totalMinutes);
        }
    }

    public event Action OnDayStarted;
    public event Action OnDayFinished;
    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartNewDay();
    }

    private void Update()
    {
        if (!IsDayActive)
            return;

        CurrentTime += Time.deltaTime * (endHour - startHour) / dayDurationInSeconds;

        if (CurrentTime >= endHour)
        {
            CurrentTime = endHour;
            FinishDay();
        }
    }

    private void StartNewDay()
    {
        CurrentDay = startingDay;
        CurrentTime = startHour;

        IsDayActive = true;
        IsDayFinished = false;

        OnDayStarted?.Invoke();
        OnDayChanged?.Invoke(CurrentDay);
    }

    private void FinishDay()
    {
        if (!IsDayActive)
            return;

        IsDayActive = false;
        IsDayFinished = true;

        CurrentTime = endHour;

        OnDayFinished?.Invoke();
    }

    public bool CanStartNextDay()
    {
        return IsDayFinished;
    }

    public void StartNextDay()
    {
        if (!CanStartNextDay())
            return;

        CurrentDay++;
        CurrentTime = startHour;

        IsDayActive = true;
        IsDayFinished = false;

        OnDayStarted?.Invoke();
        OnDayChanged?.Invoke(CurrentDay);
    }

    public string GetFormattedTime()
    {
        int hour = CurrentHour;
        int minute = CurrentMinute;

        return $"{hour:00}:{minute:00}";
    }
}