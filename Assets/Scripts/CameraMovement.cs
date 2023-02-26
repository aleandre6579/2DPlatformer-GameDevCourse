using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private GameObject playerRef;

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = playerRef.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, smoothTime);
    }
}
