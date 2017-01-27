using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QAPair {

    public static string cardImagePrefix = "CardImages/Question";
    public static string cardImageSuffix = "_orange";
    public static string videofilePrefix = "Question";

    public string question;
    string videoFilepath;
    string cardImage;
    public float cardDisplayTime; // When does she stop answering a question

    public QAPair (string question, float time)
    {
        this.question = question;
        this.cardDisplayTime = time;
    }
}
