using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySpeechToText.Services;

public class GoogleSpeechTest : MonoBehaviour {

    public GoogleStreamingSpeechToTextService m_SpeechToTextService;

	// Use this for initialization
	void Start () {
        m_SpeechToTextService.RegisterOnError(OnError);
        m_SpeechToTextService.RegisterOnTextResult(OnTextResult);
        m_SpeechToTextService.RegisterOnRecordingTimeout(OnRecordingTimeout);

        m_SpeechToTextService.StartRecording();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnError(string text)
    {
        Debug.LogError(text);
    }

    // Note that handling interim results is only necessary if your speech-to-text service is streaming.
    // Non-streaming speech-to-text services should only return one result per recording.
    void OnTextResult(SpeechToTextResult result)
    {
        if (result.IsFinal)
        {
            Debug.Log("Final result:");
        }
        else
        {
            Debug.Log("Interim result:");
        }
        for (int i = 0; i < result.TextAlternatives.Length; ++i)
        {
            Debug.Log("Alternative " + i + ": " + result.TextAlternatives[i].Text);
        }
    }

    void OnRecordingTimeout()
    {
        Debug.Log("Timeout");
    }
}
