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
        // Check if daily prize has regenerated
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
                GM.decrementHostItem(GM.HOST_LIVES_INDEX, -5);
                break;
            // Buy Bribes
            case 1:
                GM.decrementHostItem(GM.HOST_BRIBES_INDEX, -3);
                break;
            // Watch for Lives
            case 2:
                GM.decrementHostItem(GM.HOST_LIVES_INDEX, -2);
                break;
            // Buy Persuasion
            case 3:
                GM.decrementHostItem(GM.HOST_PERSUADES_INDEX, -5);
                break;
              
        }
    }

    // Collect prize and disable until regenerated
    public void collectDailyPrize()
    {
        //Debug.Log("collect prize, start 24h clock");
        prizeButton.interactable = false;
        GM.collectPrize();
    }
}
