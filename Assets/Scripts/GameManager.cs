using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager GM;
    private string currCharacter;
    //public PlayerMovement Player;
    //public CanvasBehavior CV;
    //public int maxLevels;
    //private bool isGameOver;

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
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
        //isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isGameOver)
        //{
        //    // New Level Loaded
        //    if (Player == null)
        //    {
        //        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        //    }
        //    if (CV == null)
        //    {
        //        CV = GameObject.FindGameObjectWithTag("UI").GetComponent<CanvasBehavior>();
        //    }
        //}
        //else
        //{
        //    // Restart game
        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        SceneManager.LoadScene("GameStart");
        //        isGameOver = false;
        //    }
        //}
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

        Debug.Log("loading next level for: " + playerCharacter);

        // If current scene is game start, get chosen character and start corresponding first level
        if (SceneManager.GetActiveScene().name == "GameStart")
        {
            SceneManager.LoadScene(playerCharacter + "_Level" + 1);
        }
        // If not game start screen, load next level for current character
        else
        {
            // Get current scene name
            string nextLevel = SceneManager.GetActiveScene().name;
            // Locate current level number
            char currLevel = nextLevel[playerCharacter.Length + 6]; // add length of "_Level" to the length of the current player
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
                nextLevel = playerCharacter + "_" + lev.ToString();
            //}

            // Load level
            SceneManager.LoadScene(nextLevel);
        }
    }

}