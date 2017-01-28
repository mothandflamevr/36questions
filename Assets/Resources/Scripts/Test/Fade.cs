using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator FadeIn(float duration)
    {
            float startTime = Time.time;
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                Color color = GetComponent<Renderer>().material.color;
                GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, t);
                yield return null;
            }

    }

    public IEnumerator FadeOut(float duration)
    {

        // Fade out
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            Color color = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 1 - t);
            yield return null;
        }

    }
}
