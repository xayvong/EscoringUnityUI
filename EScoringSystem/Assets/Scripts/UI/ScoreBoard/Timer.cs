using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI Output;

    public delegate void TimerFinished();
    public event TimerFinished OnTimerFinished;

    private void Awake()
    {
        Output = GetComponentInChildren<TextMeshProUGUI>();
        input = GetComponent<TMP_InputField>();
        if (input != null)
        {
            input.onValueChanged.AddListener(delegate { SetTime(); });
        }


        //StartTimer(20);
    }
    private TMP_InputField input;


    public void StartTimer(float startTime)
    {
        if (input != null)
        {
            var minutes = float.Parse(input.text.Split(':')[0]);
            var seconds = float.Parse(input.text.Split(':')[1]);

            var totalTime = minutes * 60 + seconds;
            Debug.Log(totalTime);
            timeLeft = totalTime;
            input.interactable = false;
        }
        else
        {
            timeLeft = startTime;
        }



        //timeLeft= startTime;
        StartCoroutine(timer());
    }
    private static float timeSet { get; set; }
    public static float timeInMinutes 
    {
        get { return timeSet / 60; }
    
    }
    public static float timeInSeconds
    {
        get { return timeSet; }
    }

    public void SetTime()
    {
        if (input != null && input.interactable)
        {
            var minutes = float.Parse(input.text.Split(':')[0]);
            var seconds = float.Parse(input.text.Split(':')[1]);

            var totalTime = minutes * 60 + seconds;

            timeSet = totalTime;

        }

    }


    public bool TimerRunning { get; private set; }
    public bool Pause { get; set; }
    private float timeLeft;

    public void ResetTimer()
    {
        Pause = false;
        timeLeft = 0;
        //Output.text = 0.ToString().PadLeft(2, '0') + ":" + 0.ToString().PadLeft(2, '0');
        SetText(0.ToString().PadLeft(2, '0') + ":" + 0.ToString().PadLeft(2, '0'));
        if (input != null)
        {
            var seconds = timeSet % 60;
            var minutes = (timeSet - seconds) / 60;

            SetText(minutes.ToString().PadLeft(2, '0') + ":" + Math.Floor(seconds).ToString().PadLeft(2, '0'));
            Debug.Log("Time is Set To " + timeSet);
        }
    }

    private IEnumerator timer()
    {
        TimerRunning = true;

        while (timeLeft > 0)
        {
            if (!Pause)
            {

                var seconds = timeLeft % 60;
                var minutes = (timeLeft - seconds) / 60;

                //Output.text = minutes.ToString().PadLeft(2 , '0') + ":" + Math.Floor(seconds).ToString().PadLeft(2 , '0');
                SetText(minutes.ToString().PadLeft(2, '0') + ":" + Math.Floor(seconds).ToString().PadLeft(2, '0'));
                timeLeft -= Time.deltaTime;

            }


            yield return null;
        }
        TimerRunning = false;

        OnTimerFinished?.Invoke();
        ResetTimer();
    }

    private void SetText(string text)
    {
        if (input != null)
        {
            input.text = text;
        }
        else
        {
            Output.text = text;
        }
    }
}
