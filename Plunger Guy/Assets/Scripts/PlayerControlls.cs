using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour
{
    [SerializeField] Animator cameraShake;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !GameManager.levelFailed)
        {
            cameraShake.SetTrigger("shake");
            GameManager.levelFailed = true;
        }
    }
}
