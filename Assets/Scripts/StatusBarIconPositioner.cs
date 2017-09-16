using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatusBarIconPositioner : MonoBehaviour {
    private LayoutElement layout;
    private RectTransform rectT;

    void Start ()
    {
        layout = GetComponent<LayoutElement>();
        rectT = GetComponent<RectTransform>();
    }

    void Update ()
    {
        layout.minWidth = rectT.rect.height;
	}
}
