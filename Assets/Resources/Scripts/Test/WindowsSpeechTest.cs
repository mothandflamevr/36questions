using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class WindowsSpeechTest : MonoBehaviour {

    private KeywordRecognizer keywordRecognizer;
    public DictationRecognizer dictationRecognizer;
    public delegate void QuestionRecognition(int index);
    public static event QuestionRecognition OnRecognized;

    public static int minWordCount = 4;
    public static float minAcceptableWordComparisonSuccessRatio = 0.75f;

    public static Dictionary<string, string> keywords = new Dictionary<string, string>();
    int numberOfChoices;

    public GameObject UIText;

    // Use this for initialization
    void Start()
    {

        // Set up initial keywords 
        //initializeDictionary();

        //keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        //keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        //keywordRecognizer.Start();

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.AutoSilenceTimeoutSeconds = 100f;
        dictationRecognizer.InitialSilenceTimeoutSeconds = 100f;

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        dictationRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Stop()
    {
        dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
        dictationRecognizer.Dispose();
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Result is: " + text);
        UpdateText("Result is: " + text);
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log("Hypothesis is: " + text);
        UpdateText("Hypothesis is: " + text);
        //Debug.Log(Controller.leftQAIndex);
        //Debug.Log(Controller.rightQAIndex);
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation complete " + cause.ToString());
        UpdateText("Dictation complete, try talking again.");
        dictationRecognizer.Start();
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
        UpdateText("Dictation error: " + error);
    }

    private void UpdateText (string text)
    {
        UIText.GetComponent<Text>().text = text;
    }

}
