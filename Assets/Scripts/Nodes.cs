using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Nodes : MonoBehaviour
{
    public List<Vector2> NodeList;
    public List<int> Path;
    public GameObject NodePrefab;
    public bool UseUndoButton;

    private List<GameObject> nodeObjects;
    private List<int> previousPath;

    private Button undoButton;

    void Awake()
    {
        NodeList = new List<Vector2>();
        Path = new List<int>();
        nodeObjects = new List<GameObject>();

        if (UseUndoButton)
            undoButton = GameObject.FindWithTag("UndoButton").GetComponent<Button>();
    }

    public void ResetNodes(bool addInitialNodes)
    {
        for (var x = 0; x < nodeObjects.Count; x++)
        {
            Destroy(nodeObjects[x]);
        }
        nodeObjects.Clear();

        NodeList.Clear();
        Path.Clear();
        previousPath = Path.ToList();

        if (addInitialNodes)
        {
            AddNode(new Vector2(2, 0));
            AddNode(new Vector2(0, 2));
            AddNode(new Vector2(-2, 0));
        }
    }

    public void AddNode(Vector2 point)
    {
        var node = (GameObject)Instantiate(NodePrefab, point, Quaternion.identity);
        node.GetComponent<RenderNode>().nodeNum = NodeList.Count;
        nodeObjects.Add(node);
        NodeList.Add(point);
    }

    public void MergePath(List<int> newPath)
    {
        if (CheckPathDifferent(newPath))
        {
            if (UseUndoButton)
            {
                previousPath = Path.ToList();
                undoButton.interactable = true;
            }

            if (Path.Count == 0)
            {
                Path = newPath.ToList();
                return;
            }

            var startIndex = Path.IndexOf(newPath.First());
            var endIndex = Path.IndexOf(newPath.Last());

            if (startIndex > endIndex)
            {
                var temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
                newPath.Reverse();
            }

            var insideRange = Path.GetRange(startIndex, endIndex - startIndex + 1); //path from startIndex to endIndex
            var outsideRange = Path.GetRange(endIndex, Path.Count - endIndex).Concat(Path.Take(startIndex + 1)); //path from endIndex to startIndex
            var insideDiffCount = insideRange.Except(newPath).Count();
            var outsideDiffCount = outsideRange.Except(newPath).Count();

            if (insideDiffCount <= outsideDiffCount)
            {
                Path = outsideRange.ToList();
            }
            else
            {
                Path = insideRange.ToList();
                newPath.Reverse();
            }

            Path = Path.Where(x => !newPath.Contains(x) || x == newPath.Last()).ToList();
            endIndex = Path.IndexOf(newPath.Last());
            Path.InsertRange(endIndex, newPath.GetRange(0, newPath.Count - 1));
        }
    }

    bool CheckPathDifferent (List<int> newPath)
    {
        if (Path.Count == 0)
            return true;

        var startIndex = Path.IndexOf(newPath.First());
        var endIndex = Path.IndexOf(newPath.Last());
        if (startIndex < 0 || endIndex < 0)
            return false;

        var forwardPathSame = true;
        var backwardPathSame = true;
        var size = Path.Count;
        for (var x = 1; x < newPath.Count; x++)
        {
            var curForwardIndex = (startIndex + x) % size;
            var curBackwardIndex = (startIndex - x) % size;
            if (curBackwardIndex < 0)
                curBackwardIndex += size;

            if (newPath[x] != Path[curForwardIndex])
                forwardPathSame = false;
            if (newPath[x] != Path[curBackwardIndex])
                backwardPathSame = false;
        }
        return !forwardPathSame && !backwardPathSame;
    }

    public void UndoPath()
    {
        Path = previousPath;
        undoButton.interactable = false;
    }
}
