using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleVideoHandler : MonoBehaviour {

    MovieTexture leftMovie;
    MovieTexture rightMovie;
    public GameObject plane1;
    public GameObject plane2;
    GameObject[] videoPlanes;
    int availableIndex = 0; 

    // Use this for initialization
    void Start()
    {
        plane1 = transform.FindChild("VideoPlane1").gameObject;
        plane2 = transform.FindChild("VideoPlane2").gameObject;
        videoPlanes = new GameObject[] { plane1, plane2 };

        // Start intro video
        MovieTexture movie = videoPlanes[availableIndex].GetComponent<Renderer>().material.mainTexture as MovieTexture;
        videoPlanes[availableIndex].GetComponent<AudioSource>().clip = movie.audioClip;
        videoPlanes[availableIndex].GetComponent<AudioSource>().Play();
        movie.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void nextCut(int i)
    {
        //MovieTexture newMovie;
        //if (i == 0) newMovie = leftMovie;
        //else newMovie = rightMovie;

        //MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        //if (movie.isPlaying)
        //{
        //    movie.Stop();
        //    audioSource.Stop();
        //}
        ////movie = (MovieTexture)www.movie;
        //GetComponent<Renderer>().material.mainTexture = newMovie;
        //audioSource.clip = null;
        //audioSource.clip = newMovie.audioClip;

        //audioSource.Play();
        //newMovie.Play();

        //// Hide cards
        ////GameObject.Find("Cards").GetComponent<CardHandler>().HideCards();
    }

    public void loadLeftMovie(string file)
    {
        leftMovie = Resources.Load(file) as MovieTexture;
    }

    public void loadRightMovie(string file)
    {
        rightMovie = Resources.Load(file) as MovieTexture;
    }
}
