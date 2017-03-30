using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GameManager : MonoBehaviour
{
    // Game Manager and Canvas references
    public static GameManager GM;
    public StartCanvasBehavior CV;
    public HUDCanvasBehavior HUD;


    // Second and Third game modes are locked at game start
    public Button staffButton; 
    private bool staffIsUnlocked;
    public Button guestButton;
    private bool guestIsUnlocked;

    // Player game mode and character references
    private string currCharacter;
    private int currGameMode;
    public PlayerMovement Player;

    // Player lives and timer for regeneration
    public static int MAX_LIVES = 9;
    public int LIFE_REGEN_RATE = 20;
    private bool lifeRegenIsActive = false;


    // Daily prize regeneration
    public int PRIZE_REGEN_RATE = 80;
    private bool isPrizeAvailable = true;

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

    // Max game levels for each game mode
    // host, staff, and guest respectively
    public int [] maxLevels;

    // Game state
    private bool isGameOver;
    private int lev;

    // Player levels for each game mode
    private int curHostLevel;
    private int curStaffLevel;
    private int curGuestLevel;

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
        HUD = GameObject.FindGameObjectWithTag("UI_HUD").GetComponent<HUDCanvasBehavior>();
        staffIsUnlocked = false;
        guestIsUnlocked = false;

        initUI();

        // Init player progress through levels
        curHostLevel = 1;
        curStaffLevel = 1;
        curGuestLevel = 1;
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
        if (!isGameOver && 
            SceneManager.GetActiveScene().name != "GameStart" && 
            SceneManager.GetActiveScene().name != "GameStore")
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
            else if (currCharacter == "Guest")
            {

            }

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
            // TMP: shouldn't end up needing this
            //if (CV == null)
            //{
            //    Debug.Log("CV reref");
            //    CV = GameObject.FindGameObjectWithTag("UI").GetComponent<StartCanvasBehavior>();
            //}
            //if(HUD == null)
            //{
            //    Debug.Log("HUD reref");
            //    HUD = GameObject.FindGameObjectWithTag("UI_HUD").GetComponent<HUDCanvasBehavior>();
            //}
        }
    }

    // Load game start scene
    public void restartGame()
    {
        SceneManager.LoadScene("GameStart");
        isGameOver = false;
        CV.GetComponent<Canvas>().enabled = true;

        initUI();
    }

    // Initialize navigation and resource UI 
    // constant throughout all game modes
    public void initUI()
    {
        // Scene navigation
        staffButton.interactable = staffIsUnlocked;
        guestButton.interactable = guestIsUnlocked;

        // Player resources
        hostLivesText = GameObject.FindGameObjectWithTag("HostLivesText").GetComponent<Text>();
        hostPersuadesText = GameObject.FindGameObjectWithTag("HostPersText").GetComponent<Text>();
        hostBribesText = GameObject.FindGameObjectWithTag("HostBribeText").GetComponent<Text>();

        // Display current resource values
        refreshHUDValues();
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
        // Send to store if no lives remaining
        if (hostLivesLeft <= 0)
        {
            //Debug.Log("no lives left, pay or wait");
            loadStore();
        }
        else
        {
            HUD.loadHomeButton();
            CV.GetComponent<Canvas>().enabled = false;

            currCharacter = playerCharacter;

            // Default, start level 1
            lev = 1;

            // Load current level for chosen game mode
            switch(currCharacter)
            {
                case "Host":
                    lev = curHostLevel;
                    break;
                case "Staff":
                    lev = curStaffLevel;
                    break;
                case "Guest":
                    lev = curGuestLevel;
                    break;
            }

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

        // If new level in game mode exists
        if (lev <= maxLevels[currGameMode])
        {
            // Save level progress for current game mode
            switch (currCharacter)
            {
                case "Host":
                    curHostLevel = lev;
                    break;
                case "Staff":
                    curStaffLevel = lev;
                    break;
                case "Guest":
                    curGuestLevel = lev;
                    break;
            }

           // Build string for next level 
           nextLevel = currCharacter + "_Level" + lev.ToString();
        }
        //If last level, load game over scene
        else
        {
            nextLevel = "GameOver";
            isGameOver = true;

            if (currGameMode < 2)
            {
                Debug.Log("unlocking mode: " + currGameMode + 1);
                unlockGameMode(currGameMode + 1);
            }
        }
        
        Debug.Log("loading: " + nextLevel); 

        // Load level
        SceneManager.LoadScene(nextLevel);
    }

    // Load game store scene
    public void loadStore()
    {
        CV.GetComponent<Canvas>().enabled = false;
        HUD.loadHomeButton();
        SceneManager.LoadScene("GameStore");
    }

    //TMP: FOR DEBUG TO ACTIVATE GAME MODE
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

    // Adjust player resources
    // int index: which resources is changed
    // int amount: resource quantity changed
    public void decrementHostItem(int index, int amount)
    {
        switch(index)
        {
            // Lives
            case 0:
                hostLivesLeft -= amount;
                // If game lost, return to home screen
                if (hostLivesLeft <= 0)
                {
                    restartGame();
                }
                // If buying lives, limit addition to max
                if(hostLivesLeft > MAX_LIVES)
                {
                    hostLivesLeft = MAX_LIVES;
                }
                break;
            // Persuasion
            case 1:
                hostPersuadesLeft -= amount;
                break;
            // Bribe
            case 2:
                hostBribesLeft -= amount;
                break;
        }
        refreshHUDValues();
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

        updateAwarenessLevel(0.0f);

        // Send player current level 
        // to set awareness modifier
        Player.setLevelMod(lev);
    }

    // Update text values to reflect current values
    public void refreshHUDValues()
    {
        hostLivesText.text = hostLivesLeft.ToString();
        hostPersuadesText.text = hostPersuadesLeft.ToString();
        hostBribesText.text = hostBribesLeft.ToString();
    }

    // Timer for regenerating lives
    public IEnumerator regenLives()
    {
        while (hostLivesLeft < MAX_LIVES)
        {
            yield return new WaitForSeconds(LIFE_REGEN_RATE);
            hostLivesLeft++;
            refreshHUDValues();
            Debug.Log("regen lives: " + hostLivesLeft);
        }
        Debug.Log("regen DONE: " + hostLivesLeft);
        lifeRegenIsActive = false;
        yield return null;
    }

    // Collect prize and initiate timer
    // for prize regeneration
    public void collectPrize()
    {
        hostBribesLeft++;
        hostPersuadesLeft += 3;
        refreshHUDValues();
        StartCoroutine(regenPrize());
    }
    // Timer for prize regeneration
    public IEnumerator regenPrize()
    {
        isPrizeAvailable = false;
        yield return new WaitForSeconds(PRIZE_REGEN_RATE);
        isPrizeAvailable = true;
    }
    // Getter for enabling prize button
    public bool getIsPrizeAvailable()
    {
        return isPrizeAvailable;
    }

}