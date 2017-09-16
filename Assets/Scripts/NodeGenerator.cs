using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeGenerator : MonoBehaviour {
    public float timeUntilNode;

    private Nodes nodes;
    private Camera cam;
    private PathMeasurement measure;
    private GameController controller;

    private Image timerCircle;

    private float timeBetweenNodes = 15f;
    private float zoomOutRate = 4f;
    private float startingArea = 15f;

	void Start ()
    {
        timeUntilNode = timeBetweenNodes;
        nodes = GetComponent<Nodes>();
        cam = GetComponent<Camera>();
        measure = GetComponent<PathMeasurement>();
        controller = GetComponent<GameController>();
        timerCircle = GameObject.FindWithTag("TimerCircle").GetComponent<Image>();
    }

    void Update()
    {
        if (GameController.health > 0)
        {
            timeUntilNode -= Time.deltaTime;
            var portionRemaining = timeUntilNode / timeBetweenNodes;
            cam.orthographicSize = GetCameraSize(portionRemaining);
            timerCircle.fillAmount = portionRemaining;
        }

        if (timeUntilNode < 0)
        {
            controller.CheckEfficiency();
            timeUntilNode = timeBetweenNodes;

            if (GameController.health > 0)
            {
                var point = new Vector2(0, 0);
                var limit = cam.orthographicSize * 0.8f;

                do
                {
                    point.x = Random.Range(-limit, limit);
                    point.y = Random.Range(-limit, limit);
                }
                while (!CheckNodePosition(point, nodes.NodeList));

                nodes.AddNode(point);
                measure.MeasureOptimalPath();
                controller.UpdateMinEfficiency();
            }
        }
    }

    public static bool CheckNodePosition (Vector2 point, List<Vector2> nodeList)
    {
        for (var x = 0; x < nodeList.Count; x++)
        {
            if (Vector2.Distance(nodeList[x], point) < 1.5)
                return false;
        }
        return true;
    }

    public void Skip ()
    {
        cam.orthographicSize = GetCameraSize(0);
        timeUntilNode = 0f;
    }

    float GetCameraSize (float tRem)
    {
        var n = nodes.NodeList.Count;
        var previousSize = Mathf.Sqrt(startingArea + zoomOutRate * n);
        var nextSize = Mathf.Sqrt(startingArea + zoomOutRate * (n + 1));
        return Mathf.Lerp(nextSize, previousSize, tRem);
    }
}
