using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    public GameObject policePrefab;
    public TextMeshProUGUI timeText;
    public static List<GameObject> policeList = new List<GameObject>();
    private float initTime;
    private int day = 0;
    private bool raiding = false;
    private const int GAME_START_TIME = 600; // Game time starts at 10:00
    private const int POLICE_GOT_BORED_TIME = 600; // 06:00
    private const int POLICE_RAID_TIME = 1400; // 14:00
    private const float MINS_PER_SECOND_SCALE = 25f;
    private const float POLICE_LINE_LENGTH = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = "Time: " + GetIngameMilTime().ToString() + "\nDay: " + day.ToString();
        if (GetIngameMilTime() >= POLICE_RAID_TIME && !raiding)
        {
            day++;
            spawnPoliceRaid();
            raiding = true;
        }
        // call off raid at next day's wakeup time
        if (GetIngameMilTime() >= POLICE_GOT_BORED_TIME && GetIngameMilTime() < POLICE_RAID_TIME && raiding)
        {
            callOffRaid();
            raiding = false;
        }
        if (day >= 3 && policeList.Count == 0)
        {
            //Trigger win condition
            Debug.Log("You win!");            
        }
    }

    void spawnPoliceRaid()
    {
        // Spawn row of police in an evenly spaced horizontal line
        int numPolice = 3 + (day - 1) * 2;
        for (int i = 0; i < numPolice; i++)
        {
            float x = transform.position.x - POLICE_LINE_LENGTH / 2 + i * POLICE_LINE_LENGTH / (numPolice - 1);
            GameObject police = Instantiate(policePrefab, new Vector3(x, transform.position.y, transform.position.z), Quaternion.identity);
            policeList.Add(police);
        }
    }

    void callOffRaid()
    {
        // TODO: Replace with police heading back to spawn
        foreach (GameObject police in policeList)
        {
            Destroy(police);
        }
        policeList.Clear();
    }

    private int GetIngameMilTime()
    {
        // Calculate the time displayed on the clock as a 24-hour 4-digit int
        int currentTime = (int)((Time.time - initTime) * MINS_PER_SECOND_SCALE);
        currentTime += GAME_START_TIME;
        currentTime %= 1440;
        // format as hhmm
        currentTime = 100*(currentTime/60) + currentTime % 60;
        return currentTime;
    }
}
