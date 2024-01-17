using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour, IDataPersistence
{
    public static TimeController Instance { get; private set; }
    [SerializeField] private float timeMultiplier = 700f;
    [SerializeField] public static float startHour { get; private set; } = 7.5f;

    [SerializeField] private Light sunLight;
    [SerializeField] private float sunriseHour;
    [SerializeField] private float sunsetHour;
    [SerializeField] private float maxSunLightIntensity;

    [SerializeField] private Light moonLight;
    [SerializeField] private float maxMoonLightIntensity;

    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;
    [SerializeField] private AnimationCurve lightChangeCurve;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;
    private int currentDay = 1;
    private bool dayAlreadyChanged = true; // prevent day count multiple times in one day
    private bool waveAlreadySpawned = false; // can be saved/loaded to spawn zombies when the game is saved/loaded when the time just turn night


    // cached variables for sun and moon rotation
    private TimeSpan sunriseToSunsetDuration;
    private TimeSpan timeSinceSunrise;
    private TimeSpan sunsetToSunriseDuration;
    private TimeSpan timeSinceSunset;
    // cached variable for caculating timeDifference
    private TimeSpan difference;
    // cached variable for updating lightSetting
    private float dotProduct;

    public static float sunLightRotation;
    public static float moonLightRotation;
    public int CurrentDay => currentDay;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        dayText.text = $"Day {currentDay}";
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSetting();

        CheckSpawnWave();
    }

    private void UpdateTimeOfDay()
    {
        // count day
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        // change to new day
        if (currentTime.Hour == 7 && !dayAlreadyChanged)
        {
            waveAlreadySpawned = false;

            DataPersistenceManager.Instance.SaveGame();

            dayAlreadyChanged = true;
            currentDay++;
            dayText.text = $"Day {currentDay}";

            // respawn zombies in areas when new reach new day
            EnemySpawnManager.Instance.SpawnZombieInAreas();

            string message = "";
            if (currentDay == EnemySpawnManager.Instance.FinalDay)
            {
                MapManager.Instance.ToggleExtractionPoint(true);
                message = "I could hear the helicopter, rescue is here";
            }
            else message = "Survived another day, gotta prepare for the next night";
            Notification.Instance.DisplayMessage(message, 0.1f, 1.5f);
        }

        if (currentTime.Hour == 6) dayAlreadyChanged = false;

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void RotateSun()
    {

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            moonLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, new Vector3(1, 0.65f, 0));
        moonLight.transform.rotation = Quaternion.AngleAxis(moonLightRotation * -1f, new Vector3(1, 0.65f, 0));
    }

    private void UpdateLightSetting()
    {
        dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        difference = toTime - fromTime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }

    private void CheckSpawnWave()
    {
        if (IsNightTime() && !waveAlreadySpawned)
        {
            Debug.Log($"Spawned wave zombies and is nighttime: {IsNightTime()} ({currentTime.ToString("HH:mm")})");
            DataPersistenceManager.Instance.SaveGame();

            string message = "It's night time, the zombies are coming";
            Notification.Instance.DisplayMessage(message, 0.1f, 1.5f);
            // Debug.Log("Night TIme!!");
            // Debug.Log(currentDay - 1);
            StartCoroutine(EnemySpawnManager.Instance.SpawnWaveZombies(currentDay));
            waveAlreadySpawned = true;
        }
    }

    public bool IsNightTime()
    {
        if (!(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime))
        {
            // Debug.Log("Night Time!!");
            return true;
        }
        // Debug.Log("Day Time!!");
        return false;
    }

    public void LoadData(GameData data)
    {
        currentTime = data.CurrentTime.DateTime;
        currentDay = data.CurrentDay;
        sunLightRotation = data.SunLightRotation;
        moonLightRotation = data.MoonLightRotation;
        waveAlreadySpawned = data.WaveAlreadySpawned;

        timeText.text = currentTime.ToString("HH:mm");
        dayText.text = $"Day {currentDay}";
        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, new Vector3(1, 0.65f, 0));
        moonLight.transform.rotation = Quaternion.AngleAxis(moonLightRotation * -1f, new Vector3(1, 0.65f, 0));
    }

    public void SaveData(ref GameData data)
    {
        data.CurrentTime.DateTime = currentTime;
        data.CurrentDay = currentDay;
        data.SunLightRotation = sunLightRotation;
        data.MoonLightRotation = moonLightRotation;
        data.WaveAlreadySpawned = waveAlreadySpawned;
    }
}
