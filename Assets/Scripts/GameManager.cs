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

    public static int MAX_LIVES = 9;
    public int LIFE_REGEN_RATE = 20;
    private bool lifeRegenIsActive = false;

    // HOST Game Mode 
    private Canvas HostCanvas;
    private Slider PlayerAwareness;
    private Button hostSaveButton1;
    private Button hostSaveButton2;
    private Button hostSaveButton3;
    private Text hostLivesText;
    private Text hostBribesText;
    private Text hostPersuadesText;
    private int hostLivesLeft = MAX_LIVES;
    private int hostBribesLeft = 8;
    private int hostPersuadesLeft = 10;
    public int HOST_LIVES_INDEX = 0;
    public int HOST_PERSUADES_INDEX = 1;
    public int HOST_BRIBES_INDEX = 2;


    public int [] maxLevels;
    private bool isGameOver;
    private int lev;

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

        initUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Life regeneration over time
        if (!lifeRegenIsActive && hostLivesLeft < MAX_LIVES)
        {
            lifeRegenIsActive = true;
            StartCoroutine(regenLives());
        }

        // Player is playing in one of three game modes
        if (!isGameOver && SceneManager.GetActiveScene().name != "GameStart")
        {
            // New Level Loaded
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            }
            // Get appropriate game mode UI and player references
            if (currCharacter == "Host" && HostCanvas == null)
            {
                initHostGame();
            }
            else if (currCharacter == "Staff")
            {

            }
            else if (currCharacter == "Guest") ;

            if (!Player.getIsPaused())
            {
                // Get input for player movement
                float xMove = 0;
                float yMove = 0;

                if ((yMove = Input.GetAxis("Vertical")) != 0)
                {
                    //  Debug.Log("y movement: " + Input.GetAxis("Vertical"));
                    Player.movePlayer(0, yMove);
                }
                if ((xMove = Input.GetAxis("Horizontal")) != 0)
                {
                    // Debug.Log("x movement: " + Input.GetAxis("Horizontal"));
                    Player.movePlayer(xMove, 0);

                }
            }
        }
        else
        {
            // Restart game
            if (Input.GetKeyDown(KeyCode.R))
            {
                restartGame();
            }
            if (CV == null)
            {
                CV = GameObject.FindGameObjectWithTag("UI").GetComponent<StartCanvasBehavior>();
            }
        }
    }

    // Load game start scene
    public void restartGame()
    {
        SceneManager.LoadScene("GameStart");
        isGameOver = false;
        CV.GetComponent<Canvas>().enabled = true;

        //Debug.Log("staffB: " + staffIsUnlocked);
        //Debug.Log("guestB: " + guestIsUnlocked);

        initUI();
    }

    public void initUI()
    {
        staffButton.interactable = staffIsUnlocked;
        guestButton.interactable = guestIsUnlocked;

        hostLivesText = GameObject.FindGameObjectWithTag("HostLivesText").GetComponent<Text>();
        hostLivesText.text = hostLivesLeft.ToString();
    }

    // Set player choice for game mode
    public void setGameMode(int mode)
    {
        currGameMode = mode;
    }

    // Load Level, load first level in game mode
    // string playerCharacter: identifier for game mode between host, staff, and guest
    public void loadNextLevel(string playerCharacter)
    {
        if (playerCharacter == "Host" && hostLivesLeft <= 0)
        {
            Debug.Log("no lives left, pay or wait");
        }
        else
        {
            CV.GetComponent<Canvas>().enabled = false;

            currCharacter = playerCharacter;

            lev = 1;    // start level 1
            Debug.Log("loading next level for: " + playerCharacter);
            SceneManager.LoadScene(currCharacter + "_Level" + lev);
        }
    }
    // Load Level, load next level in current mode
    public void loadNextLevel()
    {
        // Get current scene name
        string nextLevel = SceneManager.GetActiveScene().name;
        // Locate current level number
        char currLevel = nextLevel[currCharacter.Length + 6]; // add length of "_Level" to the length of the current player
        // Cast level number to integer
        lev = (int)char.GetNumericValue(currLevel) + 1;

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

    // FOR DEBUG TO ACTIVATE GAME MODE
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

    // Initiate and deactivate buttons choices 
    // on critical threat levels for host
    public void initHostSave()
    {
        hostSaveButton1.image.gameObject.SetActive(true);
        hostSaveButton2.image.gameObject.SetActive(true);
        hostSaveButton3.image.gameObject.SetActive(true);

    }
    public void endHostSave()
    {
        hostSaveButton1.image.gameObject.SetActive(false);
        hostSaveButton2.image.gameObject.SetActive(false);
        hostSaveButton3.image.gameObject.SetActive(false);

    }

    public void decrementHostItem(int index)
    {
        switch(index)
        {
            // Admission, lives
            case 0:
                hostLivesLeft--;

                // If game lost, return to home screen
                if (hostLivesLeft <= 0)
                {
                    restartGame();
                }

                hostLivesText.text = hostLivesLeft.ToString();
                break;
            // Persuasion
            case 1:
                hostPersuadesLeft--;
                hostPersuadesText.text = hostPersuadesLeft.ToString();
                break;
            // Bribe
            case 2:
                hostBribesLeft--;
                hostBribesText.text = hostBribesLeft.ToString();
                break;
        }
    }


    // Initialize UI and appropriate variables for 
    // playing game in host mode
    public void initHostGame()
    {
        HostCanvas = GameObject.FindGameObjectWithTag("Host_HUD").GetComponent<Canvas>();
        PlayerAwareness = GameObject.FindGameObjectWithTag("awarenessUI").GetComponent<Slider>();
        // PlayerAwareness = HostCanvas.GetComponent<Slider>();

        hostSaveButton1 = GameObject.FindGameObjectWithTag("HostSaveButton1").GetComponent<Button>();
        hostSaveButton1.image.gameObject.SetActive(false);
        hostSaveButton2 = GameObject.FindGameObjectWithTag("HostSaveButton2").GetComponent<Button>();
        hostSaveButton2.image.gameObject.SetActive(false);
        hostSaveButton3 = GameObject.FindGameObjectWithTag("HostSaveButton3").GetComponent<Button>();
        hostSaveButton3.image.gameObject.SetActive(false);

        hostLivesText = GameObject.FindGameObjectsWithTag("HostLivesText")[1].GetComponent<Text>();
        hostLivesText.text = hostLivesLeft.ToString();
        hostPersuadesText = GameObject.FindGameObjectWithTag("HostPersText").GetComponent<Text>();
        hostPersuadesText.text = hostPersuadesLeft.ToString();
        hostBribesText = GameObject.FindGameObjectWithTag("HostBribeText").GetComponent<Text>();
        hostBribesText.text = hostBribesLeft.ToString();

        updateAwarenessLevel(0.0f);

        // Send player current level 
        // to set awareness modifier
        Player.setLevelMod(lev);
    }

    public IEnumerator regenLives()
    {
        while (hostLivesLeft < MAX_LIVES)
        {
            yield return new WaitForSeconds(LIFE_REGEN_RATE);
            hostLivesLeft++;
            hostLivesText.text = hostLivesLeft.ToString();
            Debug.Log("regen lives: " + hostLivesLeft);
        }
        Debug.Log("regen DONE: " + hostLivesLeft);
        lifeRegenIsActive = false;
        yield return null;
    }
}