using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoHandler : MonoBehaviour
{

    MediaPlayer mp1;
    MediaPlayer mp2;
    public GameObject screen1;
    public GameObject screen2;
    public bool isTransitioning = false;

    int availablePlayer = 1;
    MediaPlayer[] mps;
    GameObject[] screens;

    private IEnumerator LoopCoroutine;

    // Use this for initialization
    void Start()
    {
        mp1 = transform.FindChild("MediaPlayer1").GetComponent<MediaPlayer>();
        mp1.Events.AddListener(OnVideoEvent1);

        mp2 = transform.FindChild("MediaPlayer2").GetComponent<MediaPlayer>();
        mp2.Events.AddListener(OnVideoEvent2);

        mps = new MediaPlayer[] { mp1, mp2 };
        screens = new GameObject[] { screen1, screen2 };
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Plays next video file based on first detected word
    /// </summary>
    /// <param name="word">Word that was spoken</param>
    /// <param name="videoFile">Video file that should be played</param>
    public void nextCut(string videoFile)
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
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
    }

    // Callback function to handle events
    public void OnVideoEvent1(MediaPlayer mp, MediaPlayerEvent.EventType et,
    ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                StartCoroutine(FadeIn(2f, 1));
                StartCoroutine(FadeOut(2f, 2));
                StopCoroutine("LoopIntroVideo");
                StartCoroutine("LoopIntroVideo", mp.Info.GetDurationMs() / 1000);
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                mp.Control.Pause();
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
                StartCoroutine(FadeIn(2f, 2));
                StartCoroutine(FadeOut(2f, 1));
                StopCoroutine("LoopIntroVideo");
                StartCoroutine("LoopIntroVideo", mp.Info.GetDurationMs() / 1000);
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                mp.Control.Pause();
                break;
        }
        //Debug.Log("Event: " + et.ToString());
    }

    public IEnumerator FadeIn(float duration, int index)
    {
        index -= 1;
        Color color = screens[index].GetComponent<Renderer>().material.color;
        screens[index].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0);
        screens[index].SetActive(true);

        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            color = screens[index].GetComponent<Renderer>().material.color;
            screens[index].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, t);
            yield return null;
        }

        isTransitioning = false;
    }

    public IEnumerator FadeOut(float duration, int index)
    {
        index -= 1;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            Color color = screens[index].GetComponent<Renderer>().material.color;
            screens[index].GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 1 - t);
            yield return null;
        }
        screens[index].SetActive(false);
        mps[index].Control.Stop();
        isTransitioning = false;
    }

    public IEnumerator LoopIntroVideo (float duration)
    {
        yield return new WaitForSeconds(duration - 2.5f);
        nextCut("Intro.mp4");
    }
}
