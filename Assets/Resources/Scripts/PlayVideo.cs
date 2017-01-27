using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideo : MonoBehaviour {

    AudioSource audioSource;
    public MovieTexture leftMovie;
    public MovieTexture rightMovie;
    MovieTexture currentMovie;

    MovieTexture introVideo;

	// Use this for initialization
	void Start () {

        audioSource = GetComponent<AudioSource>();
        currentMovie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        introVideo = currentMovie;
    }
	
	// Update is called once per frame
	void Update () {
		//if (!currentMovie.isPlaying)
  //      {
  //          Debug.Log("Video stopped");
  //          GetComponent<Renderer>().material.mainTexture = introVideo;
  //          audioSource.clip = null;
  //          audioSource.clip = introVideo.audioClip;

  //          audioSource.Play();
  //          currentMovie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
  //          currentMovie.Play();
  //      }
	}

    public void nextCut(int i)
    {
        //string path = Application.streamingAssetsPath + videoFile;
        //WWW www = new WWW(path);

        //MovieTexture newMovie = Resources.Load(videoFile) as MovieTexture;
        MovieTexture newMovie;
        if (i == 0) newMovie = leftMovie;
        else newMovie = rightMovie;

        MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        if (movie.isPlaying)
        {
            movie.Stop();
            audioSource.Stop();
        }
        //movie = (MovieTexture)www.movie;
        GetComponent<Renderer>().material.mainTexture = newMovie;
        audioSource.clip = null;
        audioSource.clip = newMovie.audioClip;

        audioSource.Play();
        newMovie.Play();

        //currentMovie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        // Hide cards
        //GameObject.Find("Cards").GetComponent<CardHandler>().HideCards();
    }

    public void loadLeftMovie (string file)
    {
        leftMovie = Resources.Load(file) as MovieTexture;
    }

    public void loadRightMovie(string file)
    {
        rightMovie = Resources.Load(file) as MovieTexture;
    }
}
