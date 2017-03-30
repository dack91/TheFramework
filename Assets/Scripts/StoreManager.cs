using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StoreManager : MonoBehaviour {
    private GameManager GM;
    private Button prizeButton;

	// Use this for initialization
	void Start () {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        prizeButton = GameObject.FindGameObjectWithTag("PrizeButton").GetComponent<Button>();
        prizeButton.interactable = GM.getIsPrizeAvailable();
    }
	
	// Update is called once per frame
	void Update () {
		if (GM.getIsPrizeAvailable())
        {
            prizeButton.interactable = GM.getIsPrizeAvailable();
        }
	}

    public void buyItem(int itemID)
    {
        switch(itemID)
        {
            // Buy Lives
            case 0:
                break;
            // Buy Bribes
            case 1:
                break;
            // Watch for Lives
            case 2:
                break;
            // Buy Persuasion
            case 3:
                break;
              
        }
    }

    public void collectDailyPrize()
    {
        Debug.Log("collect prize, start 24h clock");
        prizeButton.interactable = false;
        GM.collectPrize();
    }
}
