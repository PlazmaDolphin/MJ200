using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoliceSpawner : MonoBehaviourSingleton<PoliceSpawner>
{
    public Action<int> OnDayChanged;
    public Action<bool> OnRaidingStateChanged;

    [Header("Police Spawning")]
    public GameObject policePrefab;
    public static List<GameObject> policeList = new List<GameObject>();
    private bool raiding = false;
    public bool IsRaiding => raiding;

    [Header("Time")]
    private float initTime;
    private int day = 0;
    public UnityEvent onWin;
    public int CurrentDay => day;

    [Header("Settings")]
    [SerializeField] private int GameStartTime = 600; // Game time starts at 10:00
    [SerializeField] private int PoliceGetBoredTime = 600; // 06:00
    [SerializeField] private int PoliceRaidTime = 1400; // 14:00
    [SerializeField] private float MinsPerSecondScale = 25f;
    [SerializeField] private float PoliceLineLenght = 20f;

    [Header("Sound Effects")]
    [SerializeField] private SoundFXData raidStartSound;
    [SerializeField] private SoundFXData endRaidSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetIngameMilTime() >= PoliceRaidTime && !raiding)
        {
            day++;
            OnDayChanged?.Invoke(day);

            spawnPoliceRaid();
            raiding = true;
            OnRaidingStateChanged?.Invoke(true);
        }
        // call off raid at next day's wakeup time
        if (GetIngameMilTime() >= PoliceGetBoredTime && GetIngameMilTime() < PoliceRaidTime && raiding)
        {
            callOffRaid();
            raiding = false;
            OnRaidingStateChanged?.Invoke(false);
        }
        if (day >= 3 && policeList.Count == 0)
        {
            //Trigger win condition
            onWin.Invoke();
            Debug.Log("You win!");
        }
    }

    void spawnPoliceRaid()
    {
        // Spawn row of police in an evenly spaced horizontal line
        int numPolice = 3 + (day - 1) * 2;
        for (int i = 0; i < numPolice; i++)
        {
            float x = transform.position.x - PoliceLineLenght / 2 + i * PoliceLineLenght / (numPolice - 1);
            GameObject police = Instantiate(policePrefab, new Vector3(x, transform.position.y, transform.position.z), Quaternion.identity);
            policeList.Add(police);
        }

        if (raidStartSound) raidStartSound.Play();
    }

    void callOffRaid()
    {
        if (endRaidSound) endRaidSound.Play();

        // TODO: Replace with police heading back to spawn
        foreach (GameObject police in policeList)
        {
            Destroy(police);
        }
        policeList.Clear();
    }

    public int GetIngameMilTime()
    {
        // Calculate the time displayed on the clock as a 24-hour 4-digit int
        int currentTime = (int)((Time.time - initTime) * MinsPerSecondScale);
        currentTime += GameStartTime;
        currentTime %= 1440;
        // format as hhmm
        currentTime = 100 * (currentTime / 60) + currentTime % 60;
        return currentTime;
    }

    public float GetDayProgress01()
    {
        int time = GetIngameMilTime();

        int startMinutes = GameStartTime;
        int endMinutes = PoliceGetBoredTime;

        int currentMinutes =
            (time / 100) * 60 + (time % 100);

        if (currentMinutes < startMinutes)
            currentMinutes += 1440;

        float dayLength = endMinutes - startMinutes;
        float elapsed = currentMinutes - startMinutes;

        return Mathf.Clamp01(elapsed / dayLength);
    }
}
