using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static int score;
    public static int health;
    public static float minEfficiency;
    public static int maxHealth = 5;
    public static bool needRestart = false;
    public static bool needUndo = false;
    public static bool needSkip = false;

    public GameObject GameOverPanel;

    private float difficulty = 3f;

    private PathMeasurement measure;
    private Nodes nodes;
    private RenderPaths render;
    private NodeGenerator generator;

    private Text scoreText;
    private Text healthText;
    private GameObject panel;
    private Button undoButton;
    private Button skipButton;
    private RectTransform minLine;

    void Start ()
    {
        measure = GetComponent<PathMeasurement>();
        nodes = GetComponent<Nodes>();
        render = GetComponent<RenderPaths>();
        generator = GetComponent<NodeGenerator>();
        scoreText = GameObject.FindWithTag("ScoreText").GetComponent<Text>();
        healthText = GameObject.FindWithTag("HealthText").GetComponent<Text>();
        undoButton = GameObject.FindWithTag("UndoButton").GetComponent<Button>();
        skipButton = GameObject.FindWithTag("SkipButton").GetComponent<Button>();
        minLine = GameObject.FindWithTag("MinimumLine").GetComponent<RectTransform>();

        RestartGame();
    }

    void Update ()
    {
        if (needRestart)
        {
            RestartGame();
            needRestart = false;
        }
        if (needUndo || Input.GetMouseButtonDown(3))
        {
            Undo();
            needUndo = false;
        }
        if (needSkip || Input.GetMouseButtonDown(4))
        {
            Skip();
            needSkip = false;
        }
    }

    public void UpdateMinEfficiency ()
    {
        minEfficiency = 1 - 0.1f / Mathf.Pow(difficulty, nodes.NodeList.Count / 100f);
        minLine.anchorMin = new Vector2(minEfficiency, 0);
        minLine.anchorMax = new Vector2(minEfficiency, 1);
    }

    public void CheckEfficiency ()
    {
        if (measure.Efficiency >= minEfficiency)
        {
            score++;
            scoreText.text = score.ToString();
            health = Mathf.Min(health + 1, maxHealth);
        }
        else
        {
            health--;
            if (health <= 0)
            {
                panel = Instantiate(GameOverPanel);
                panel.GetComponent<RectTransform>().SetParent(GameObject.FindWithTag("Canvas").GetComponent<RectTransform>(), false);
                GameObject.FindWithTag("GameOverScore").GetComponent<Text>().text = score.ToString();
                undoButton.interactable = false;
                skipButton.interactable = false;
                render.UpdateOptimalPath(true);
            }
        }

        healthText.text = health.ToString();
    }

    public void ForceRestart ()
    {
        needRestart = true;
    }

    public void ForceUndo ()
    {
        needUndo = true;
    }

    public void ForceSkip ()
    {
        needSkip = true;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void RestartGame ()
    {
        score = 0;
        health = maxHealth;
        scoreText.text = score.ToString();
        healthText.text = health.ToString();
        undoButton.interactable = false;
        skipButton.interactable = false;

        nodes.ResetNodes(true);
        measure.ResetPaths();
        render.UpdateOverallPath();
        render.UpdateSelectedPath();
        render.UpdateOptimalPath(false);
        UpdateMinEfficiency();

        if (panel != null)
            Destroy(panel);
    }

    void Undo ()
    {
        nodes.UndoPath();
        render.UpdateOverallPath();
        measure.MeasureUserPath();
    }

    void Skip ()
    {
        generator.Skip();
    }
}
