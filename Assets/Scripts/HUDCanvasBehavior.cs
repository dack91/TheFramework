using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDCanvasBehavior : MonoBehaviour
{
    public static HUDCanvasBehavior CV;
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
        homeButton = GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>();
        storeButton = GameObject.FindGameObjectWithTag("StoreButton").GetComponent<Button>();
        loadStoreButton();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadStoreButton()
    {
        homeButton.image.gameObject.SetActive(false);
        storeButton.image.gameObject.SetActive(true);
    }
    public void loadHomeButton()
    {
        homeButton.image.gameObject.SetActive(true);
        storeButton.image.gameObject.SetActive(false);
    }

    public void loadHome()
    {
        loadStoreButton();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().restartGame();

        Debug.Log("load home");
        // SceneManager.LoadScene("GameStart");
    }
}
