using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float speed = 200f;
    Vector3 upperRight;
    Vector3 lowerLeft;

    void Awake()
    {
        lowerLeft  = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
        upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += - transform.up * speed * Time.deltaTime;

        if (transform.position.x < lowerLeft.x  || transform.position.y < lowerLeft.y ||
            transform.position.x > upperRight.x || transform.position.y > upperRight.y)
        {
            Destroy(gameObject);
        }
    }

}
