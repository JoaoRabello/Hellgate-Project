using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    public Vector3 offset;
    public float smoothSpeed;

    void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 desiredPos = player.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }
}
