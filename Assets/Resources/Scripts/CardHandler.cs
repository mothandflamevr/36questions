using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour {

    public GameObject leftCard;
    public GameObject rightCard;

    public static bool IsHidden = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayCards ()
    {
        gameObject.SetActive(true);
        IsHidden = false;
    }

    public void HideCards()
    {
        gameObject.SetActive(false);
        IsHidden = true;
    }

    public void SetCardImages (string leftImagePath, string rightImagePath)
    {
        SetLeftCardImage(leftImagePath);
        SetRightCardImage(rightImagePath);
    }

    public void SetLeftCardImage (string path)
    {
        leftCard.GetComponent<Renderer>().material.mainTexture = Resources.Load(path) as Texture;
    }

    public void SetRightCardImage (string path)
    {
        rightCard.GetComponent<Renderer>().material.mainTexture = Resources.Load(path) as Texture;
    }
}
