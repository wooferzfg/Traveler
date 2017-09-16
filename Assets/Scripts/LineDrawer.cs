using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour
{
    public GameObject DrawerItem;
    public Material Coloring;
    public float LineWidth;
    public List<Vector3> Points;

    public void UpdateLines()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for (var x = 0; x < Points.Count - 1; x++)
        {
            var drawerItem = Instantiate(DrawerItem);
            drawerItem.transform.parent = transform;
            var lineRenderer = drawerItem.GetComponent<LineRenderer>();
            lineRenderer.material = Coloring;
            lineRenderer.SetWidth(LineWidth, LineWidth);

            var positions = Points.GetRange(x, 2);
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, positions[0]);
            lineRenderer.SetPosition(1, positions[1]);
        }
    }
}
