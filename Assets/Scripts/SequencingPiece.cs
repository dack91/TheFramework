using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencingPiece : MonoBehaviour {
    // Puzzle manager
    private SequencingPuzzle Puzzle;

    // Box Renderer to change material color
    private Renderer rend;

    // Box ID
    public int index;

    // Box Color
    public Vector3 color;

    // Neutral Color
    public Vector3 baseColor;

	// Use this for initialization
	void Start () {
        Puzzle = GameObject.FindGameObjectWithTag("PersuadePuzzle").GetComponent<SequencingPuzzle>();
        baseColor = new Vector3(0, 0, 0);
        rend = gameObject.GetComponent<Renderer>();
        changeColor(baseColor);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // When player clicks on boxs
    private void OnMouseDown()
    {
        // If the puzzle allows input and failure not occurred 
        if (Puzzle.getIsActive() && !Puzzle.getHasFailed())
        {
            Debug.Log("CUBE " + index + " CLICKED");

            // Light up color for selected box
            // while mouse is clicked down
            changeColor(color);
        }
    }

    private void OnMouseUp()
    {
        // Select box and check sequencing
        Puzzle.boxPicked(index);
        changeColor(baseColor);
    }

    // Change material color to input RGB value
    public void changeColor(Vector3 c)
    {
        rend.material.color = new Color(c.x, c.y, c.z);
    }
}
