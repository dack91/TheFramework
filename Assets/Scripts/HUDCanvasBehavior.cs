using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDCanvasBehavior : MonoBehaviour
{
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
        // Initialize references and display store button, hiding home button
        homeButton = GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>();
        storeButton = GameObject.FindGameObjectWithTag("StoreButton").GetComponent<Button>();
        loadStoreButton();
    }

    // Show store and hide home buttons
    public void loadStoreButton()
    {
        homeButton.image.gameObject.SetActive(false);
        storeButton.image.gameObject.SetActive(true);
    }
    // Show home and hide store buttons;
    public void loadHomeButton()
    {
        homeButton.image.gameObject.SetActive(true);
        storeButton.image.gameObject.SetActive(false);
    }

    // When home button is pressed, load GameStart scene
    public void loadHome()
    {
        loadStoreButton();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().restartGame();

        // Debug.Log("load home");
    }
}
