using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomHome : MonoBehaviour
{
    public float scale;
    public float minScale = 5.0f;
    public float maxScale = 70.0f;

    private float prevScale;
    private CameraStretcher camStretch;

    private void Start()
    {
        // Assuming uniform scaling
        scale = minScale;
        prevScale = transform.localScale.x; ;

        camStretch = Camera.main.GetComponent<CameraStretcher>();
    }

    private void Update()
    {
        if ( Mathf.Abs(scale - prevScale) > Mathf.Epsilon )
        {
            if (scale < minScale || scale > maxScale) return;

            // Update home scale
            prevScale = scale;
            transform.localScale = new Vector3(scale, scale, scale);

            // Snap to ground plane
            float height = GetComponent<Renderer>().bounds.size.y;
            Vector3 pos = transform.position;
            pos.y = height / 2.0f;
            transform.position = pos;

            camStretch.Stretch(scale, pos);
        }
    }
}
