using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Highscores : MonoBehaviour {
    const string privateCode = "0AdJp-jCSUeX4ogeoRZm8A1ooym5bn-ESqO7I_GxPFsg";
    const string publicCode = "5747408c8af6030e8cef220a";
    const string webURL = "http://dreamlo.com/lb/";

    public static bool needToSubmit = false;
    public static Highscore[] scores;

    void Update ()
    {
        if (needToSubmit)
        {
            SubmitScore();
            needToSubmit = false;
        }
    }

    public void ForceSubmit ()
    {
        needToSubmit = true;
    }

    private void SubmitScore ()
    {
        var username = GameObject.FindWithTag("UsernameText").GetComponent<Text>().text.Replace('*', ' ');
        StartCoroutine(UploadNewHighscore(username, GameController.score));
        GameObject.FindWithTag("SubmitButton").GetComponent<Button>().interactable = false;
    }

    IEnumerator UploadNewHighscore(string username, int score)
    {
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;
    }

    public void GetScores()
    {
        scores = new Highscore[0];
        WWW www = new WWW(webURL + publicCode + "/pipe/10");

        while (!www.isDone && string.IsNullOrEmpty(www.error)) { }

        if (www.isDone)
            FormatHighscores(www.text);

        SceneManager.LoadScene("Leaderboard");
    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        var highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
        }
        scores = highscoresList;
    }
}

public struct Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }

}
