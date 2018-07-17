using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreControl : MonoBehaviour {

    public Text scoreBoard;

    public GameObject p1;
    public GameObject p2;

    private float p1Score = 0;
    private float p2Score = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(p1.gameObject.tag == "Dead")
        {
            p2Score++;
            updateScore();
            p1.gameObject.tag = "P1";
        }

        if (p2.gameObject.tag == "Dead")
        {
            p1Score++;
            updateScore();
            p2.gameObject.tag = "P2";
        }
    }

    public void updateScore()
    {
        scoreBoard.text = p1Score + " - " + p2Score;
    }
}
