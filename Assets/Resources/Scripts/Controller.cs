using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public static int leftQAIndex = 0;
    public static int rightQAIndex = 1;
    public static List<QAPair> interactions;
    public static bool listeningMode = true;

    public string[] questionBank;
    public string[] QuestionBank { get; set; }
    public float[] cardDisplayTimes;

    VideoHandler videoHandler;
    PlayVideo playVideo;
    CardHandler cardHandler;
    SpeechRecognition speechRecognition;

    // Use this for initialization
    void Start () {

        videoHandler = GameObject.Find("MediaPlayer").GetComponent<VideoHandler>();
        cardHandler = GameObject.Find("Cards").GetComponent<CardHandler>();
        playVideo = GameObject.Find("VideoPlane").GetComponent<PlayVideo>();
        speechRecognition = GetComponent<SpeechRecognition>();
        interactions = new List<QAPair>();

        // Create QAPair list
        CreatePairList();

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
        // Stop listening to further speech
        listeningMode = false;
        speechRecognition.dictationRecognizer.Stop();

        // Trigger next video
        //videoHandler.nextCut(QAPair.videofilePrefix + questionIndex);

        // Update index variables and card images
        int newIndex = Mathf.Max(leftQAIndex, rightQAIndex) + 1;
        if (questionIndex == leftQAIndex)
        {
            playVideo.nextCut(0); // TODO: Enumeration
            if (newIndex == interactions.Count)
            {
                leftQAIndex = -1;
            }
            else
            {
                leftQAIndex = newIndex;
                //cardHandler.SetLeftCardImage(QAPair.cardImagePrefix + leftQAIndex + QAPair.cardImageSuffix);
                playVideo.loadLeftMovie(QAPair.videofilePrefix + leftQAIndex);
            }

        }
        else if (questionIndex == rightQAIndex)
        {
            playVideo.nextCut(1);
            if (newIndex == interactions.Count)
            {
                rightQAIndex = -1;
            }
            else
            {
                rightQAIndex = newIndex;
                //cardHandler.SetRightCardImage(QAPair.cardImagePrefix + rightQAIndex + QAPair.cardImageSuffix);
                playVideo.loadRightMovie(QAPair.videofilePrefix + rightQAIndex);
            }
        }

        StartCoroutine(Wait(interactions[questionIndex].cardDisplayTime));
        StartCoroutine(HideCardsIn(3f));
    }

    void ListeningModeOn ()
    {
        if (leftQAIndex == -1)
        {
            cardHandler.SetLeftCardImage("CardImages/IntroQuestion_orange");  
        }
        else
        {
            cardHandler.SetLeftCardImage(QAPair.cardImagePrefix + leftQAIndex + QAPair.cardImageSuffix);
        }

        if (rightQAIndex == -1)
        {
            cardHandler.SetRightCardImage("CardImages/IntroQuestion_orange");
        }
        else
        {
            cardHandler.SetRightCardImage(QAPair.cardImagePrefix + rightQAIndex + QAPair.cardImageSuffix);
        }

        if (leftQAIndex == -1 && rightQAIndex == -1)
        {
            // Fade out scene
        }

        // Called when girl stops talking in video. Cards are displayed. Dictation recognizer enabled. 
        listeningMode = true;
        cardHandler.DisplayCards();
        speechRecognition.dictationRecognizer.Start();
    }

    IEnumerator Wait (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ListeningModeOn();
    }

    IEnumerator HideCardsIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        cardHandler.HideCards();
    }
}
