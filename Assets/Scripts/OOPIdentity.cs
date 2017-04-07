using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOPIdentity : MonoBehaviour {
    // Game manager reference
    private GameManager GM;

    // OOP ID
    public int index;
    public bool isTraitor;

    // OOP Color
    public Vector3 color;
    // OOP Renderer to change material color
    private Renderer rend;

    // Use this for initialization
    void Start () {
        rend = gameObject.GetComponent<Renderer>();
        rend.material.color = new Color(color.x, color.y, color.z);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Player")
        //{
        //    Debug.Log("check traitor");
        //    if (inspectOOP())
        //    {
        //        Debug.Log("LEVEL WON");
        //    }
        //    // Rugen inspection was incorrect, decrement persuade
        //    else
        //    {
        //        GM.decrementHostItem(GM.HOST_PERSUADES_INDEX, 1);
        //    }
        //}
    }


    // Check if this OOP is trying to escape
    public bool inspectOOP()
    {
        StartCoroutine(revealInspection());
        return isTraitor;
    }

    public IEnumerator revealInspection()
    {
        if (isTraitor)
        {
            rend.material.color = new Color(1, 0,0);
        }
        else
        {
            rend.material.color = new Color(0, 1,0);
        }
        yield return new WaitForSeconds(2);
        rend.material.color = new Color(color.x, color.y, color.z);
    }
}
