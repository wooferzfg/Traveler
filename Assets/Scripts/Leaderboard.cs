using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour {

	void Start ()
    {
        var namesText = GameObject.FindWithTag("Names").GetComponent<Text>();
        var scoresText = GameObject.FindWithTag("Scores").GetComponent<Text>();
        var highscores = Highscores.scores;

        var names = "";
        var scores = "";
        for (var x = 0; x < highscores.Length; x++)
        {
            var curScore = highscores[x];
            names += curScore.username + "\n";
            scores += curScore.score + "\n";
        }
        namesText.text = names;
        scoresText.text = scores;
	}

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
