using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour {
    public GameObject Arrow;

    private Camera cam;
    private Text tutorialText;
    private Button nextButton;
    private Text nextButtonText;
    private GameObject timer;
    private Text scoreText;
    private Text healthText;
    private Button undoButton;
    private Button skipButton;
    
    private PathMeasurement measure;
    private Nodes nodes;
    private PathSelection pathSelection;
    private RenderPaths render;
    private List<TutorialItem> tutorial;
    private int curItem;
    private List<GameObject> arrows;

    private Color disabledColor = new Color(47f / 510, 236f / 510, 0f);
    private Color enabledColor = new Color(47f / 255, 236f / 255, 0f);

    void Start ()
    {
        measure = GetComponent<PathMeasurement>();
        nodes = GetComponent<Nodes>();
        pathSelection = GetComponent<PathSelection>();
        render = GetComponent<RenderPaths>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        tutorialText = GameObject.FindWithTag("TutorialText").GetComponent<Text>();
        nextButton = GameObject.FindWithTag("NextButton").GetComponent<Button>();
        nextButtonText = GameObject.FindWithTag("NextButtonText").GetComponent<Text>();
        timer = GameObject.FindWithTag("TimerCircle");
        scoreText = GameObject.FindWithTag("ScoreText").GetComponent<Text>();
        healthText = GameObject.FindWithTag("HealthText").GetComponent<Text>();
        undoButton = GameObject.FindWithTag("UndoButton").GetComponent<Button>();
        skipButton = GameObject.FindWithTag("SkipButton").GetComponent<Button>();

        RestartGame();
        CreateTutorial();
    }
	
	void Update ()
    {
        if (curItem == 1) //connect the three nodes
        {
            if (measure.Efficiency >= 0.9995)
                NextItem();
            else if (nodes.Path.Count > 0)
            {
                nodes.Path.Clear();
                render.UpdateOverallPath();
            }
        }
        else if (curItem == 3) //connect additional node
        {
            if (arrows.Count == 0)
            {
                AddArrow(new Vector2(2, 0.75f), 0);
                AddArrow(new Vector2(0, 2.75f), 0);
                AddArrow(new Vector2(3, 3.75f), 0);

                nodes.AddNode(new Vector2(3, 3));
                measure.MeasureOptimalPath();
            }
            if (measure.Efficiency >= 0.9995)
            {
                ClearArrows();
                NextItem();
            }
            else if (nodes.Path.Count > 3)
            {
                nodes.Path = new List<int>() { 0, 1, 2 };
                render.UpdateOverallPath();
                measure.MeasureUserPath();
            }
        }
        else if (curItem == 7) //modify path to optimize
        {
            if (nodes.NodeList.Count == 4)
            {
                nodes.ResetNodes(false);
                nodes.AddNode(new Vector2(4, 3));
                nodes.AddNode(new Vector2(-2, 2));
                nodes.AddNode(new Vector2(1, 3));
                nodes.AddNode(new Vector2(3, -1));
                nodes.AddNode(new Vector2(-4, 0));
                nodes.Path = new List<int>() { 0, 1, 4, 2, 3 };

                render.UpdateOverallPath();
                measure.MeasureOptimalPath();
                measure.MeasureUserPath();
            }
            if (measure.Efficiency >= 0.9995)
                NextItem();
        }
        else if (curItem == 10) //efficiency bar
        {
            if (arrows.Count == 0)
                AddArrow(new Vector2(0, CalculateTopArrowY()), 180);
        }
        else if (curItem == 11) //timer
        {
            arrows[0].transform.position = new Vector3(CalculateArrowX(timer.GetComponent<RectTransform>()), CalculateTopArrowY());
        }
        else if (curItem == 12) //lives counter
        {
            arrows[0].transform.position = new Vector3(CalculateArrowX(healthText.rectTransform), CalculateTopArrowY());
        }
        else if (curItem == 13) //undo button
        {
            arrows[0].transform.position = new Vector3(CalculateArrowX(undoButton.GetComponent<RectTransform>()), CalculateBottomArrowY());
            arrows[0].transform.rotation = Quaternion.identity;
        }
        else if (curItem == 14) //skip button
        {
            arrows[0].transform.position = new Vector3(CalculateArrowX(skipButton.GetComponent<RectTransform>()), CalculateBottomArrowY());
        }
        else if (curItem == 15)
        {
            if (arrows.Count > 0)
                ClearArrows();
        }

        if (curItem < tutorial.Count)
        {
            var curTutorialItem = tutorial[curItem];
            tutorialText.text = curTutorialItem.TutorialText;
            if (curTutorialItem.Interactive)
            {
                nextButton.interactable = false;
                pathSelection.CanInteract = true;
                nextButtonText.color = disabledColor;
            }
            else
            {
                nextButton.interactable = true;
                pathSelection.CanInteract = false;
                nextButtonText.color = enabledColor;
            }
        }
    }

    float CalculateTopArrowY ()
    {
        var rectT = healthText.rectTransform;
        return cam.ScreenToWorldPoint(new Vector3(0, rectT.position.y - rectT.rect.yMax)).y - 0.5f;
    }

    float CalculateBottomArrowY()
    {
        var rectT = undoButton.GetComponent<RectTransform>();
        return cam.ScreenToWorldPoint(new Vector3(0, rectT.position.y - rectT.rect.yMin)).y + 0.5f;
    }

    float CalculateArrowX (RectTransform uiElem)
    {
        return cam.ScreenToWorldPoint(uiElem.position).x;
    }

    void AddArrow (Vector2 position, float rotation)
    {
        var arrow = (GameObject)Instantiate(Arrow, position, Quaternion.identity);
        arrow.transform.eulerAngles = new Vector3(0, 0, rotation);
        arrows.Add(arrow);
    }

    void ClearArrows ()
    {
        for (var x = 0; x < arrows.Count; x++)
        {
            Destroy(arrows[x]);
        }
        arrows.Clear();
    }

    void CreateTutorial ()
    {
        arrows = new List<GameObject>();
        tutorial = new List<TutorialItem>();
        tutorial.Add(new TutorialItem("Welcome to Traveler. The goal of this game is to create the shortest possible round trip path between all the nodes on the screen.", false));
        tutorial.Add(new TutorialItem("Let's start by connecting the three nodes. You can create a path by clicking and dragging over each of the nodes without releasing in between.", true));
        tutorial.Add(new TutorialItem("The three nodes are now connected. Since the path is as short as possible, the efficiency is at 100%.", false));
        tutorial.Add(new TutorialItem("A new node has now appeared. Add this node to the path by dragging between the three nodes shown.", true));
        tutorial.Add(new TutorialItem("The path now contains all four nodes, so the efficiency is once again at 100%.", false));
        tutorial.Add(new TutorialItem("You can modify the path by dragging between nodes. This can be used to include new nodes or to optimize parts of the existing path.", false));
        tutorial.Add(new TutorialItem("You don't need to redraw the entire path in order to change it. You can also right click in order to cancel the path you are currently selecting.", false));
        tutorial.Add(new TutorialItem("Try modifying this path in order to increase the efficiency back to 100%.", true));
        tutorial.Add(new TutorialItem("With your modifications, the path is once again as short as possible, and the efficiency has returned to 100%.", false));
        tutorial.Add(new TutorialItem("Efficiency can sometimes be greater than 100% if your path is shorter than the one the game finds. You should always keep trying to make your path shorter in order to increase efficiency.", false));
        tutorial.Add(new TutorialItem("The efficiency meter will constantly display the efficiency of your path. The yellow line on the meter shows the minimum efficiency required not to lose a life.", false));
        tutorial.Add(new TutorialItem("The timer shows how much time is left until a new node will appear.", false));
        tutorial.Add(new TutorialItem("The lives counter displays how many lives you currently have. If your efficiency is too low when the timer runs out, you'll lose a life. Otherwise, you'll gain a life.", false));
        tutorial.Add(new TutorialItem("The undo button lets you undo one move. This is useful if you make a mistake while changing the path.", false));
        tutorial.Add(new TutorialItem("The skip button instantly adds the next node so that you don't have to wait for the timer to run out.", false));
        tutorial.Add(new TutorialItem("You have now completed the tutorial.", false));
    }

    public void ExitToMenu ()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextItem ()
    {
        curItem++;
        if (curItem == tutorial.Count - 1)
            nextButtonText.text = "Finish";
        else if (curItem == tutorial.Count)
            ExitToMenu();
    }

    void RestartGame ()
    {
        GameController.score = 0;
        GameController.health = GameController.maxHealth;
        GameController.minEfficiency = 0.85f;
        scoreText.text = GameController.score.ToString();
        healthText.text = GameController.health.ToString();
        undoButton.interactable = false;
        skipButton.interactable = false;

        nodes.ResetNodes(true);
        measure.ResetPaths();

        curItem = 0;
    }

    struct TutorialItem
    {
        public string TutorialText;
        public bool Interactive;

        public TutorialItem(string text, bool interactive)
        {
            TutorialText = text;
            Interactive = interactive;
        }
    }
}
