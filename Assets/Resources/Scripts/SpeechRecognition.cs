using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class SpeechRecognition : MonoBehaviour {

    private KeywordRecognizer keywordRecognizer;
    public DictationRecognizer dictationRecognizer;
    public delegate void QuestionRecognition(int index);
    public static event QuestionRecognition OnRecognized;

    public static int minWordCount = 4;
    public static float minAcceptableWordComparisonSuccessRatio = 0.75f;

    public static Dictionary<string, string> keywords = new Dictionary<string, string>();
    int numberOfChoices;

    // Use this for initialization
    void Start () {

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
	void Update () {
		
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
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        //Debug.Log("Hypothesis is: " + text);
        //Debug.Log(Controller.leftQAIndex);
        //Debug.Log(Controller.rightQAIndex);
        string[] hypoWords = text.Split(" "[0]);
        if (hypoWords.Length >= minWordCount)
        {
            if (Controller.leftQAIndex < 0)
            {
                CompareToQuestions(hypoWords, "What what what what what what what", Controller.interactions[Controller.rightQAIndex].question);
            }
            else if (Controller.rightQAIndex < 0)
            {
                CompareToQuestions(hypoWords, Controller.interactions[Controller.leftQAIndex].question, "What what what what what what what");
            }
            else
            {
                CompareToQuestions(hypoWords, Controller.interactions[Controller.leftQAIndex].question, Controller.interactions[Controller.rightQAIndex].question);
            }
        }
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation complete" + cause.ToString());
        if (Controller.listeningMode) dictationRecognizer.Start();
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.LogError("Dictation error: " + error);
    }

    /// <summary>
    /// Decides whether next video should be triggered based on similarity of dictation hypothesis with the existing 2 questions. 
    /// This decision depends on minAcceptableWordComparisonSuccessRatio which is the proportion of words that the hypothesis needs to match with either of the given questions
    /// </summary>
    /// <param name="words"></param>
    /// <param name="question1"></param>
    /// <param name="question2"></param>
    /// <returns>Positive if question 1 matches, negative if question 2 matches, 0 otherwise</returns>
    public static void CompareToQuestions (string[] words, string question1, string question2)
    {
        string[] question1Words = question1.Split(" "[0]);
        string[] question2Words = question2.Split(" "[0]);

        int matchingWordCount1 = 0;
        int matchingWordCount2 = 0;

        int l = Mathf.Min(words.Length, question1Words.Length, question2Words.Length);
        //Debug.Log("Min length: " + l);
        for (int i = 0; i < l; i++)
        {
            if (string.Equals(words[i],question1Words[i]))
            {
                matchingWordCount1 += 1;
            }
            if (string.Equals(words[i], question2Words[i]))
            {
                matchingWordCount2 += 1;
            }
        }

        if (matchingWordCount1 / (float)words.Length > minAcceptableWordComparisonSuccessRatio)
        {
            OnRecognized(Controller.leftQAIndex);
            Debug.Log("matches left question");
        }
        else if (matchingWordCount2 / (float)words.Length > minAcceptableWordComparisonSuccessRatio)
        {
            OnRecognized(Controller.rightQAIndex);
            Debug.Log("matches right question");
        }
    }

    // Keyword recognizer handler
    //private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    //{
    //    string videoFile;
    //    // if the keyword recognized is in our dictionary, call handler in videoHandler
    //    if (keywords.TryGetValue(args.text, out videoFile) && args.confidence <= ConfidenceLevel.Low)
    //    {
    //        Debug.Log("You said " + args.text + " (acceptable confidence)");

    //        // Trigger cut
    //        videoHandler.nextCut(args.text, videoFile);

    //        // Change card text for next time
    //        if (args.text == questionBank[0])
    //        {
    //            videoHandler.setOrReplaceCardText(0, questionBank[2]);
    //        }
    //        else if (args.text == questionBank[1]) {
    //            videoHandler.setOrReplaceCardText(1, questionBank[2]);
    //        }
    //    }
    //    else if (keywords.TryGetValue(args.text, out videoFile) && args.confidence == ConfidenceLevel.Rejected) {
    //        Debug.Log("You said " + args.text + " but it was rejected");
    //    }
    //}

    //void initializeDictionary() {
    //    keywords.Clear();
    //    for (int i = 0; i < numberOfChoices; i++) {
    //        keywords.Add(questionBank[i], videoFilePaths[i]);
    //    }
    //}

}
