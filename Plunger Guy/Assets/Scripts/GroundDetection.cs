using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public static bool groundHit = false;
    public static Transform stickyObject;

    [SerializeField] Animator scoreBump;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            groundHit = true;
            stickyObject = other.gameObject.transform;
        }

        if (other.tag == "Point")
        {
            GameManager.shouldSpawn = true;

            scoreBump.SetTrigger("Bump");

            GameManager.currentScore += 1;
            StartCoroutine(WaitABit());

            Destroy(other.transform.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            groundHit = false;
            stickyObject = null;
        }
    }

    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(1f/6f);
        GameManager.staticPlayerScore.text = GameManager.currentScore + "";
    }
}
