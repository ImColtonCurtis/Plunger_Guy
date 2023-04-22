using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotation : MonoBehaviour
{
    Transform myTransform;
    float turnSpeed = 0;
    [SerializeField] GameObject wholeObj;

    // Start is called before the first frame update
    void OnEnable()
    {
        myTransform = transform;
        if (Random.Range(0, 5) >= 2 && !GameManager.startingBlocks)
            turnSpeed = Random.Range(1.5f, 4f);
        if (Random.Range(0, 2) == 0)
            turnSpeed *= -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myTransform.localEulerAngles += new Vector3(0, turnSpeed, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(wholeObj);
        }
    }
}
