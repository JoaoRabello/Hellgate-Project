using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    private Vector2 _direction;
    public Vector2 Direction {set => _direction = value;}

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * 3f * Time.deltaTime);
    }
}
