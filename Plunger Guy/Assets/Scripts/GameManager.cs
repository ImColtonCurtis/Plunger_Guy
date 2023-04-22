using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool levelStarted, levelFailed, retryTapped;

    [SerializeField] GameObject bee;
    [SerializeField] GameObject[] levelObjects;
    [SerializeField] Transform spawnFolder;

    [SerializeField] SpriteRenderer maintTitle, mainTitleBG, maintTitle2, mainTitleBG2, instructionsTitle, instructionsBG, whiteSquare, highscoreText, highscoreTextBG, soundIcon, soundIconBG, noIcon, noIconBG;

    [SerializeField] Transform[] retryTexts;

    bool restartLogic, startLogic, retryLogic;

    [SerializeField] TextMeshPro playerScore;
    public static TextMeshPro staticPlayerScore;
    public static int currentScore ;

    [SerializeField] BoxCollider plungerCol;
    [SerializeField] Rigidbody myRB;

    [SerializeField] GameObject realPlunger, animatedPlunger;
    [SerializeField] MeshRenderer charMesh, eyeMesh, stickMesh;

    public static bool shouldSpawn;

    [SerializeField] AudioSource losingSound;

    // level spawner literature
    float spawnPosition;
    int spawnInt;

    public static bool startingBlocks;

    [SerializeField] AudioSource mainMenuMusic;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        levelStarted = false;
        levelFailed = false;

        restartLogic = false;
        startLogic = false;

        retryTapped = false;
        retryLogic = false;

        currentScore = 0;
        staticPlayerScore = playerScore;
        
        playerScore.text = PlayerPrefs.GetInt("highScore", 0) + "";
    }
    
    private void Start()
    {
        shouldSpawn = false;

        // starting blocks
        spawnPosition = Random.Range(1.5f, 2.5f);
        startingBlocks = true;
        SpawnBlock();
        SpawnBlock();
        startingBlocks = false;
        SpawnBlock();

        StartCoroutine(StartLogic());
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

    void SpawnBlock()
    {
        GameObject tempBee;
        if (Random.Range(0, 9) == 0 && !startingBlocks)
        {
            tempBee = Instantiate(bee, spawnFolder);
            tempBee.transform.localPosition = new Vector3(2, spawnPosition, 7.93f);
        }


        int tempInt = currentScore+3;

        if (tempInt < 100)
            spawnInt = tempInt / 10;
        else
        {
            while (tempInt > 100)
                tempInt -= 100;
            spawnInt = tempInt / 10;
        }
        GameObject tempObj = Instantiate(levelObjects[spawnInt], spawnFolder);
        tempObj.transform.localPosition = new Vector3(0, spawnPosition, 0);
        spawnPosition += Random.Range(5, 7.5f);
    }

    private void Update()
    {
        if (retryTapped && !retryLogic)
        {
            StartCoroutine(RestartWait());
            retryLogic = true;
        }

        if (shouldSpawn)
        {
            SpawnBlock();
            shouldSpawn = false;
        }

        if (!restartLogic && levelFailed)
        {
            losingSound.Play();

            if (currentScore > PlayerPrefs.GetInt("highScore", 0))
                PlayerPrefs.SetInt("highScore", currentScore);

            StartCoroutine(ScreenFlash());



            Transform tempObj = retryTexts[Random.Range(0, retryTexts.Length)].transform;
            SpriteRenderer retryTitle, retryBg;
            retryTitle = tempObj.GetComponent<SpriteRenderer>();
            retryBg = tempObj.GetComponentsInChildren<SpriteRenderer>()[1];

            StartCoroutine(RetryLiterature(retryTitle, retryBg));
            
            ControlsLogic.touchedDown = false;

            restartLogic = true;
        }

        if (!startLogic && levelStarted)
        {
            playerScore.text = currentScore + "";

            realPlunger.SetActive(true);
            animatedPlunger.SetActive(false);
            charMesh.enabled = true;
            eyeMesh.enabled = true;
            stickMesh.enabled = true;

            StartCoroutine(FadeImageOut(highscoreText));
            StartCoroutine(FadeImageOut(highscoreTextBG));

            StartCoroutine(FadeOutAudio(mainMenuMusic));

            StartCoroutine(FadeImageOut(maintTitle));
            StartCoroutine(FadeImageOut(mainTitleBG));
            StartCoroutine(FadeImageOut(maintTitle2));
            StartCoroutine(FadeImageOut(mainTitleBG2));
            StartCoroutine(FadeImageOut(instructionsTitle));
            StartCoroutine(FadeImageOut(instructionsBG));

            StartCoroutine(FadeImageOut(soundIcon));
            StartCoroutine(FadeImageOut(noIcon));

            StartCoroutine(FadeImageOut(soundIconBG));
            StartCoroutine(FadeImageOut(noIconBG));

            startLogic = true;
        }
    }

    IEnumerator ScreenFlash()
    {    
        myRB.constraints = RigidbodyConstraints.None;
        // add spin
        myRB.AddTorque(new Vector3(Random.Range(10, 30), Random.Range(10, 25), Random.Range(10, 25)), ForceMode.Impulse);
        myRB.AddForce(new Vector3(0, Random.Range(20, 30), 0), ForceMode.Impulse);
        myRB.useGravity = true;

        plungerCol.enabled = false;
        whiteSquare.enabled = true;
        whiteSquare.color = Color.white;
        yield return new WaitForSeconds(0.06f);
        whiteSquare.enabled = false;
    }

    IEnumerator StartLogic()
    {
        whiteSquare.enabled = true;
        whiteSquare.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FadeImageOut(whiteSquare));
    }

    IEnumerator RetryLiterature(SpriteRenderer mainText, SpriteRenderer bgText)
    {
        float timer = 0, totalTime = 40;
        Color startingColor1 = mainText.color;
        Color startingColor2 = bgText.color;
        Transform textTransform = mainText.gameObject.transform.parent.transform;

        Vector3 startingScale = textTransform.localScale;

        while (timer <= totalTime)
        {
            if (timer <= 18)
                textTransform.localScale = Vector3.Lerp(startingScale * 0.1f, startingScale * 1.65f, timer / (totalTime - 18));

            if (timer < totalTime * 0.75f)
            {
                mainText.color = Color.Lerp(startingColor1, new Color(startingColor1.r, startingColor1.g, startingColor1.b, 1), timer / (totalTime * 0.7f));
                bgText.color = Color.Lerp(startingColor2, new Color(startingColor2.r, startingColor2.g, startingColor2.b, 1), timer / (totalTime * 0.7f));
            }

            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator RestartWait()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(RestartLevel(whiteSquare));
    }

    IEnumerator RestartLevel(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    IEnumerator FadeImageOut(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        myImage.enabled = false;
    }

    IEnumerator FadeImageIn(SpriteRenderer myImage, float totalTime)
    {
        float timer = 0;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextOut(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextIn(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }
}
