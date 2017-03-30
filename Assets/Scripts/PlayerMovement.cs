using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Game manager reference
    private GameManager GM;
    // Character controller reference
    private CharacterController controller;

    public GameObject puzzle;

    // Player start location
    private Vector3 startLoc;

    // Player game states
    private bool isDead;
    public bool isPaused;
    private bool hasTempImmunity;
    public int IMMUNITY_TIME;

    // Monitor and apply appropriate 
    // modifiers to awareness level
    // based on current location
    // and current level
    private float alarmMod;
    private float warningMod;
    private float safeMod;
    private float levelMod;

    // Current threat level
    private float awarenessLevel;

    // Movement force applied on input
    public float force;

    // Use this for initialization
    void Start()
    {
        // Get Game Manager reference
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        // Set current threat level
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);

        // Get Player controller reference
        controller = GetComponent<CharacterController>();

        // Store player start location
        startLoc = transform.position;

        // Initialize player game states
        isDead = false;
        isPaused = false;
        hasTempImmunity = false;

        // Location and level modifiers
        // for zone awareness
        alarmMod = 12f;
        warningMod = 4f;
        safeMod = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        //  Debug.Log("currZone: " + currAwarenessZone);

        // Check if threat level results in player death
        if (awarenessLevel >= 0.95f)
        {
            Debug.Log("player death");
            // if dead, reset level
            resetPlayer();
        }
        else if (awarenessLevel >= 0.9f && !isPaused)
        {
            //Debug.Log("threat high, minigame puzzle save init");
            isPaused = true;
            GM.initHostSave();
        }
    }

    // Called once per frame when player overlaps with one or more trigger colliders
    private void OnTriggerStay(Collider other)
    {
        if (!isPaused)
        {
            // Show visualization of current threat zone
            other.gameObject.GetComponent<Renderer>().enabled = true;

            // If threat saved and immunity activated,
            // do not increase threat level temporarily
            if (!hasTempImmunity)
            {

                // Based on current security zone, update player awareness
                if (other.tag == "safe")
                {
                    //Debug.Log("safe");
                    // Update awareness UI slider
                    influenceAwareness(awarenessLevel,
                        0.0f, safeMod * levelMod);
                }
                else if (other.tag == "warning")
                {
                    //Debug.Log("warning");
                    // Update awareness UI slider
                    influenceAwareness(awarenessLevel,
                        1.0f, warningMod * levelMod);
                }
                else if (other.tag == "alarm")
                {
                    //Debug.Log("alarm");
                    // Update awareness UI slider
                    influenceAwareness(awarenessLevel,
                        1.0f, alarmMod * levelMod);
                }
            }
            if (other.tag == "Finish")
            {
                // Level goal reached, 
                // progress through game
                GM.loadNextLevel();
            }
        }
    }

    // Called when player exits trigger collider
    private void OnTriggerExit(Collider other)
    {
        // Hide visualization of zone threat level
        other.gameObject.GetComponent<Renderer>().enabled = false;
    }

    // 2 directional player movement
    public void movePlayer(float dirX, float dirY)
    {
        // Input axis received from GameManager
        Vector3 movement = new Vector3(dirX, dirY, 0.0f);
        // Apply force in all directions with input
        movement *= force;

        // Move player over time
        controller.Move(movement * Time.deltaTime);
    }

    // Reset player to level start after death
    public void resetPlayer()
    {
        // Set player state
        isDead = true;

        // Move player to level start
        transform.position = startLoc;

        // Reset threat level
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);
    }

    // Update UI Slider visualizing awareness threat level
    // Parameters:
    // float start: initial awareness level at function call
    // float goal: end awareness level at lerp completion
    // float duration: time step for lerp
    private void influenceAwareness(float start, float goal, float duration)
    {
        // Note coroutine start time
        float startTime = Time.time;

        // If the player died, reset animation
        if (isDead)
        {
            isDead = false;
        }
        else if (!isPaused)
        {
            // Smoothly transition between float values over time
            awarenessLevel = Mathf.Lerp(start, goal, Time.deltaTime * duration);
            // Visually update awareness slider bar as value transitions
            GM.updateAwarenessLevel(awarenessLevel);
        }
    }
    public void threatResolution(int index)
    {

        switch (index)
        {
            // Admission, lose life reset
            case 0:
                GM.decrementHostItem(GM.HOST_LIVES_INDEX, 1);
                isPaused = false;
                GM.endHostSave();
                resetPlayer();
                break;
            // Persuasion, lose persuade, start minipuzzle
            case 1:
                Debug.Log("persuade chosen");
                GM.decrementHostItem(GM.HOST_PERSUADES_INDEX, 1);
                GM.endHostSave();
                //Debug.Log("starting minigame");

                GameObject puzzleClone = Instantiate(puzzle);

                // isSolvingPuzzle = true;
                // threatSaveSuccessful();
                break;
            // Bribe, lose bribe
            case 2:
                threatSaveSuccessful();
                GM.decrementHostItem(GM.HOST_BRIBES_INDEX, 1);
                break;
        }
    }

    // Called when bribe or persuasion succeeds
    public void threatSaveSuccessful()
    {
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);
        GM.endHostSave();
        isPaused = false;
        StartCoroutine(initTempImmunity());
    }

    // When threat successfully evaded, allow
    // for short grace period to leave alarm area
    public IEnumerator initTempImmunity()
    {
        Debug.Log("start grace period");
        hasTempImmunity = true;
        //TODO: visualization and sound for temp immunity
        yield return new WaitForSeconds(IMMUNITY_TIME);
        hasTempImmunity = false;
        Debug.Log("end grace period");

    }

    // Called when persuasion sequencing puzzle fails
    public void threatSaveFailed()
    {
        GM.decrementHostItem(GM.HOST_LIVES_INDEX, 2);
        resetPlayer();
        isPaused = false;
    }

    // Set the threat time step modifier based on current level
    public void setLevelMod(int mod)
    {
        levelMod = (float) mod * 0.2f;
    }

    // Get/Set player paused game state
    public bool getIsPaused()
    {
        return isPaused;
    }
    public void setIsPaused(bool pause)
    {
        isPaused = pause;
    }
}
