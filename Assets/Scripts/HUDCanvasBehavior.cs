using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDCanvasBehavior : MonoBehaviour
{
    // Game Manager reference
    public static GameManager GM;

    // Canvas reference
    public static HUDCanvasBehavior CV;

    // Button scene navigation references
    private static Button homeButton;
    private static Button storeButton;

    private void Awake()
    {
        if (CV == null)
        {
            CV = this;
        }
        else if (CV != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // Persist between levels
    }

    // Use this for initialization
    void Start()
    {
        // Game Manager 
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        // Initialize references and display store button, hiding home button
        homeButton = GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>();
        storeButton = GameObject.FindGameObjectWithTag("StoreButton").GetComponent<Button>();
        loadStoreButton();
    }

    // Show store and hide home buttons
    public void loadStoreButton()
    {
        homeButton.image.gameObject.SetActive(false);
        homeButton.interactable = false;
        storeButton.image.gameObject.SetActive(true);
        storeButton.interactable = true;
    }
    // Show home and hide store buttons;
    public void loadHomeButton()
    {
        homeButton.image.gameObject.SetActive(true);
        homeButton.interactable = true;
        storeButton.image.gameObject.SetActive(false);
        storeButton.interactable = false;
    }

    // When home button is pressed, load GameStart scene
    public void loadHome()
    {
        // If threat level critical and player
        // quits to home screen instead of resolving
        // decrement life and return to home
        if (GM.Player != null && GM.Player.getIsPaused())
        {
            GM.decrementHostItem(GM.HOST_LIVES_INDEX, 1);
        }

        // Load game start scene
        //loadStoreButton();
        GM.restartGame();

        // Debug.Log("load home");
    }
}
