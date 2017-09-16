using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderNode : MonoBehaviour
{
    public int nodeNum;
    public Material notSelected;
    public Material selected;
    public Material notIncluded;
    public Material optimal;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private int numDivisions = 360;
    private PathSelection pathSelection;
    private Nodes nodes;
    private MeshRenderer rend;

    void Start ()
    {
        BuildMesh();
        var mainCam = GameObject.FindWithTag("MainCamera");
        pathSelection = mainCam.GetComponent<PathSelection>();
        nodes = mainCam.GetComponent<Nodes>();
        rend = GetComponent<MeshRenderer>();
    }

    void Update ()
    {
        if (GameController.health <= 0)
            rend.material = optimal;
        else if (pathSelection.CurrentPath.Contains(nodeNum))
            rend.material = selected;
        else if (nodes.Path.Contains(nodeNum))
            rend.material = notSelected;
        else
            rend.material = notIncluded;
    }
	
    void BuildMesh ()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>();
        vertices.Add(Vector3.zero);
        triangles = new List<int>();
        var i = 0;

        for (var x = 0f; x <= numDivisions; x++)
        {
            var angle = x / numDivisions * 2 * Mathf.PI;
            AddPoint(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0), i);
            i++;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void AddPoint(Vector3 point, int i)
    {
        vertices.Add(point);
        if (i > 0)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
    }
}
