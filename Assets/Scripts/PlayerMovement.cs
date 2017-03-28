﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private GameManager GM;
    private CharacterController controller;

    public float force;
    public float awarenessInterval = 3f;
    private float awarenessLevel;

    // Current zone used to monitor and smoothly 
    // transition between zones when changes
    // made before animation finishes
    private int currAwarenessZone = 0;

    // Use this for initialization
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        awarenessLevel = 0.0f;
        GM.updateAwarenessLevel(awarenessLevel);
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log("currZone: " + currAwarenessZone);
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Renderer>().enabled = true;
        // Based on current security zone, update player awareness
        if (other.tag == "safe")
        {
            //   Debug.Log("safe");
            currAwarenessZone = 0;
            StartCoroutine(influenceAwareness(awarenessLevel, 0.0f, awarenessInterval, 0));
        }
        else if (other.tag == "warning")
        {
            //    Debug.Log("warning");
            currAwarenessZone = 1;
            StartCoroutine(influenceAwareness(awarenessLevel, 0.5f, awarenessInterval, 1));
        }
        else if (other.tag == "alarm")
        {
            //  Debug.Log("alarm");
            currAwarenessZone = 2;
            StartCoroutine(influenceAwareness(awarenessLevel, 1.0f, awarenessInterval, 2));
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

    private IEnumerator influenceAwareness(float start, float goal, float duration, int zone)
    {
        // Note coroutine start time
        float startTime = Time.time;

        // for the duration of the transition
        while (Time.time < startTime + duration)
        {
            // If awareness zone has changed, halt animation
            if (currAwarenessZone != zone)
            {
                // Debug.Log("quitting: " + zone);
                yield break;
            }
            // Smoothly transition between float values over time
            awarenessLevel = Mathf.Lerp(start, goal, (Time.time - startTime) / duration);
            // Visually update awareness slider bar as value transitions
            GM.updateAwarenessLevel(awarenessLevel);
            yield return null;
        }
    }
}
