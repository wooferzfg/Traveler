using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathMeasurement : MonoBehaviour {
    public float UserPath;
    public float OptimalPath;
    public float Efficiency;
    public bool GetButtons;
    public bool UseSkipButton;
    
    private Nodes nodes;
    private int userPathCount;
    private int optimalPathCount;

    private Button skipButton;
    private Text efficiencyText;
    private RectTransform efficiencyBar;
    private Image efficiencyBarImage;
    private Color greenBar = new Color(0, 100f/255, 30f/255);
    private Color redBar = new Color(100f/255, 0, 0);

    void Awake ()
    {
        nodes = GetComponent<Nodes>();
        if (GetButtons)
        {
            skipButton = GameObject.FindWithTag("SkipButton").GetComponent<Button>();
            efficiencyText = GameObject.FindWithTag("EfficiencyText").GetComponent<Text>();
            var barObject = GameObject.FindWithTag("EfficiencyBar");
            efficiencyBar = barObject.GetComponent<RectTransform>();
            efficiencyBarImage = barObject.GetComponent<Image>();
        }
    }

    public void ResetPaths ()
    {
        UserPath = 0;
        userPathCount = 0;
        MeasureOptimalPath();
    }

    float MeasurePath (List<int> path)
    {
        var pathLength = 0f;
        for (var x = 0; x < path.Count; x++)
        {
            var nextIndex = x + 1;
            if (nextIndex >= path.Count)
                nextIndex = 0;
            pathLength += Vector2.Distance(nodes.NodeList[path[x]], nodes.NodeList[path[nextIndex]]);
        }
        return pathLength;
    }

    void CalculateEfficiency ()
    {
        if (optimalPathCount > userPathCount)
        {
            Efficiency = 0;
            efficiencyText.text = "Not All Nodes Connected";
            if (UseSkipButton)
                skipButton.interactable = false;
        }
        else
        {
            Efficiency = OptimalPath / UserPath;
            efficiencyText.text = string.Format("{0:f1}", Efficiency * 100) + "% Efficiency";
            if (UseSkipButton)
                skipButton.interactable = true;
        }
        efficiencyBar.anchorMax = new Vector2(Mathf.Min(Efficiency, 1), 1);
        if (Efficiency >= GameController.minEfficiency)
            efficiencyBarImage.color = greenBar;
        else
            efficiencyBarImage.color = redBar;
    }

    public void MeasureUserPath ()
    {
        UserPath = MeasurePath(nodes.Path);
        userPathCount = nodes.Path.Count;
        CalculateEfficiency();
    }

    public void MeasureOptimalPath ()
    {
        var path = GetOptimalPath();
        OptimalPath = MeasurePath(path);
        optimalPathCount = path.Count;
        CalculateEfficiency();
    }

    public List<int> GetOptimalPath ()
    {
        var adjacencies = GetMST();
        var odds = GetOdds(adjacencies);
        var newAdjacencies = MatchOdds(odds, adjacencies.Count);
        var graph = CombineGraphs(adjacencies, newAdjacencies);
        var euler = Euler(graph);
        var path = Hamiltonian(euler);
        for (var x = 0; x < 10; x++)
            path = Optimize(path);

        return path;
    }

    List<List<int>> GetMST()
    {
        var size = nodes.NodeList.Count;
        var parents = new int[size];
        var weights = new float[size];
        var marked = new bool[size];
        var pq = new IndexMinPriorityQueue<float>(size);

        for (var x = 1; x < size; x++)
        {
            weights[x] = float.MaxValue;
        }
        weights[0] = 0;

        pq.Insert(0, 0);
        while (!pq.IsEmpty())
        {
            int v = pq.DeleteMin();
            marked[v] = true;
            for (var x = 0; x < size; x++)
            {
                if (x != v && !marked[x])
                {
                    var distance = Vector2.Distance(nodes.NodeList[v], nodes.NodeList[x]);
                    if (distance < weights[x])
                    {
                        weights[x] = distance;
                        parents[x] = v;
                        if (pq.Contains(x))
                            pq.ChangeKey(x, distance);
                        else
                            pq.Insert(x, distance);
                    }
                }
            }
        }

        var result = GetAdjacencyLists(size);
        for (var x = 1; x < parents.Length; x++)
        {
            var target = parents[x];
            result[x].Add(target);
            result[target].Add(x);
        }
        return result;
    }

    List<int> GetOdds(List<List<int>> adjacencies)
    {
        var result = new List<int>();
        for (var x = 0; x < adjacencies.Count; x++)
        {
            if (adjacencies[x].Count % 2 == 1)
                result.Add(x);
        }
        return result;
    }

    List<List<int>> MatchOdds(List<int> odds, int size)
    {
        var result = GetAdjacencyLists(size);
        while (odds.Count > 0)
        {
            var v = odds.Last();
            odds.RemoveAt(odds.Count - 1);
            var bestDistance = float.MaxValue;
            var bestMatch = -1;
            for (var x = 0; x < odds.Count; x++)
            {
                var u = odds[x];
                var distance = Vector2.Distance(nodes.NodeList[v], nodes.NodeList[u]);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMatch = u;
                }
            }
            result[v].Add(bestMatch);
            result[bestMatch].Add(v);
            odds.Remove(bestMatch);
        }
        return result;
    }

    List<List<int>> GetAdjacencyLists(int size)
    {
        var result = new List<List<int>>();
        for (var x = 0; x < size; x++)
        {
            result.Add(new List<int>());
        }
        return result;
    }

    List<List<int>> CombineGraphs(List<List<int>> a, List<List<int>> b)
    {
        var result = new List<List<int>>();
        for (var x = 0; x < a.Count; x++)
        {
            result.Add(a[x].Concat(b[x]).ToList());
        }
        return result;
    }

    List<int> Euler(List<List<int>> g)
    {
        var graph = new List<List<int>>();
        for (var x = 0; x < g.Count; x++)
        {
            graph.Add(g[x].ToList());
        }
        var path = new List<int>();
        var stack = new Stack<int>();

        var cur = 0;
        while (stack.Count > 0 || graph[cur].Count > 0)
        {
            if (graph[cur].Count == 0)
            {
                path.Add(cur);
                cur = stack.Pop();
            }
            else
            {
                stack.Push(cur);
                var neighbor = graph[cur].Last();
                graph[cur].RemoveAt(graph[cur].Count - 1);
                for (var x = 0; x < graph[neighbor].Count; x++)
                {
                    if (graph[neighbor][x] == cur)
                    {
                        graph[neighbor].RemoveAt(x);
                        break;
                    }
                }
                cur = neighbor;
            }
        }
        path.Add(cur);

        return path;
    }

    List<int> Hamiltonian(List<int> euler)
    {
        List<int> result = new List<int>();
        var visited = new bool[euler.Count];

        for (var x = 0; x < euler.Count; x++)
        {
            var node = euler[x];
            if (!visited[node])
            {
                result.Add(node);
                visited[node] = true;
            }
        }

        return result;
    }

    List<int> Optimize(List<int> p)
    {
        var path = p.ToList();
        var n = path.Count;

        for (var i = 0; i < n; i++)
        {
            var a1 = i;
            var b1 = (i + 1) % n;

            for (var j = i + 2; (j + 1) % n != i; j++)
            {
                var a2 = j % n;
                var b2 = (j + 1) % n;

                if (IsShorter(path[a1], path[b1], path[a2], path[b2]))
                    Reverse(path, i + 1, j, n);
            }
        }

        return path;
    }

    bool IsShorter(int a1, int b1, int a2, int b2)
    {
        var oldDist1 = Vector2.Distance(nodes.NodeList[a1], nodes.NodeList[b1]);
        var oldDist2 = Vector2.Distance(nodes.NodeList[a2], nodes.NodeList[b2]);
        var newDist1 = Vector2.Distance(nodes.NodeList[a1], nodes.NodeList[a2]);
        var newDist2 = Vector2.Distance(nodes.NodeList[b1], nodes.NodeList[b2]);
        return newDist1 + newDist2 < oldDist1 + oldDist2;
    }

    void Reverse(List<int> path, int start, int end, int n)
    {
        while (end - start > 0)
        {
            var temp = path[start % n];
            path[start % n] = path[end % n];
            path[end % n] = temp;
            start++;
            end--;
        }
    }
}
