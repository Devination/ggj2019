using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStretcher : MonoBehaviour
{
    private Camera mainCamera;
    private float minScale = 5.0f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void Stretch( float homeScale, Vector3 homePos )
    {
        mainCamera.orthographicSize = minScale + homeScale;
        mainCamera.gameObject.transform.LookAt(homePos);
    }
}
