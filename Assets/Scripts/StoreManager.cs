using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StoreManager : MonoBehaviour {
    // Game Manager reference
    private GameManager GM;

    // Particle system for buying effect
    public ParticleSystem ps;
    private ParticleSystemRenderer psR;
    public Material m_lives;
    public Material m_diversions;
    public Material m_bribes;
    public Material m_gift;

    // Daily prize button reference
    private Button prizeButton;

	// Use this for initialization
	void Start () {
        // Initialize references and activate prize button if available
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        prizeButton = GameObject.FindGameObjectWithTag("PrizeButton").GetComponent<Button>();
        prizeButton.interactable = GM.getIsPrizeAvailable();
        psR = ps.GetComponent<ParticleSystemRenderer>();
    }

    // Update is called once per frame
    void Update () {
        // Check if daily prize has regenerated
		if (GM.getIsPrizeAvailable())
        {
            prizeButton.interactable = GM.getIsPrizeAvailable();
        }
	}

    // Add player resources if item bought from store
    public void buyItem(int itemID)
    {
        bool canBuyResource = false;
        switch (itemID)
        {
            // Buy Lives
            case 0:
                if (GM.canBuyResource(GM.HOST_LIVES_INDEX))
                {
                    // Move particle emittor to lives button
                    ps.transform.position = new Vector3(-2.5f, -0.5f, 0f);
                    // Change particle material to lives
                    psR.material = m_lives;
                    canBuyResource = true;
                }
                // Increase player lives
                GM.decrementHostItem(GM.HOST_LIVES_INDEX, -5);
                break;
            // Buy Bribes
            case 1:
                if (GM.canBuyResource(GM.HOST_BRIBES_INDEX))
                {
                    // Move particle emittor to bribe button
                    ps.transform.position = new Vector3(2.5f, -0.5f, 0f);
                    // Change particle material to bribes
                    psR.material = m_bribes;
                    canBuyResource = true;
                }
                // Increase player bribes
                GM.decrementHostItem(GM.HOST_BRIBES_INDEX, -3);
                break;
            // Watch Ad for Lives
            case 2:
                if (GM.canBuyResource(GM.HOST_LIVES_INDEX))
                {
                    // Move particle emittor to ad button
                    ps.transform.position = new Vector3(-2.5f, -3f, 0f);
                    // Change particle material to lives
                    psR.material = m_lives;
                    canBuyResource = true;
                }
                // Increase player lives
                GM.decrementHostItem(GM.HOST_LIVES_INDEX, -2);
                break;
            // Buy Persuasion
            case 3:
                if (GM.canBuyResource(GM.HOST_PERSUADES_INDEX))
                {
                    // Move particle emittor to diversions button
                    ps.transform.position = new Vector3(2.5f, -3f, 0f);
                    // Change particle material to diversions
                    psR.material = m_diversions;
                    canBuyResource = true;
                }
                // Increase player diversions
                GM.decrementHostItem(GM.HOST_PERSUADES_INDEX, -5);
                break;
              
        }
        // If resource is not full
        if (canBuyResource)
        {
            // Clear any left-over particles
            ps.Clear();
            // Start particle system emittor
            ps.Play();
        }
    }

    // Collect prize and disable until regenerated
    public void collectDailyPrize()
    {
        // Clear any left-over particles
        ps.Clear();
        // Move particle emittor to daily bonus button
        ps.transform.position = new Vector3(0f, 2f, 0f);
        // Change particle material to daily bonus
        psR.material = m_gift;
        // Start particle system emittor
        ps.Play();

        //Debug.Log("collect prize, start 24h clock");
        prizeButton.interactable = false;
        GM.collectPrize();
    }
}
