using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public static int leftQAIndex = 0;
    public static int rightQAIndex = 1;
    public static int questionsAsked = 0;
    public static List<QAPair> interactions;
    public static bool listeningMode = true;

    public string[] questionBank;
    public string[] QuestionBank { get; set; }
    public float[] cardDisplayTimes;

    VideoHandler videoHandler;
    PlayVideo playVideo;
    CardHandler cardHandler;
    SpeechRecognition speechRecognition;
    bool flagLeft, flagRight = false;

    // Use this for initialization
    void Start () {

        videoHandler = GameObject.Find("MediaPlayers").GetComponent<VideoHandler>();
        cardHandler = GameObject.Find("Cards").GetComponent<CardHandler>();
        //playVideo = GameObject.Find("VideoPlane").GetComponent<PlayVideo>();
        speechRecognition = GetComponent<SpeechRecognition>();
        interactions = new List<QAPair>();

        // Create QAPair list
        CreatePairList();
        speechRecognition.initializeSpeechRecognitionSystem();

        // Initialize experience: Set correct images to cards
        cardHandler.SetCardImages(QAPair.cardImagePrefix + leftQAIndex + QAPair.cardImageSuffix, QAPair.cardImagePrefix + rightQAIndex + QAPair.cardImageSuffix);

        // Start intro video
        //MovieTexture movie = playVideo.GetComponent<Renderer>().material.mainTexture as MovieTexture;
        //playVideo.GetComponent<AudioSource>().clip = movie.audioClip;
        //playVideo.GetComponent<AudioSource>().Play();
        //movie.Play();

        //playVideo.loadLeftMovie(QAPair.videofilePrefix + leftQAIndex);
        //playVideo.loadRightMovie(QAPair.videofilePrefix + rightQAIndex);

        videoHandler.nextCut("Intro.mp4");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        SpeechRecognition.OnRecognized += TriggerNextVideo;
    }

    private void CreatePairList()
    {
        for (int i = 0; i < questionBank.Length; i++)
        {
            QAPair pair = new QAPair(questionBank[i], cardDisplayTimes[i]);
            interactions.Add(pair);
        }
    }

    public void TriggerNextVideo (int questionIndex)
    {
        listeningMode = false;
        questionsAsked += 1;

        // Stop listening to further speech
        if (speechRecognition.shouldUseDictation)
            speechRecognition.dictationRecognizer.Stop();
        else speechRecognition.keywordRecognizer.Stop();


        // Trigger next video. Trigger after 2 seconds if there is currently a transiton in effect
        if (videoHandler.isTransitioning)
        {
            StartCoroutine(PlayVideoAfter(2f, QAPair.videofilePrefix + questionIndex + ".mp4", questionIndex));
        }
        else StartCoroutine(PlayVideoAfter(0f, QAPair.videofilePrefix + questionIndex + ".mp4", questionIndex));

        // Update index variables and card images
        int newIndex = Mathf.Max(leftQAIndex, rightQAIndex) + 1;
        if (questionIndex == leftQAIndex)
        {
            if (newIndex < interactions.Count)
                leftQAIndex = newIndex;
            else flagLeft = true;
            //cardHandler.SetLeftCardImage(QAPair.cardImagePrefix + leftQAIndex + QAPair.cardImageSuffix);
            //playVideo.loadLeftMovie(QAPair.videofilePrefix + leftQAIndex);

        }
        else if (questionIndex == rightQAIndex)
        {
            //playVideo.nextCut(1);
            if (newIndex < interactions.Count)
                rightQAIndex = newIndex;
            else flagRight = true;
            //cardHandler.SetRightCardImage(QAPair.cardImagePrefix + rightQAIndex + QAPair.cardImageSuffix);
            //playVideo.loadRightMovie(QAPair.videofilePrefix + rightQAIndex);
        }
    }

    void ListeningModeOn ()
    {
        if (flagLeft)
        {
            cardHandler.SetLeftCardImage("CardImages/IntroQuestion_grey");  
        }
        else
        {
            cardHandler.SetLeftCardImage(QAPair.cardImagePrefix + leftQAIndex + QAPair.cardImageSuffix);
        }

        if (flagRight)
        {
            cardHandler.SetRightCardImage("CardImages/IntroQuestion_grey");
        }
        else
        {
            cardHandler.SetRightCardImage(QAPair.cardImagePrefix + rightQAIndex + QAPair.cardImageSuffix);
        }

        if (questionsAsked == interactions.Count) return;

        // Called when girl stops talking in video. Cards are displayed. Dictation recognizer enabled. 
        listeningMode = true;
        cardHandler.DisplayCards();

        // Activate speech recognition
        if (speechRecognition.shouldUseDictation)
            speechRecognition.dictationRecognizer.Start();
        else speechRecognition.keywordRecognizer.Start();
    }

    IEnumerator WaitForListeningMode (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ListeningModeOn();
    }

    IEnumerator HideCardsIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cardHandler.HideCards();
    }

    IEnumerator PlayVideoAfter(float seconds, string videoFile, int index)
    {
        yield return new WaitForSeconds(seconds);
        videoHandler.nextCut(videoFile);
        StartCoroutine(WaitForListeningMode(interactions[index].cardDisplayTime));
        StartCoroutine(HideCardsIn(2f));
    }
}
