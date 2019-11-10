using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    public Vector3 offset;
    public float smoothSpeed;
    private PixelPerfectCamera thisCamera;

    void Start()
    {
        player = FindObjectOfType<Player>().GetComponent<Transform>();
        thisCamera = GetComponent<PixelPerfectCamera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            thisCamera.assetsPPU++;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
            {
                thisCamera.assetsPPU--;
            }
        }
        Vector3 desiredPos = player.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;


    }
}
