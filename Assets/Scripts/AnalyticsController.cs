using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine;

public class AnalyticsController : MonoBehaviour
{
    public static AnalyticsController analyticsController;

    Dictionary<string, object> event_parameters = new Dictionary<string, object>();

    //Dictionary<string, object> event_parameters;
    [HideInInspector] public int level_number = 1;
    [HideInInspector] public int level_count;
    [HideInInspector] public float time;


    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init();
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        if (analyticsController == null)
        {
            //level_count = PlayerPrefs.GetInt("level_count", 0);
            analyticsController = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        level_count++;
        start_analitics();
    }

    private void Start()
    {
        GameAnalytics.Initialize();
    }

    private void Common_parameters()
    {
        event_parameters.Add("level_count", level_count);
        event_parameters.Add("level_number", level_number);
        event_parameters.Add("level_random", true);
    }

    private void start_analitics()
    {
        event_parameters.Clear();
        Common_parameters();
        Event_analitics("level_start");

        //Debug.Log(level_count + " / " + PlayerPrefs.GetInt("Level_analitics") + " / " + level_loop);
    }

    private void finish_analitics()
    {
        event_parameters.Clear();
        Common_parameters();
        event_parameters.Add("time", (int) time);
        Event_analitics("level_finish");

        //Debug.Log(level_count + " / " + PlayerPrefs.GetInt("Level_analitics") + " / " + level_loop + " / " + result + " / " + time + " / " + progress);
    }

    private void Event_analitics(string start_finish)
    {
    }

    public void ResetVar()
    {
        finish_analitics();
        time = 0;
        level_number++;
        level_count++;
        PlayerPrefs.SetInt("level_count", level_count);
        start_analitics();
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
    }
}