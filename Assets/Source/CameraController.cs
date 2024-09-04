using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform t1;
    public Transform t2;
    public Camera cam;
    public float zoomFactor = 1.5f;
    public float followTimeDelta = 0.8f;
    public float minZoom = 10f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FixedCameraFollowSmooth();
    }
    // Follow Two Transforms with a Fixed-Orientation Camera
    public void FixedCameraFollowSmooth()
    {
        // How many units should we keep from the players
        

        // Midpoint we're after
        Vector3 midpoint = (t1.position + t2.position) / 2f;

        character p1 = t1.GetComponent<character>();
        character p2 = t2.GetComponent<character>();

        if (p1.State == CharState.dying)
            midpoint = t2.position;
        if(p2.State == CharState.dying)
            midpoint = t1.position;


        // Distance between objects
        float distance = (t1.position - t2.position).magnitude;
        distance = Mathf.Max(distance, minZoom);

        // Move camera a certain distance
        Vector3 cameraDestination = midpoint - cam.transform.forward * distance * zoomFactor;

        // Adjust ortho size if we're using one of those
        if (cam.orthographic)
        {
            // The camera's forward vector is irrelevant, only this size will matter
            cam.orthographicSize = distance;
        }

        // You specified to use MoveTowards instead of Slerp
        cam.transform.position = Vector3.Slerp(cam.transform.position, cameraDestination, followTimeDelta);

        // Snap when close enough to prevent annoying slerp behavior
        if ((cameraDestination - cam.transform.position).magnitude <= 0.05f)
            cam.transform.position = cameraDestination;
    }
}
