using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isOn = false;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isOn){
            spriteRenderer.color = Color.red;
        } else {
            spriteRenderer.color = Color.green;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isOn && other.CompareTag("Player")){
            other.GetComponent<Living>().Death(gameObject);   
        }
    }
}
