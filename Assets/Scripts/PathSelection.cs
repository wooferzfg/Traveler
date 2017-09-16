using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathSelection : MonoBehaviour {
    public List<int> CurrentPath;
    public bool CanInteract;

    private int currentNode = -1;
    private Nodes nodes;
    private RenderPaths render;
    private PathMeasurement measure;
    private Vector2 mousePosition;
    private bool selectingPath = false;

    void Start ()
    {
        nodes = GetComponent<Nodes>();
        render = GetComponent<RenderPaths>();
        measure = GetComponent<PathMeasurement>();
    }

    void Update ()
    {
        if (GameController.health > 0 && CanInteract)
        {
            mousePosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0)) //press left mouse button
                selectingPath = true;
            if (Input.GetMouseButtonDown(1)) //press right mouse button
                ClearPath();
            if (Input.GetMouseButtonUp(0)) //release left mouse button
            {
                if (CurrentPath.Count > 1)
                {
                    nodes.MergePath(CurrentPath);
                    measure.MeasureUserPath();
                    render.UpdateOverallPath();
                }
                ClearPath();
            }

            if (selectingPath)
                UpdateCurrentPath();
        }
        else
            ClearPath();
    }

    void ClearPath()
    {
        selectingPath = false;
        currentNode = -1;
        CurrentPath.Clear();
        render.UpdateSelectedPath();
    }

    void UpdateCurrentPath ()
    {
        if (CheckNodeChanged())
        {
            for (var x = 0; x < nodes.NodeList.Count; x++)
            {
                if (Vector2.Distance(nodes.NodeList[x], mousePosition) <= 0.5)
                {
                    if (CurrentPath.Count > 0 && CurrentPath.Last() == x)
                        CurrentPath.RemoveAt(CurrentPath.Count - 1);
                    else if (!CurrentPath.Contains(x))
                        CurrentPath.Add(x);
                    currentNode = x;
                    render.UpdateSelectedPath();
                    return;
                }
            }
            currentNode = -1;
        }
    }

    bool CheckNodeChanged ()
    {
        if (currentNode < 0)
            return true;
        return Vector2.Distance(nodes.NodeList[currentNode], mousePosition) > 1;
    }
}
