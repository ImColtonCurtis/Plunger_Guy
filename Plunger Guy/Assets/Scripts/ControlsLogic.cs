using System.Collections;
using System.Collections.Generic;
using Unity.Services.Mediation.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsLogic : MonoBehaviour
{
    public static bool touchedDown;

    [SerializeField]
    Transform stick, plunger, actorHolder;

    float counter = 0, fallSpeed, startSize, gravityRamp = 1;
    public static float growthSpeedRamp = 1;
    Vector3 startingScale;

    public bool grounded = false;
    public static bool isGrowing = false;
    bool falling = true;

    Transform childTransform;

    float deltaMoveSpeed = 0, lastPlungerHeadPos = 0;

    bool parentSet = false, preformingFollowThrough = false;
    
    [SerializeField] Animator cameraShake;

    [SerializeField] GameObject noIcon;

    [SerializeField] AudioSource growingSound, plopSound, landingSound;

    void Awake()
    {
        touchedDown = false;
        startSize = stick.localScale.z;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
        {
            noIcon.SetActive(false);
            AudioListener.volume = 1;
        }
        else
        {
            noIcon.SetActive(true);
            AudioListener.volume = 0;
        }
    }

    private void FixedUpdate()
    {
        if (touchedDown && grounded)
        {
            if (stick.localScale.z < 2.8f)
            {
                if (!isGrowing)
                {
                    growingSound.volume = 1;
                    growingSound.pitch = 1;
                    growingSound.pitch += Random.Range(-0.1f, 0.1f);
                    growingSound.Play();
                }

                isGrowing = true;
                growthSpeedRamp += 0.015f;
                stick.localScale += new Vector3(0, 0, 0.02659f * growthSpeedRamp);
                plunger.localPosition += new Vector3(0, 0.1f * growthSpeedRamp, 0);
                counter = 0;
                startingScale = stick.localScale;

                deltaMoveSpeed = plunger.position.y - lastPlungerHeadPos;
                lastPlungerHeadPos = plunger.position.y;
            }
            else
            {
                deltaMoveSpeed = 0;
                lastPlungerHeadPos = 0;
                growthSpeedRamp = 1;
                isGrowing = false;
            }
            falling = false;
        }
        else
        {
            if (parentSet && !grounded)
            {
                plunger.SetParent(actorHolder);
                parentSet = false;
            }
            if (isGrowing && !preformingFollowThrough)
            {
                // preform follow through
                StartCoroutine(PlungerHeadFollowThrough());                
                lastPlungerHeadPos = 0;

                growthSpeedRamp = 1;
            }
            if (stick.localScale.z >= startSize && GameManager.levelStarted && counter < 10) // 10 frames
            {
                counter++;
                stick.localScale = Vector3.Lerp(startingScale, new Vector3(0.100338f, 0.100338f, startSize), counter / 9.0f);
            }
            falling = true;
        }

        // gravity
        if (!grounded && falling && !isGrowing)
        {
            gravityRamp += 0.0778f;
            if (fallSpeed < 0.5f)
                fallSpeed += 0.0045f * gravityRamp;
            plunger.localPosition -= new Vector3(0, fallSpeed, 0);
        }
        else if (grounded) // this gets set over and over again
        {
            fallSpeed = 0.094f;
            gravityRamp = 1;

            // sticky (moving) objects
            if (GroundDetection.stickyObject != null)
            {
                childTransform = GroundDetection.stickyObject.GetComponentsInChildren<Transform>()[0];

                if (!parentSet)
                {
                    parentSet = true;
                    plunger.SetParent(childTransform);
                }
            }
        }
    }

    private void Update()
    {
        if (falling && !GroundDetection.groundHit)
        {
            grounded = false;
        }
        else if (!grounded)
        {
            grounded = true;
            cameraShake.SetTrigger("shake");
        }
    }

    void OnTouchDown(Vector3 point)
    {
        if (ShowAds.poppedUp)
        {
            if (point.x <= 0)
            {
                ShowAds.shouldShowRewardedAd = true;
            }
            else
            {
                ShowAds.dontShow = true;
            }
        }
        else
        {
            if (!GameManager.levelStarted && point.x <= -0.5f && point.y <= 4.5f) // bottom left button clicked
            {
                if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
                {
                    PlayerPrefs.SetInt("SoundStatus", 0);
                    noIcon.SetActive(true);
                    AudioListener.volume = 0;
                }
                else
                {
                    PlayerPrefs.SetInt("SoundStatus", 1);
                    noIcon.SetActive(false);
                    AudioListener.volume = 1;
                }
            }
            else
            {
                if (!GameManager.levelStarted)
                    GameManager.levelStarted = true;

                if (!touchedDown && !GameManager.levelFailed)
                {
                    touchedDown = true;
                }
                if (GameManager.levelFailed && !GameManager.retryTapped)
                {
                    GameManager.retryTapped = true;
                }
            }
        }
    }

    void OnTouchUp()
    {
        if (touchedDown)
        {
            plopSound.volume = 1;
            plopSound.pitch = 1;
            plopSound.pitch += Random.Range(-0.1f, 0.1f);
            plopSound.Play();
            growingSound.Stop();
            FadeOutAudio(growingSound);
            touchedDown = false;
        }
    }
    IEnumerator FadeOutAudio(AudioSource myAudio)
    {
        float timer = 0, totalTime = 24;
        float startingLevel = myAudio.volume;
        while (timer <= totalTime)
        {
            myAudio.volume = Mathf.Lerp(startingLevel, 0, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }


    void OnTouchExit()
    {
        if (touchedDown)
        {
            touchedDown = false;
        }
    }

    IEnumerator PlungerHeadFollowThrough()
    {
        float timer = 0, totalTime = 4;
        float mover;
        preformingFollowThrough = true;
        while (timer <= totalTime)
        {
            mover = Mathf.Lerp(deltaMoveSpeed*7f/9f, 0, timer / totalTime);
            plunger.localPosition += new Vector3(0, mover, 0);
            yield return new WaitForFixedUpdate();
            timer++;
            if (touchedDown)
                break;
        }
        preformingFollowThrough = false;
        isGrowing = false;
        deltaMoveSpeed = 0;
    }
}