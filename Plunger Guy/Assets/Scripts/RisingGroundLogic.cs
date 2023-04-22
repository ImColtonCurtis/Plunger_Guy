using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingGroundLogic : MonoBehaviour
{

    Transform myTransform;
    float riseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        riseSpeed = 0.02f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.levelStarted && !GameManager.levelFailed)
        {
            riseSpeed = (GameManager.currentScore / 20)*(0.0575f-0.02f)+0.02f;
            myTransform.localPosition += new Vector3(0, riseSpeed, 0);
        }
    }
}
