using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapColliderDebug : MonoBehaviour
{

[SerializeField]
LayerMask layer;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 mouseWorldPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos,Vector3.forward,50f,layer);
            if(hit)
                Debug.Log(hit.collider.name);
        }
    }
}
