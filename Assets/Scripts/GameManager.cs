using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public static GameManager GM;

    // Second and Third game modes are locked at game start
    public Button staffButton; 
    private bool staffIsUnlocked = false;
    public Button guestButton;
    private bool guestIsUnlocked = false;

    private string currCharacter;
    public PlayerMovement Player;
    public Slider PlayerAwareness;
    //public CanvasBehavior CV;
    //public int maxLevels;
    private bool isGameOver;

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;

            // DEBUG TMP:
            if (SceneManager.GetActiveScene().name == "GameStart")
            {
                // AFTER DEBUG TMP, KEEP BUT REMOVE FROM IF GAME START STATEMENT
                if (!staffIsUnlocked)
                {
                    staffButton.interactable = false;
                }
                if (!guestIsUnlocked)
                {
                    guestButton.interactable = false;
                }
            }
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // Persist between levels
    }

    // Use this for initialization
    void Start()
    {
        //Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        //Player.setSaves(savesLeft);
        //CV = GameObject.FindGameObjectWithTag("UI").GetComponent<CanvasBehavior>();
        //CV.setSaveText(savesLeft);
        isGameOver = false;
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
            //if (CV == null)
            //{
            //    CV = GameObject.FindGameObjectWithTag("UI").GetComponent<CanvasBehavior>();
            //}
        }
        //else
        //{
        //    // Restart game
        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        SceneManager.LoadScene("GameStart");
        //        isGameOver = false;
        //    }
        //}

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

    private void FixedUpdate()
    {
        //if (!isGameOver)
        //{
        //    // Get input for player movement
        //    float xMove = 0;
        //    if ((xMove = Input.GetAxis("Horizontal")) != 0)
        //    {
        //        Player.movePlayer(xMove);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Space) && !Player.getIsJumping())
        //    {
        //        Player.jump();
        //    }

        //    // Help player death save text
        //    if (Player != null && CV != null &&
        //        Player.isAirWarning &&
        //        Player.getIsUncovered() &&
        //    {
        //        CV.showSaveHelpText();
        //    }
        //    else if (CV != null)
        //    {
        //        CV.hideSaveHelpText();
        //    }
        //}
    }

    // Load Level
    // string playerCharacter: identifier for game mode between host, staff, and guest
    public void loadNextLevel(string playerCharacter)
    {
        currCharacter = playerCharacter;

        Debug.Log("loading next level for: " + playerCharacter);

        // If current scene is game start, get chosen character and start corresponding first level
        if (SceneManager.GetActiveScene().name == "GameStart")
        {
            SceneManager.LoadScene(currCharacter + "_Level" + 1);
        }
        // If not game start screen, load next level for current character
        else
        {
            // Get current scene name
            string nextLevel = SceneManager.GetActiveScene().name;
            // Locate current level number
            char currLevel = nextLevel[currCharacter.Length + 6]; // add length of "_Level" to the length of the current player
            // Cast level number to integer
            int lev = (int)char.GetNumericValue(currLevel) + 1;

            // If last level, load game over scene
            //if (lev > maxLevels)
            //{
            //    nextLevel = "GameOver";
            //    isGameOver = true;

            //    // Destroy in game UI when moving to Game Over screen
            //    GameObject canv = GameObject.FindGameObjectWithTag("UI");
            //    if (canv != null)
            //    {
            //        Destroy(canv);
            //    }
            //}
            // Load next level
            //else
            //{
                // Build string for next level
                nextLevel = currCharacter + "_" + lev.ToString();
            //}

            // Load level
            SceneManager.LoadScene(nextLevel);
        }
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
        PlayerAwareness.value = currAwareness;
    }

}