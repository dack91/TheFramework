using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameManager GM;
    private CharacterController controller;

    private Vector3 startLoc;
    private bool isDead;

    // Monitor and apply appropriate 
    // modifiers to awareness level
    // based on current location
    private float alarmMod;
    private float warningMod;
    private float safeMod;
    private float levelMod;

    private float awarenessLevel;

    // Current zone used to monitor and smoothly 
    // transition between zones when changes
    // made before animation finishes
    private int currAwarenessZone = 0;

    // Movement force applied on input
    public float force;


    // Use this for initialization
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);
        controller = GetComponent<CharacterController>();
        startLoc = transform.position;
        isDead = false;

        // Location and level modifiers
        // for zone awareness
        alarmMod = 12f;
        warningMod = 4f;
        safeMod = 2f;
       // levelMod = 0.1f;   
    }

    // Update is called once per frame
    void Update()
    {
        //  Debug.Log("currZone: " + currAwarenessZone);
        if (awarenessLevel >= 0.95f)
        {
            Debug.Log("player death");
            resetPlayer();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("lev mod: " + levelMod);
        other.gameObject.GetComponent<Renderer>().enabled = true;
        // Based on current security zone, update player awareness
        if (other.tag == "safe")
        {
            //Debug.Log("safe");
            currAwarenessZone = 0;
            influenceAwareness(awarenessLevel, 
                0.0f, safeMod * levelMod, 0);
        }
        else if (other.tag == "warning")
        {
            //Debug.Log("warning");
            currAwarenessZone = 1;
            influenceAwareness(awarenessLevel, 
                1.0f, warningMod * levelMod, 1);
        }
        else if (other.tag == "alarm")
        {
            //Debug.Log("alarm");
            currAwarenessZone = 2;
            influenceAwareness(awarenessLevel, 
                1.0f, alarmMod * levelMod, 2);
        }
        else if (other.tag == "Finish")
        {
            GM.loadNextLevel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<Renderer>().enabled = false;
    }

    // 2 directional player movement
    public void movePlayer(float dirX, float dirY)
    {
        Vector3 movement = new Vector3(dirX, dirY, 0.0f);
        movement *= force;

        controller.Move(movement * Time.deltaTime);
    }

    // Reset player to level start after death
    public void resetPlayer()
    {
        isDead = true;
        transform.position = startLoc;
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);
    }

    private void influenceAwareness(float start, float goal, float duration, int zone)
    {
        // Note coroutine start time
        float startTime = Time.time;

        // If awareness zone has changed, halt animation
        if (currAwarenessZone != zone)
        {
            Debug.Log("quitting: " + zone);
        }
        // If the player died, reset animation
        else if (isDead)
        {
            isDead = false;
        }
        else
        {
            // Smoothly transition between float values over time
            awarenessLevel = Mathf.Lerp(start, goal, Time.deltaTime * duration);
            // Visually update awareness slider bar as value transitions
            GM.updateAwarenessLevel(awarenessLevel);
        }
    }

    public void setLevelMod(int mod)
    {
        levelMod = (float) mod * 0.2f;
    }
}
