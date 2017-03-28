using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCanvasBehavior : MonoBehaviour
{
    public static StartCanvasBehavior CV;

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

    }

    // Update is called once per frame
    void Update()
    {

    }
}
