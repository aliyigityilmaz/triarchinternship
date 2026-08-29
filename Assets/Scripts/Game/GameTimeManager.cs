using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance { get; private set; }

    [Header("Day Settings")]
    [SerializeField] private int startingDay = 1;
    [SerializeField] private float dayDurationInSeconds = 240f;

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
    public event Action<float> OnTimeChanged;

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
        InitializeFirstDay();
    }

    private void Update()
    {
        if (!IsDayActive)
            return;

        CurrentTime += Time.deltaTime *
                       (endHour - startHour) /
                       dayDurationInSeconds;

        OnTimeChanged?.Invoke(CurrentTime);

        if (CurrentTime >= endHour)
        {
            CurrentTime = endHour;
            FinishDay();
        }
    }

    private void InitializeFirstDay()
    {
        CurrentDay = startingDay;
        CurrentTime = startHour;

        IsDayActive = true;
        IsDayFinished = false;

        OnDayStarted?.Invoke();
        OnDayChanged?.Invoke(CurrentDay);
        OnTimeChanged?.Invoke(CurrentTime);
    }

    private void FinishDay()
    {
        if (!IsDayActive)
            return;

        IsDayActive = false;
        IsDayFinished = true;

        CurrentTime = endHour;

        OnTimeChanged?.Invoke(CurrentTime);
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
        OnTimeChanged?.Invoke(CurrentTime);
    }

    public string GetFormattedTime()
    {
        return $"{CurrentHour:00}:{CurrentMinute:00}";
    }

    public string GetFormattedDay()
    {
        return $"Day {CurrentDay}";
    }
}