using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordRecognition : MonoBehaviour {

    private KeywordRecognizer keywordRecognizer;
    private DictationRecognizer dictationRecognizer;

    public string[] questionBank;
    public string[] videoFilePaths;
    string[] keywords;
    public int numberOfChoices;

    // Use this for initialization
    void Start()
    {

        // Set up initial keywords 
        keywords = new string[] {"When did you last sing" , "For what in your life", "If we become friends" };

        keywordRecognizer = new KeywordRecognizer(keywords);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Keyword recognizer handler
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (string.Equals(args.text, keywords[0]) && args.confidence < ConfidenceLevel.Rejected)
        {
            Debug.Log("QUESTION 0");
        }
        else if (string.Equals(args.text, keywords[1]) && args.confidence < ConfidenceLevel.Rejected)
        {
            Debug.Log("QUESTION 1");
        }
        else if (string.Equals(args.text, keywords[2]) && args.confidence < ConfidenceLevel.Rejected)
        {
            Debug.Log("QUESTION 2");
        }
        else
        {
            Debug.Log("Rejected");
        }
    }
}
