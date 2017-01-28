using FrostweepGames.SpeechRecognition.Utilites;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.SpeechRecognition.Google.Cloud.Examples
{
    public class SpeechControl : MonoBehaviour
    {
        private ILowLevelSpeechRecognition _speechRecognition;

        //private Text _speechRecognitionResult;
        private string speechRecognitionResult;

        private void Start()
        {
            _speechRecognition = SpeechRecognitionModule.Instance;
            _speechRecognition.SpeechRecognizedSuccessEvent += SpeechRecognizedSuccessEventHandler;
            _speechRecognition.SpeechRecognizedFailedEvent += SpeechRecognizedFailedEventHandler;

            //StartRecordButtonOnClickHandler();
            //StartCoroutine(StopAfter(15f));

            StartRuntimeDetectionButtonOnClickHandler();
        }

        private void OnDestroy()
        {
            _speechRecognition.SpeechRecognizedSuccessEvent -= SpeechRecognizedSuccessEventHandler;
            _speechRecognition.SpeechRecognizedFailedEvent -= SpeechRecognizedFailedEventHandler;
        }

        private void StartRuntimeDetectionButtonOnClickHandler()
        {
            //ApplySpeechContextPhrases();
            speechRecognitionResult = "";
            _speechRecognition.StartRuntimeRecord();
        }

        private void StopRuntimeDetectionButtonOnClickHandler()
        {
            _speechRecognition.StopRuntimeRecord();
            speechRecognitionResult = "";
        }

        private void StartRecordButtonOnClickHandler()
        {
            speechRecognitionResult = "";
            _speechRecognition.StartRecord();
        }

        private void StopRecordButtonOnClickHandler()
        {
            //ApplySpeechContextPhrases();
            _speechRecognition.StopRecord();
        }

        private void LanguageDropdownOnValueChanged(int value)
        {
            _speechRecognition.SetLanguage((Enumerators.Language)value);
        }

        private void IsRuntimeDetectionOnValueChangedHandler(bool value)
        {
            StopRuntimeDetectionButtonOnClickHandler();

            (_speechRecognition as SpeechRecognitionModule).isRuntimeDetection = value;
        }

        //private void ApplySpeechContextPhrases()
        //{
        //    string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

        //    if (phrases.Length > 0)
        //        _speechRecognition.SetSpeechContext(phrases);
        //}

        private void SpeechRecognizedFailedEventHandler(string obj)
        {
            speechRecognitionResult = "Speech Recognition failed with error: " + obj;
            Debug.Log("Speech Recognition failed with error: " + obj);
        }

        private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj)
        {

            if (obj != null && obj.results.Length > 0)
            {
                speechRecognitionResult = "Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript;

                string other = "\nDetected alternative: ";

                foreach (var result in obj.results)
                {
                    foreach (var alternative in result.alternatives)
                    {
                        if (obj.results[0].alternatives[0] != alternative)
                            other += alternative.transcript + ", ";
                    }
                }

                speechRecognitionResult += other;
                Debug.Log(speechRecognitionResult);
            }
            else
            {
                speechRecognitionResult = "Speech Recognition succeeded! Words are not detected.";
                Debug.Log(speechRecognitionResult);

            }
        }

        IEnumerator StopAfter(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            //StopRecordButtonOnClickHandler();
            StopRuntimeDetectionButtonOnClickHandler();
        }
    }
}