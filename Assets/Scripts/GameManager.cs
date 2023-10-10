using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager singleton;

    // ADD SOUNDS
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip levelCompleteSound;

    private GroundPiece[] allGroundPieces;
    void Start()
    {
        SetUpNewLevel();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }


    private void SetUpNewLevel()
    {
        allGroundPieces = FindObjectsOfType<GroundPiece>();
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (singleton == null)
        {
            singleton = this;
        }else if(singleton != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetUpNewLevel();
    }

    public void CheckComplete()
    {
        bool isFinished = true;
        for (int i = 0; i < allGroundPieces.Length; i++)
        {
            if (allGroundPieces[i].isColored == false)
            {
                isFinished = false;
                break;
            }

        }
        if (isFinished)
        {
            NextLevel();
        }
            
    }
    
    private void NextLevel()
    {
        // Get the next scene index
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        audioSource.PlayOneShot(levelCompleteSound);
        // Check if there's a next scene available
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Last level reached. Implement a game end or loop back to start.");
            // For example, loop back to the start:
            SceneManager.LoadScene(0);
        }
    }

}
