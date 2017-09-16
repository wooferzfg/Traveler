using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    private Nodes nodes;
    private PathMeasurement measure;
    private RenderPaths render;

    void Start ()
    {
        nodes = GetComponent<Nodes>();
        GameController.health = 1;

        RegenerateNodes();
        nodes.Path = GetComponent<PathMeasurement>().GetOptimalPath();
        GetComponent<RenderPaths>().UpdateOverallPath();
    }

    void RegenerateNodes()
    {
        nodes.ResetNodes(false);
        for (var x = 0; x < 30;)
        {
            var point = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            if (NodeGenerator.CheckNodePosition(point, nodes.NodeList))
            {
                nodes.AddNode(point);
                x++;
            }
        }
    }

	public void StartGame ()
    {
        SceneManager.LoadScene("Main");
    }

    public void StartTutorial ()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Exit ()
    {
        Application.Quit();
    }
}
