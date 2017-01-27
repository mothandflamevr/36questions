using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoHandler : MonoBehaviour {

    public MediaPlayer mp;
    public bool isLoading = false;

    // Use this for initialization
    void Start () {
        //mp = GetComponentInChildren<MediaPlayer>();
        mp.Events.AddListener(OnVideoEvent);

        //mp.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "guyquestion1.mp4", false);
        //mp.Control.SeekFast(7000f);
        //mp.Control.Play();
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
        mp.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, videoFile, true);
    }

    // Callback function to handle events
    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et,
    ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                isLoading = false;
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                mp.Control.Stop();
                break;
        }
        //Debug.Log("Event: " + et.ToString());
    }

    IEnumerator instructionTimeout(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        GameObject.Find("Instruction").SetActive(false);
    }
}
