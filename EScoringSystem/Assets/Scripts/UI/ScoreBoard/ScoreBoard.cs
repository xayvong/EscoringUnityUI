using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ScoreBoard : MonoBehaviour
{
    public Timer Timer;

    public GameObject Winner;
    public TextMeshProUGUI ChongScore;
    public TextMeshProUGUI HongScore;

    public TextMeshProUGUI ChongPenalty;
    public TextMeshProUGUI HongPenalty;

    public TextMeshProUGUI ChongMatchPoint;
    public TextMeshProUGUI HongMatchPoint;

    public TextMeshProUGUI ChongName;
    public TextMeshProUGUI HongName;

    public TextMeshProUGUI Round;

    public float RoundTime = 0;

    private bool matchStarted { get; set; }



    private void Start()
    {
        roundNumber = 1;

        //Timer.OnTimerFinished += TimeExpired;
        Winner.SetActive(false);
        StartCoroutine(CheckScore());
    }

    private void TimeExpired()
    {
        checkScore = false;
        roundNumber++;
        Round.text = roundNumber.ToString();

        matchStarted = false;
        ChongScore.text = "0";
        HongScore.text = "0";
        ChongPenalty.text = "0";
        HongPenalty.text = "0";
        StartCoroutine(CheckScore());

    }
    private int roundNumber = 1;
    public int HongPoint;

    public void StartNewRound()
    {
        Round.text = roundNumber.ToString();

        ChongScore.text = "0";
        HongScore.text = "0";
        ChongPenalty.text = "0";
        HongPenalty.text = "0";

        Timer.StartTimer(RoundTime);

    }
    //New round never comes back and re-iterates which is why the timer never starts. 
    //Edit, new round works properly for gap points or penalties, but doesn't iterate properly when timer runs out.

    private bool checkScore;

    IEnumerator CheckScore()
    {
        checkScore = true;

        while (checkScore)
        {
            UnityWebRequest www = UnityWebRequest.Get($"https://localhost:7221/Player/UpdateScoreBoard");
            yield return www.SendWebRequest();



            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                while (!www.downloadHandler.isDone)
                {
                    yield return null;
                }

                var json = www.downloadHandler.text;
                var jObj = JsonConvert.DeserializeObject<JObject>(json);
                var matchEnded = (bool)jObj["MatchEnded"];
                ChongName.text = (string)jObj["ChongName"];
                HongName.text = (string)jObj["HongName"];

                if (!matchStarted && !matchEnded)
                {


                    var startDuration = float.Parse(jObj["TimeDuration"].ToString());
                    Debug.Log(startDuration);
                    if (startDuration > 0)
                    {
                        var matchStart = (DateTime)jObj["TimeStart"];
                        var timeDifference = DateTime.Now - matchStart;
                        var realTime = (float)((startDuration * 60) - (timeDifference.TotalSeconds));
                        this.Timer.StartTimer(realTime);
                        matchStarted = true;
                        Winner.SetActive(false);
                    }


                }
                else if (matchStarted)
                {
                    var hong = (JObject)jObj["hong"];
                    var chong = (JObject)jObj["chong"];
                    HongScore.text = hong["Score"].ToString();
                    HongPenalty.text = hong["Penalties"].ToString();

                    ChongScore.text = chong["Score"].ToString();
                    ChongPenalty.text = chong["Penalties"].ToString();

                    var gamePaused = (bool)jObj["TimerPaused"];

                    if (matchEnded)
                    {
                        checkScore = false;
                        Timer.ResetTimer();
                        TimeExpired();
                        ChongMatchPoint.text = chong["MatchPoint"].ToString();
                        HongMatchPoint.text = hong["MatchPoint"].ToString();
                        Debug.Log(chong["MatchPoint"].ToString());
                        Debug.Log(matchEnded);

                        var getWinner = (string)jObj["Winner"];
                        Debug.Log(getWinner);

                        if (!string.IsNullOrEmpty(getWinner))
                        {
                            Winner.SetActive(true);
                            var winText = Winner.GetComponentInChildren<TextMeshProUGUI>();
                            winText.text = getWinner + " Wins";

                            roundNumber = 1;
                            Round.text = "1";
                            ChongMatchPoint.text = "0";
                            HongMatchPoint.text = "0";
                            var showWinner = true;
                            while (showWinner)
                            {
                                www = UnityWebRequest.Get($"https://localhost:7221/Player/UpdateScoreBoard");
                                yield return www.SendWebRequest();



                                if (www.result != UnityWebRequest.Result.Success)
                                {
                                    Debug.Log(www.error);
                                }
                                else
                                {
                                    while (!www.downloadHandler.isDone)
                                    {
                                        yield return null;
                                    }

                                    json = www.downloadHandler.text;
                                    jObj = JsonConvert.DeserializeObject<JObject>(json);

                                    getWinner = (string)jObj["Winner"];
                                    
                                    if (string.IsNullOrEmpty(getWinner))
                                    {
                                        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
                                    }
                                }
                                    yield return null;
                            }
                        }


                    }
                    else
                    {
                        var matchPause = gamePaused;
                        if (matchPause)
                        {
                            this.Timer.Pause = true;
                        }
                        else
                        {
                            this.Timer.Pause = false;
                        }
                    }


                }


            }
            yield return null;
        }

    }

    IEnumerator MatchEnded(bool value)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/MatchEnded?value={value}", formData);
        yield return www.SendWebRequest();
    }



}
