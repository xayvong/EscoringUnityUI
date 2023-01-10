using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditScoreBoard : MonoBehaviour
{

    //Lots going on here, but these are properties pertaining to buttons, panels, and win conditions
    public TextMeshProUGUI ChongScore;
    public TextMeshProUGUI HongScore;

    public TextMeshProUGUI ChongName;
    public TextMeshProUGUI HongName;

    public TextMeshProUGUI ChongPenalty;
    public TextMeshProUGUI HongPenalty;

    public TextMeshProUGUI ChongMatchPoint;
    public TextMeshProUGUI HongMatchPoint;

    public int RoundNumber = 1;
    public TextMeshProUGUI RoundNumText;

    public Button ChongWinButton;
    public Button HongWinButton;
    public GameObject PickWinnerCard;
    public GameObject winnerDetermined;
    private TextMeshProUGUI winnerText;

    public Button ChongChangeNameBtn;
    public Button HongChangeNameBtn;
    public GameObject ChongInput;
    public GameObject HongInput;

    private int _chongMatchPoint;
    private int _hongMatchPoint;

    public Button StartPauseButton;

    private TextMeshProUGUI StartPause;
    public Timer timer;

    private bool MatchStarted = false;
    private List<Button> AllButtons;

    private int ChongScoreInt = 0;
    private int ChongPenaltyInt = 0;
    private int HongScoreInt = 0;
    private int HongPenaltyInt = 0;
    public string input;

    //Quick change name script
    public void ChongButtonName()
    {
        ChongInput.SetActive(true);
        ChongChangeNameBtn.interactable = false;
    }
    public void HongNameButton()
    {
        HongInput.SetActive(true);
        HongChangeNameBtn.interactable = false;
    }

    public void ChongStringInput(string name)
    {
        ChongName.text = name;
        ChongInput.SetActive(false);
        ChongChangeNameBtn.interactable = true;
        StartCoroutine(SetName(1, name));    
    }
    public void HongStringInput(string name)
    {
        HongName.text = name;
        HongInput.SetActive(false) ;
        HongChangeNameBtn.interactable = true;
        StartCoroutine(SetName(2, name));
    }

    //Getter and setter for matchpoints. Should have done this sooner to make things easier.
    private int chongMatchPoint
    {
        get { return _chongMatchPoint; }
        set
        {
            StartCoroutine(PauseResume(true));
            if (value >= 2)
            {
                StartCoroutine(SetWinner(ChongName.text));
                WinnerDetermined(ChongName.text);
            }
            else
            {
                _chongMatchPoint = value;
                ChongMatchPoint.text = value.ToString();
                StartCoroutine(addMatchPoint("chong"));
            }



        }

    }
    private int hongMatchPoint
    {
        get { return _hongMatchPoint; }
        set
        {
            StartCoroutine(PauseResume(true));
            if (value >= 2)
            {
                StartCoroutine(SetWinner(HongName.text));
                WinnerDetermined(HongName.text);
            }
            else
            {
                _hongMatchPoint = value;
                HongMatchPoint.text = value.ToString();
                StartCoroutine(addMatchPoint("hong"));
            }

        }
    }

    //Method to make sure that application and DB resets properly after closing. Will come back and implement more later for practical use.
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");


    }

    public void ResetWholeMatch()
    {
        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }

    //Runs on startup. All sorts of bools, checks, and default values are set here.
    private void Awake()
    {
        StartCoroutine(ResetMatch());
        StartCoroutine(PauseResume(true));
        ChongScore.text = "0";
        ChongPenalty.text = "0";
        HongScore.text = "0";
        HongPenalty.text = "0";
        StartPause = StartPauseButton.GetComponentInChildren<TextMeshProUGUI>();
        StartPause.text = "Start";
        StartPauseButton.onClick.AddListener(startMatch);
        AllButtons = GameObject.FindObjectsOfType<Button>().ToList();
        AllButtons.Remove(StartPauseButton);
        AllButtons.Remove(ChongWinButton);
        AllButtons.Remove(HongWinButton);
        AllButtons.Remove(ChongChangeNameBtn);
        AllButtons.Remove(HongChangeNameBtn);
        timer.OnTimerFinished += Timer_OnTimerFinished;
        PickWinnerCard.SetActive(false);
        winnerDetermined.SetActive(false);
        winnerText = winnerDetermined.GetComponentInChildren<TextMeshProUGUI>();

        foreach (var button in AllButtons)
        {
            button.interactable = false;
        }

    }

    //Panel that declares a winner after conditions are met. 
    private void WinnerDetermined(string player)
    {
        StartCoroutine(PauseResume(true));
        StartCoroutine(addMatchPoint(player));

        winnerText.text = player + " Wins";
        winnerDetermined.SetActive(true);

        foreach (var button in AllButtons)
        {
            button.interactable = true;
        }
        timer.ResetTimer();

    }


    //Add match point after round has ended. Not being referenced because it's assigned to buttons
    public void AddMatchPoint(string player)
    {
        if (player.ToLower() == "chong")
        {
            chongMatchPoint++;
        }
        else
        {
            hongMatchPoint++;
        }
        PickWinnerCard.SetActive(false);
    }

    //Method that determines what happens if the timer is finished. 
    private void Timer_OnTimerFinished()
    {
        MatchStarted = false;
        StartPause.text = "Start";
        StartPauseButton.onClick.RemoveAllListeners();
        StartPauseButton.onClick.AddListener(startMatch);
        foreach (var button in AllButtons)
        {
            button.interactable = false;
        }
        var cScore = int.Parse(ChongScore.text);
        var hScore = int.Parse(HongScore.text);
        var cPen = int.Parse(ChongPenalty.text);
        var hPen = int.Parse(HongPenalty.text);

        if (hPen >= 5)
        {

            chongMatchPoint++;

        }
        else if (cPen >= 5)
        {
            hongMatchPoint++;
        }
        else
        {
            if (cScore > hScore)
            {
                //Chong winner
                chongMatchPoint++;

            }
            else if (hScore > cScore)
            {
                //Hong winner
                hongMatchPoint++;
            }
            else
            {
                //Tied
                PickWinnerCard.SetActive(true);

            }
        }
        RoundNumber++;
        RoundNumText.text = RoundNumber.ToString();

        timer.ResetTimer();
        ChongScore.text = "0";
        ChongPenalty.text = "0";
        HongScore.text = "0";
        HongPenalty.text = "0";
    }

    //Starts the match and sets the time. Will come back and add multiple selectable times later. 
    IEnumerator StartMatch()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        Debug.Log(Timer.timeInMinutes);
        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/StartMatch?matchStart={DateTime.Now}&minutes={Timer.timeInMinutes}", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }

        MatchStarted = true;
        StartCoroutine(CheckScore());

    }

    //Adds match point to DB
    IEnumerator addMatchPoint(string player)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/AddMatchPoint?player={player}", formData);
        yield return www.SendWebRequest();


    }

    //Resets score for both players in DB
    IEnumerator ResetMatch()
    {


        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/Reset", formData);
        yield return www.SendWebRequest();


    }

    //Sends to DB state of match. Paused or resumed.
    private IEnumerator PauseResume(bool pause)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/PauseMatch?pauseTime={DateTime.Now}&isPaused={pause}", formData);
        yield return www.SendWebRequest();
    }

    //As soon as the Start Button is clicked match starts. This sets all the buttons to false and starts the timer.
    private void startMatch()
    {
        StartCoroutine(StartMatch());
        StartCoroutine(PauseResume(false));
        timer.StartTimer(Timer.timeInSeconds);
        StartPause.text = "Pause";
        StartPauseButton.onClick.RemoveAllListeners();
        StartPauseButton.onClick.AddListener(pauseMatch);
        foreach (var button in AllButtons)
        {
            button.interactable = false;
        }
    }

    //Pauses timer and makes buttons interactable.
    private void pauseMatch()
    {
        StartCoroutine(PauseResume(true));
        timer.Pause = true;
        StartPause.text = "Resume";
        StartPauseButton.onClick.RemoveAllListeners();
        StartPauseButton.onClick.AddListener(resumeMatch);
        foreach (var button in AllButtons)
        {
            button.interactable = true;
        }
    }

    //Starts timer again afer pausing.
    private void resumeMatch()
    {
        StartCoroutine(PauseResume(false));
        timer.Pause = false;
        StartPause.text = "Pause";
        StartPauseButton.onClick.RemoveAllListeners();
        StartPauseButton.onClick.AddListener(pauseMatch);
        foreach (var button in AllButtons)
        {
            button.interactable = false;
        }
    }

    //The most important part of this program. Interacts with API and constantly checks if scores have changed and updates accordingly.
    IEnumerator CheckScore()
    {
        while (MatchStarted)
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
                var hong = (JObject)jObj["hong"];
                var chong = (JObject)jObj["chong"];
                HongScore.text = hong["Score"].ToString();
                HongPenalty.text = hong["Penalties"].ToString();

                ChongScore.text = chong["Score"].ToString();
                ChongPenalty.text = chong["Penalties"].ToString();

                var gamePaused = (bool)jObj["TimerPaused"];
            }

            if ((Math.Abs(int.Parse(ChongScore.text) - int.Parse(HongScore.text)) > 11) || int.Parse(ChongPenalty.text) >= 5 || int.Parse(HongPenalty.text) >= 5)
            {
                Debug.Log("Penalty/score/winner");
                timer.ResetTimer();
                yield break;

            }


            yield return null;
        }
    }

    //Everything after this line is API calling and adding/removing points and penalties. 
    public void AddChongPoint(int point)
    {
        //ChongScoreInt += point;
        //ChongScore.text = ChongScoreInt.ToString();
        StartCoroutine(AddPoints(2, point));
    }

    public void AddHongPoint(int point)
    {
        //HongScoreInt += point;
        //HongScore.text = HongScoreInt.ToString();
        StartCoroutine(AddPoints(1, point));
    }

    public void AddChongPenalty(int penalty)
    {
        ChongPenaltyInt += penalty;
        ChongPenalty.text = ChongPenaltyInt.ToString();
        HongScoreInt += penalty;
        HongScore.text = HongScoreInt.ToString();
        StartCoroutine(AddPenalties(2, penalty));

    }
    public void AddHongPenalty(int penalty)
    {
        HongPenaltyInt += penalty;
        HongPenalty.text = HongPenaltyInt.ToString();
        ChongScoreInt += penalty;
        ChongScore.text = ChongScoreInt.ToString();
        StartCoroutine(AddPenalties(1, penalty));
    }

    //Note, none of the remove options should ever lead to a negative value, so a simple check is added to prevent that.
    public void RemoveChongPoint(int point)
    {
        //ChongScoreInt -= point;
        //ChongScore.text = ChongScoreInt.ToString();
        if (int.Parse(ChongScore.text) != 0)
        {
            StartCoroutine(RemovePoints(2, point));
        }


    }
    public void RemoveHongPoint(int point)
    {
        //HongScoreInt -= point;
        //HongScore.text = HongScoreInt.ToString();
        if (int.Parse(HongScore.text) != 0)
        {
            StartCoroutine(RemovePoints(1, point));
        }


    }

    public void RemoveChongPenalty(int penalty)
    {
        if (int.Parse(ChongPenalty.text) != 0)
        {
            ChongPenaltyInt -= penalty;
            ChongPenalty.text = ChongPenaltyInt.ToString();
            HongScoreInt -= penalty;
            HongScore.text = HongScoreInt.ToString();
            StartCoroutine(RemovePenalties(2, penalty));
        }

    }

    public void RemoveHongPenalty(int penalty)
    {
        if (int.Parse(HongPenalty.text) != 0)
        {
            HongPenaltyInt -= penalty;
            HongPenalty.text = HongPenaltyInt.ToString();
            ChongScoreInt -= penalty;
            ChongScore.text = ChongScoreInt.ToString();
            StartCoroutine(RemovePenalties(1, penalty));
        }

    }

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

    public IEnumerator GetScore(int id)
    {

        UnityWebRequest www = UnityWebRequest.Get($"https://localhost:7221/Player/GetScore?id={id}");
        yield return www.SendWebRequest();

        if (id == 1)
        {
            HongScore.text = www.downloadHandler.text;
        }
        else if (id == 2)
        {
            ChongScore.text = www.downloadHandler.text;
        }


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

    private IEnumerator AddPoints(int team, int points)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", team.ToString() + "points", points.ToString()));


        //WWWForm formData = new WWWForm();
        //formData.AddField("id", team);
        //formData.AddField("points", points);

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/AddPoints?id={team}&points={points}", formData);
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
    private IEnumerator RemovePoints(int team, int points)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", team.ToString() + "points", points.ToString()));

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/RemovePoints?id={team}&points={points}", formData);


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
    private IEnumerator AddPenalties(int team, int points)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", team.ToString() + "points", points.ToString()));

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/AddPenalties?id={team}&penalties={points}", formData);
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
    private IEnumerator RemovePenalties(int team, int points)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", team.ToString() + "points", points.ToString()));

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/RemovePenalties?id={team}&penalties={points}", formData);
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

    IEnumerator SetWinner(string winner)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/SetWinner?winner={winner}", formData);
        yield return www.SendWebRequest();
    }

    IEnumerator SetName(int id,string name)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        UnityWebRequest www = UnityWebRequest.Post($"https://localhost:7221/Player/SetName?id={id}&name={name}", formData);
        yield return www.SendWebRequest();
    }
}
