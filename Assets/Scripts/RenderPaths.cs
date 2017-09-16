using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RenderPaths : MonoBehaviour {
    public Material OverallColoring;
    public Material SelectedPathColoring;
    public Material OptimalColoring;
    public GameObject Drawer;

    private LineDrawer overallPath;
    private LineDrawer selectedPath;
    private LineDrawer optimalPath;
    private Nodes nodes;
    private PathSelection pathSelection;
    private PathMeasurement measure;

	void Awake ()
    {
        var overallPathDrawer = Instantiate(Drawer);
        overallPath = overallPathDrawer.GetComponent<LineDrawer>();
        overallPath.Coloring = OverallColoring;
        overallPath.LineWidth = 0.2f;
        var selectedPathDrawer = Instantiate(Drawer);
        selectedPath = selectedPathDrawer.GetComponent<LineDrawer>();
        selectedPath.Coloring = SelectedPathColoring;
        selectedPath.LineWidth = 0.2f;
        var optimalDrawer = Instantiate(Drawer);
        optimalPath = optimalDrawer.GetComponent<LineDrawer>();
        optimalPath.Coloring = OptimalColoring;
        optimalPath.LineWidth = 0.2f;

        nodes = GetComponent<Nodes>();
        pathSelection = GetComponent<PathSelection>();
        measure = GetComponent<PathMeasurement>();
    }

    public void UpdateOverallPath ()
    {
        var points = GetPointsFromPath(nodes.Path, 2, true);
        overallPath.Points = points;
        overallPath.UpdateLines();
    }

    public void UpdateSelectedPath ()
    {
        var points = GetPointsFromPath(pathSelection.CurrentPath, 1, false);
        selectedPath.Points = points;
        selectedPath.UpdateLines();
    }

    public void UpdateOptimalPath (bool showPath)
    {
        var path = new List<int>();
        if (showPath)
        {
            overallPath.Points = new List<Vector3>();
            overallPath.UpdateLines();
            path = measure.GetOptimalPath();
        }

        var points = GetPointsFromPath(path, 1, true);
        optimalPath.Points = points;
        optimalPath.UpdateLines();
    }

    List<Vector3> GetPointsFromPath (List<int> path, float depth, bool wrap)
    {
        var points = new List<Vector3>();
        for (var x = 0; x < path.Count; x++)
        {
            var curPoint = nodes.NodeList[path[x]];
            points.Add(new Vector3(curPoint.x, curPoint.y, depth));
        }
        if (path.Count > 0 && wrap)
        {
            var firstPoint = nodes.NodeList[path[0]];
            points.Add(new Vector3(firstPoint.x, firstPoint.y, depth));
        }
        return points;
    }
}
