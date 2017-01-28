using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoHandler : MonoBehaviour {

    MediaPlayer mp1;
    MediaPlayer mp2;
    public GameObject screen1;
    public GameObject screen2;
    bool isLoading = false;

    int availablePlayer = 1;
    MediaPlayer[] mps;
    GameObject[] screens;

    // Use this for initialization
    void Start () {
        mp1 = transform.FindChild("MediaPlayer1").GetComponent<MediaPlayer>();
        mp1.Events.AddListener(OnVideoEvent1);

        mp2 = transform.FindChild("MediaPlayer2").GetComponent<MediaPlayer>();
        mp2.Events.AddListener(OnVideoEvent2);

        mps = new MediaPlayer[] { mp1, mp2 };
        screens = new GameObject[] { screen1, screen2 };
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Plays next video file based on first detected word
    /// </summary>
    /// <param name="word">Word that was spoken</param>
    /// <param name="videoFile">Video file that should be played</param>
    public void nextCut(string videoFile) {
        isLoading = true;
        if (availablePlayer == 1)
        {
            mp1.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFile, true);
            availablePlayer = 2;
        }
        else if (availablePlayer == 2)
        {
            mp2.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFile, true);
            availablePlayer = 1;
        }


    }

    // Callback function to handle events
    public void OnVideoEvent1(MediaPlayer mp, MediaPlayerEvent.EventType et,
    ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                isLoading = false;
                StartCoroutine(screenDisplay(1f, 1));
                StartCoroutine(screenTimeout(2f, 2));
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                mp.Control.Stop();
                nextCut("Intro.mp4");
                break;
        }
        //Debug.Log("Event: " + et.ToString());
    }

    public void OnVideoEvent2(MediaPlayer mp, MediaPlayerEvent.EventType et,
ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                isLoading = false;
                StartCoroutine(screenDisplay(1f, 2));
                StartCoroutine(screenTimeout(2f, 1));
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                mp.Control.Pause();
                nextCut("Intro.mp4");
                break;
        }
        //Debug.Log("Event: " + et.ToString());
    }

    IEnumerator screenTimeout(float delayTime, int index) {
        yield return new WaitForSeconds(delayTime);
        //GameObject.Find("Instruction").SetActive(false);

        screens[index - 1].SetActive(false);
        mps[index - 1].Control.Stop();
    }

    IEnumerator screenDisplay(float delayTime, int index)
    {
        yield return new WaitForSeconds(delayTime);
        //GameObject.Find("Instruction").SetActive(false);

        screens[index - 1].SetActive(true);
    }
}
