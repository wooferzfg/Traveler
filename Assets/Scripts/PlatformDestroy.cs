using UnityEngine;
using System.Collections;

public class PlatformDestroy : MonoBehaviour {
	void Start ()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            Destroy(gameObject);
	}
}
