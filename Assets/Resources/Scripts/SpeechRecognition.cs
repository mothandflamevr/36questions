using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class SpeechRecognition : MonoBehaviour {

    private KeywordRecognizer keywordRecognizer;
    private DictationRecognizer dictationRecognizer;
    //Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public static Dictionary<string, string> keywords = new Dictionary<string, string>();
    public string[] questionBank;
    public string[] videoFilePaths;
    public int numberOfChoices;

    // Use this for initialization
    void Start () {

        // Set up initial keywords 
        initializeDictionary();

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        //keywordRecognizer.Start();

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.AutoSilenceTimeoutSeconds = 10f;
        dictationRecognizer.InitialSilenceTimeoutSeconds = 10f;

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Result is: " + text);
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log("Hypothesis is: " + text);
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation complete" + cause.ToString());
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.LogError("Dictation error: " + error);
    }

    // Keyword recognizer handler
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        string videoFile;
        // if the keyword recognized is in our dictionary, call handler in videoHandler
        if (keywords.TryGetValue(args.text, out videoFile) && args.confidence == ConfidenceLevel.High)
        {
            Debug.Log("You said " + args.text + " (high confidence)");
        }
        else if (keywords.TryGetValue(args.text, out videoFile) && args.confidence == ConfidenceLevel.Medium) {
            Debug.Log("You said " + args.text + " (medium confidence)");
        }
        else if (keywords.TryGetValue(args.text, out videoFile) && args.confidence == ConfidenceLevel.Low)
        {
            Debug.Log("You said " + args.text + " (low confidence)");
        }
    }

    void initializeDictionary() {
        keywords.Clear();
        for (int i = 0; i < numberOfChoices; i++) {
            keywords.Add(questionBank[i], videoFilePaths[i]);
        }
    }

}
