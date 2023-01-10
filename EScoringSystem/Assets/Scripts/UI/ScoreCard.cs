using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public class ScoreCard : MonoBehaviour
{
    public TextMeshProUGUI RoundScoreBlue;
    public TextMeshProUGUI RoundScoreRed;
    public TextMeshProUGUI MatchScoreBlue;
    public TextMeshProUGUI MatchScoreRed;
    public bool MatchStarted = false;

    private List<Button> AllButtons;

    //REMINDER: these were used for earlier testing. Might not be needed anymore, so remember to remove these if neccessary. 
    private int roundScoreBlue = 0;
    private int roundScoreRed = 0;

    // Start is called before the first frame update
    //REMINDER: again, these were set for testing. Remove them if neccessary. 
    void Awake()
    {

        RoundScoreBlue.text = "0";
        RoundScoreRed.text = "0";
        MatchScoreBlue.text = "0";
        MatchScoreRed.text = "0";
        AllButtons = GameObject.FindObjectsOfType<Button>().ToList();

        foreach (var button in AllButtons)
        {
            button.interactable = false;
        }

        StartCoroutine(CheckIfPaused());



    }


    //Simple add points to send to DB and update based on buttons pressed/clicked. 
    public void AddBluePoints(int point)
    {

        roundScoreBlue += point;
        RoundScoreBlue.text = roundScoreBlue.ToString();
        StartCoroutine(AddPoints(2, point));
    }
    public void AddRedPoints(int point)
    {

        roundScoreRed += point;
        RoundScoreRed.text = roundScoreRed.ToString();
        StartCoroutine(AddPoints(1, point));
    }

    //Test to see if it's actually connecting to the API
    public IEnumerator GetPoints()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //formData.Add(new MultipartFormDataSection("id=" + team + "&points=" + points));

        //UnityWebRequest www = UnityWebRequest.Post("https://localhost:7221/Player/AddPoints", formData);
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:7221/Player/GetPoints");
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
            Debug.Log(www.downloadHandler.text);
        }
    }

    //Calls to the API ands sends info to the DB to update.
    public IEnumerator AddPoints(int player, int points)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", player.ToString() + "points", points.ToString()));


        //WWWForm formData = new WWWForm();
        //formData.AddField("id", team);
        //formData.AddField("points", points);

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/AddPoints?id={player}&points={points}", formData);
        Debug.Log(www.downloadedBytes.ToString());

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Points added");

            StartCoroutine(GetPoints());
        }


    }

    IEnumerator CheckIfPaused()
    {
        while (true)
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
                Debug.Log(www.downloadHandler.text);
                var json = www.downloadHandler.text;
                var jObj = JsonConvert.DeserializeObject<JObject>(json);

                var gamePaused = (bool)jObj["TimerPaused"];
                MatchStarted = gamePaused;

                if (gamePaused)
                {
                    foreach (var button in AllButtons)
                    {
                        button.interactable = false;
                    }
                }
                else
                {
                    foreach (var button in AllButtons)
                    {
                        button.interactable = true;
                    }
                }


            }

            yield return null;
        }
    }

}
