using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 followOffset;

    //[SerializeField]
    //float lookAheadDst = 10;
    [SerializeField]
    float smoothTime = 0.05f;
    //[SerializeField]
    //float rotSmoothSpeed = 3;

    [SerializeField]
    Camera myCam;

    Vector3 smoothV = Vector3.zero;

    float counter = 0, startingFOV;
    Transform myTransform;

    private void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = target.position + followOffset;
        if (!GameManager.levelFailed)
            myTransform.localPosition = Vector3.SmoothDamp(myTransform.localPosition, targetPos, ref smoothV, smoothTime);

        if (ControlsLogic.touchedDown)
        {
            if (myCam.fieldOfView < 132.5f && ControlsLogic.isGrowing)
                myCam.fieldOfView += 0.31f * ControlsLogic.growthSpeedRamp; // 82 is default
            startingFOV = myCam.fieldOfView;
            counter = 0;
        }
        else if (!GameManager.levelFailed)
        {
            if (myCam.fieldOfView >= 82 && GameManager.levelStarted) // 9 frames
            {
                counter++;
                myCam.fieldOfView = Mathf.Lerp(startingFOV, 82f, counter / 9.0f);                
            }                
        }        
    }
}
