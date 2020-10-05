using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFetcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
        GetComponent<Canvas>().worldCamera = camera;
    }
}
