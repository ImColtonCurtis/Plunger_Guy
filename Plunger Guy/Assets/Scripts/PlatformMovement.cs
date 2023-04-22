using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    Transform myTransform;
    float moveSpeed;
    bool goingLeft;
    [SerializeField] float[] limits;
    float limit;

    [SerializeField] bool isBee;

    // Start is called before the first frame update
    void OnEnable()
    {
        myTransform = transform;
        if (!GameManager.startingBlocks)
        {
            if (Random.Range(0, 3) >= 1 || isBee)
                moveSpeed = Random.Range(0.02f, 0.09f);
        }
        else
        {
            if (Random.Range(0, 3) >= 1)
                moveSpeed = Random.Range(0.0125f, 0.03f);
        }

        goingLeft = true;
        moveSpeed *= -1;

        limit = Random.Range(limits[0], limits[1]);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myTransform.localPosition += new Vector3(moveSpeed, 0, 0);

        if (myTransform.localPosition.x <= -limit && goingLeft)
        {
            goingLeft = false;
            StartCoroutine(TurnBee(true));
        }
        else if (myTransform.localPosition.x >= limit && !goingLeft)
        {
            goingLeft = true;
            StartCoroutine(TurnBee(false));
        }
    }

    IEnumerator TurnBee(bool turnLeft)
    {
        float timer = 0, totalTime = Random.Range(24, 54);

        Vector3 startingVector = new Vector3(0, -90, 0);
        Vector3 endVector = new Vector3(0, 90, 0);

        float startingSpeed = moveSpeed;

        while (timer <= totalTime)
        {
            moveSpeed = Mathf.Lerp(startingSpeed, -startingSpeed, timer / totalTime);

            if (isBee)
            {
                if (turnLeft)
                    myTransform.localEulerAngles = Vector3.Lerp(startingVector, endVector, timer / totalTime);
                else
                    myTransform.localEulerAngles = Vector3.Lerp(endVector, startingVector, timer / totalTime);
            }
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }
}
