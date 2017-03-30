using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencingPuzzle : MonoBehaviour {
    // Player script
    private PlayerMovement Player;

    // Light boxes for color sequencing
    public SequencingPiece[] boxes;

    // Puzzle timers
    public float timeLimit;

    // Puzzle state
    private bool isSolved;
    private bool isActive;
    private bool hasFailed;

    //Sequence generated for puzzle
    public int PUZZLE_LENGTH = 5;
    private int[] sequence;
    private int curSeqIndex = 0;

    // Use this for initialization
    void Start () {
        isSolved = false;
        isActive = false;
        hasFailed = false;

        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        // Generate random color sequence
        generateSequence();

        // Visualize sequence for player
        StartCoroutine(showSequence());
	}
	
	// Update is called once per frame
	void Update () {
        // If showing sequence is done,
        // allow user to repeat it
        if (isActive)
        {
            // If incorrect sequence entered
            if(hasFailed)
            {
                // Lose double lives and reset level
                Debug.Log("PUZZLE FAIL");
                Player.threatSaveFailed();

                // Reset fail bool so penalties 
                // are only applied once
                hasFailed = false;

                // Destroy puzzle prefab after 1 sec
                Destroy(gameObject, 1);
            }

            // If correct sequence entered
            else if (isSolved)
            {
                // Reset threat level and 
                // unpause game
                Debug.Log("PUZZLE SUCCESS");
                Player.threatSaveSuccessful();

                // Destroy puzzle prefab immediately
                Destroy(gameObject);
            }

            // Time Limit to enter correct sequence
            timeLimit -= Time.deltaTime;
            if (timeLimit <= 0)
            {
                // Successfully repeated sequence
                if (isSolved)
                {
                    // Reset threat level and 
                    // unpause game
                    Debug.Log("PUZZLE SUCCESS");
                    Player.threatSaveSuccessful();
                }
                // Timer expired before correct sequence input
                else
                {
                    // Lose double lives and reset level
                    Player.threatSaveFailed();
                }

                // Destroy puzzle prefab immediately
                Destroy(gameObject);
            }
        }
	}

    // Called from onMouseDown on a color box child
    public void boxPicked(int boxID)
    {
        // Check if box selected matches current box in sequence
        if (sequence[curSeqIndex] == boxID && curSeqIndex < sequence.Length)
        {
            //Debug.Log("correct");
            // Increment sequence index
            curSeqIndex++;

            // Check if player reached end of sequence
            if (curSeqIndex == sequence.Length)
            {
                isSolved = true;
            }
        }
        // Incorrect box selected for sequence
        else
        {
            hasFailed = true;
        }
    }

    // At initialization, visually show player the sequence
    public IEnumerator showSequence()
    {
        //Debug.Log("starting show sequence");
        // Show each color in sequence
        for(int i = 0; i < sequence.Length; i++) 
        {
            // Select current box
            SequencingPiece curr = boxes[sequence[i]];
            Debug.Log("show seq: " + sequence[i] + " -- " + curr.color.x + ", " + curr.color.y + ", " + curr.color.z);

            // Light up current box, wait for 1 sec
            // then reset to neutral
            curr.GetComponent<Renderer>().material.color = new Color(curr.color.x, curr.color.y, curr.color.z);
            yield return new WaitForSeconds(1);
            curr.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
            yield return new WaitForSeconds(0.5f);
        }

        // Enable mouse input for player
        // to repeat color sequence
        isActive = true;
    }

    // Generate a random sequence for puzzle
    public void generateSequence()
    {
        // Declare length of sequence
        sequence = new int[PUZZLE_LENGTH];

        // For each color in puzzle, 
        // generate random child box ID
        for(int i = 0; i < PUZZLE_LENGTH; i++)
        {
            sequence[i] = Random.Range(0, boxes.Length - 1);
        }
    }

    //getter
    public bool getIsActive()
    {
        return isActive;
    }
}
