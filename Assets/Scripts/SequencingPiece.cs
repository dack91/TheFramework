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
        // If the puzzle allows input
        if (Puzzle.getIsActive())
        {
            //Debug.Log("CUBE " + index + " CLICKED");

            // Select box and check sequencing
            Puzzle.boxPicked(index);

            // Light up color for selected box
            StartCoroutine(boxChecked());
        }
    }

    // Change material color to input RGB value
    public void changeColor(Vector3 c)
    {
        rend.material.color = new Color(c.x, c.y, c.z);
    }

    // Light up material color and reset to neutral
    // after 1 second
    public IEnumerator boxChecked()
    {
        changeColor(color);
        yield return new WaitForSeconds(1);
        changeColor(baseColor);
    }
}
