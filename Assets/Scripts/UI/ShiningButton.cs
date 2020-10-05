using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningButton : MonoBehaviour
{
    private float SHINING_SPEED = 0.7f;
    private float MIN_OPACITY = 0.15f;
    private float MAX_OPACITY = 0.9f;

    private SpriteRenderer sprite;

    private bool goingDown = false;

    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float amount = SHINING_SPEED * Time.deltaTime;
        if (!sprite.enabled || goingDown) {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Max(sprite.color.a - amount, MIN_OPACITY));
            if (sprite.enabled && sprite.color.a == MIN_OPACITY)
                goingDown = false;
        } else {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Min(sprite.color.a + amount, MAX_OPACITY));
            if (sprite.color.a == MAX_OPACITY)
                goingDown = true;
        }
    }
}
