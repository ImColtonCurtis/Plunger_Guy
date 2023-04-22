using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBee : MonoBehaviour
{
    [SerializeField] GameObject wholeObj;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(wholeObj);
        }
    }
}
