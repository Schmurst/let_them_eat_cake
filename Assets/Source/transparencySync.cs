using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transparencySync : MonoBehaviour
{
    public static int  PosID = Shader.PropertyToID("_PlayerPosition");
    public static int  SizeID = Shader.PropertyToID("_Size");
    public Material WallMaterial;
    public Camera Camera;
    public LayerMask Mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            WallMaterial.SetFloat(SizeID, 1);
        }
        else { WallMaterial.SetFloat(SizeID, 1); }

        var view = Camera.WorldToViewportPoint(transform.position);
        WallMaterial.SetVector(PosID,view);
    }
}
