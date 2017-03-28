using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GameManager : MonoBehaviour
{

    public static GameManager GM;
    public StartCanvasBehavior CV;


    // Second and Third game modes are locked at game start
    public Button staffButton; 
    private bool staffIsUnlocked;
    public Button guestButton;
    private bool guestIsUnlocked;

    private string currCharacter;
    private int currGameMode;
    public PlayerMovement Player;
    public Slider PlayerAwareness;
    //public CanvasBehavior CV;
    public int [] maxLevels;
    private bool isGameOver;

    private void Awake()
    {
        Debug.Log("AWAKE");
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }

        // Persist between levels
        DontDestroyOnLoad(gameObject); 
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("START");
        isGameOver = false;
        CV = GameObject.FindGameObjectWithTag("UI").GetComponent<StartCanvasBehavior>();
        staffIsUnlocked = false;
        guestIsUnlocked = false;

        staffButton.interactable = staffIsUnlocked;
        guestButton.interactable = guestIsUnlocked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver && SceneManager.GetActiveScene().name != "GameStart")
        {
            // New Level Loaded
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            }
            // Get appropriate game mode UI and player references
            if (currCharacter == "Host" && PlayerAwareness == null)
            {
                PlayerAwareness = GameObject.FindGameObjectWithTag("awarenessUI").GetComponent<Slider>();
                updateAwarenessLevel(0.0f);
            }
            // Get input for player movement
            float xMove = 0;
            float yMove = 0;

            if ((yMove = Input.GetAxis("Vertical")) != 0)
            {
              //  Debug.Log("y movement: " + Input.GetAxis("Vertical"));
                Player.movePlayer(0, yMove);
            }
            if ((xMove = Input.GetAxis("Horizontal")) != 0) { 
               // Debug.Log("x movement: " + Input.GetAxis("Horizontal"));
                Player.movePlayer(xMove, 0);

            }
        }
        else
        {
            // Restart game
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("GameStart");
                isGameOver = false;
                CV.GetComponent<Canvas>().enabled = true;

                //Debug.Log("staffB: " + staffIsUnlocked);
                //Debug.Log("guestB: " + guestIsUnlocked);

                staffButton.interactable = staffIsUnlocked;
                guestButton.interactable = guestIsUnlocked;
            }
            if (CV == null)
            {
                CV = GameObject.FindGameObjectWithTag("UI").GetComponent<StartCanvasBehavior>();
            }
        }

    }

    private void FixedUpdate()
    {
    }

    // Set player choice for game mode
    public void setGameMode(int mode)
    {
        currGameMode = mode;
    }

    // Load Level
    // string playerCharacter: identifier for game mode between host, staff, and guest
    public void loadNextLevel(string playerCharacter)
    {
        CV.GetComponent<Canvas>().enabled = false;

        currCharacter = playerCharacter;

        Debug.Log("loading next level for: " + playerCharacter);
        SceneManager.LoadScene(currCharacter + "_Level" + 1);
    }
    // Load Level
    public void loadNextLevel()
    {
        // Get current scene name
        string nextLevel = SceneManager.GetActiveScene().name;
        // Locate current level number
        char currLevel = nextLevel[currCharacter.Length + 6]; // add length of "_Level" to the length of the current player
        // Cast level number to integer
        int lev = (int)char.GetNumericValue(currLevel) + 1;

        //If last level, load game over scene
        if (lev > maxLevels[currGameMode])
        {
            nextLevel = "GameOver";
            isGameOver = true;

            if (currGameMode < 2)
            {
                Debug.Log("unlocking mode: " + currGameMode + 1);
                unlockGameMode(currGameMode + 1);
            }
        }
        // Load next level
        else
        {
           // Build string for next level 
           nextLevel = currCharacter + "_Level" + lev.ToString();
        }

        Debug.Log("loading: " + nextLevel); 

        // Load level
        SceneManager.LoadScene(nextLevel);
    }

    public void toggleGameModeEnabled(int mode) 
    {
        switch (mode)
        {
            case 1:
                staffIsUnlocked = !staffIsUnlocked;
                staffButton.interactable = staffIsUnlocked;
                break;
            case 2:
                guestIsUnlocked = !guestIsUnlocked;
                guestButton.interactable = guestIsUnlocked;
                break;
        }
    }

    public void updateAwarenessLevel(float currAwareness)
    {
        if (PlayerAwareness != null)
        {
            // Set slider level between 0 and 1
            PlayerAwareness.value = currAwareness;

            // Transition color from green to red as awareness increases
            PlayerAwareness.fillRect.GetComponent<Image>().color = 
                Color.Lerp(Color.green, Color.red, currAwareness / 1.0f);
        }
    }

    public void unlockGameMode(int mode)
    {
        switch(mode)
        {
            case 1:
                staffIsUnlocked = true;
                break;
            case 2:
                guestIsUnlocked = true;
                break;
        }
    }

}