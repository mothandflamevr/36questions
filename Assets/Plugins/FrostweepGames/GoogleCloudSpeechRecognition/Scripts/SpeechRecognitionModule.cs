using UnityEngine;
using System;
using FrostweepGames.SpeechRecognition.Utilites;
using System.Text;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public class SpeechRecognitionModule : MonoBehaviour, ILowLevelSpeechRecognition
    {
        private const string API_KEY = "YOUR_OWN_GOOGLE_CLOUD_API_KEY"; // change into your own API key
        private const string SPEECH_RECOGNITION_REQUEST = "https://speech.googleapis.com/v1beta1/speech:syncrecognize";

        public event Action<RecognitionResponse> SpeechRecognizedSuccessEvent;
        public event Action<string> SpeechRecognizedFailedEvent;

        private static SpeechRecognitionModule _Instance;
        public static SpeechRecognitionModule Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject("[Singleton] Speech Recognition Module").AddComponent<SpeechRecognitionModule>();

                return _Instance;
            }
        }
        
        private int _sampleRate = 16000; // do not change it
        private string _requestUrl;
        private WWW _requestWWW;
        private RecognitionResponse _speechRecognitionResponse;
        private RecognitionRequest _speechRecognitionRequest;

        private MicrophoneWorker _microphoneWorker;
        private RuntimeSpeechDetection _runtimeSpeechDetector;

        public bool isDontDestroyGameObject = false;

        [Header("Speech Recognition Params")]
        [Tooltip("Your own Speech Recognition API Key")]
        public string customApiKey = "";
        public Enumerators.Language language = Enumerators.Language.EN_US;
        public bool isEnabledProfanityFilter = false;
        public int maxAlternatives = 10;
        public string[] speechContextPhrases;
        [HideInInspector]
        public Enumerators.AudioEncoding audioEncoding = Enumerators.AudioEncoding.LINEAR16;

        [HideInInspector]
        public bool isRecognitionProcessing = false;

        [Header("Recording Params")]
        [Tooltip("Record length in seconds")]
        public int recordLength = 10;
        public bool isLoopRecording = true;
        public bool isRuntimeDetection = false;
        public float runtimeSpeechDetectionTreshold = 0.005f;

        private void Awake()
        {
            if (_Instance == null)
                _Instance = this;

            if (isDontDestroyGameObject)
                DontDestroyOnLoad(gameObject);

            if (string.IsNullOrEmpty(customApiKey))
                customApiKey = API_KEY;

            _requestUrl = SPEECH_RECOGNITION_REQUEST + "?key=" + customApiKey;

            _microphoneWorker = new MicrophoneWorker(recordLength, isLoopRecording, _sampleRate);
            _microphoneWorker.StartedRecordEvent += StartedRecordEventHandler;
            _microphoneWorker.FinishedRecordEvent += FinishedRecordEventHandler;
            _microphoneWorker.RecordFailedEvent += RecordFailedEventHandler;

            _runtimeSpeechDetector = new RuntimeSpeechDetection(recordLength, _sampleRate, runtimeSpeechDetectionTreshold);
            _runtimeSpeechDetector.StartedRecordEvent += StartedRecordEventHandler;
            _runtimeSpeechDetector.FinishedRecordEvent += FinishedRecordEventHandler;
            _runtimeSpeechDetector.RecordFailedEvent += RecordFailedEventHandler;
        }

        private void Update()
        {
            if (isRuntimeDetection)
                _runtimeSpeechDetector.Update();

            if (isRecognitionProcessing)
            {
                if(_requestWWW != null && _requestWWW.isDone)
                {
                    if(string.IsNullOrEmpty(_requestWWW.error))
                    {
                        ProcessParseResponse(_requestWWW.text);
                    }
                    else
                    {
                        if (SpeechRecognizedFailedEvent != null)
                            SpeechRecognizedFailedEvent(_requestWWW.error);

                        Debug.Log("Speech Recognition have an error: " + _requestWWW.error);
                    }

                    isRecognitionProcessing = false;
                }
            }
        }

        public void StartRecord()
        {
            if (isRecognitionProcessing)
                return;

            _microphoneWorker.StartRecord();
        }

        public void StopRecord()
        {
            _microphoneWorker.StopRecord();
        }

        public void StartRuntimeRecord()
        {
            if (!isRuntimeDetection || isRecognitionProcessing)
                return;

            _runtimeSpeechDetector.StartRuntimeDetection();
        }

        public void StopRuntimeRecord()
        {
            if (!isRuntimeDetection)
                return;

            _runtimeSpeechDetector.StopRuntimeDetection();
        }

        public void SetSpeechContext(string[] phrases)
        {
            if (phrases != null)
                speechContextPhrases = phrases;
        }

        public void SetLanguage(Enumerators.Language language)
        {
            this.language = language;
        }

        public void Recognize(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.Log("Recorded Clip is null!");
                return;
            }

            byte[] buffer;
            PCMWrapper.AudioClipToPCMBytesArray(clip, out buffer);

            _speechRecognitionRequest = new RecognitionRequest();
            _speechRecognitionRequest.config.encoding = audioEncoding.ToString();
            _speechRecognitionRequest.config.languageCode = language.ToString().Replace("_", "-");
            _speechRecognitionRequest.config.sampleRate = _sampleRate;
            _speechRecognitionRequest.config.maxAlternatives = maxAlternatives;
            _speechRecognitionRequest.config.profanityFilter = isEnabledProfanityFilter;
            _speechRecognitionRequest.config.speechContext.phrases = speechContextPhrases;
            _speechRecognitionRequest.audio.content = Convert.ToBase64String(buffer);

            buffer = Encoding.UTF8.GetBytes(JsonUtility.ToJson(_speechRecognitionRequest));

            var form = new WWWForm();
            var headers = form.headers;

            headers["Method"] = "POST";
            headers["Content-Type"] = "application/json";
            headers["Content-Length"] = buffer.Length.ToString();

            _requestWWW = new WWW(_requestUrl, buffer, headers);

            isRecognitionProcessing = true;
        }

        private void ProcessParseResponse(string value)
        {
            if (value.Contains("error"))
            {
                if (SpeechRecognizedFailedEvent != null)
                    SpeechRecognizedFailedEvent(value);
            }
            else if (value.Contains("results"))
            {
                _speechRecognitionResponse = JsonUtility.FromJson<RecognitionResponse>(value);

                if (SpeechRecognizedSuccessEvent != null)
                    SpeechRecognizedSuccessEvent(_speechRecognitionResponse);
            }
            else if (value.Equals("{}"))
            {
                if (SpeechRecognizedFailedEvent != null)
                    SpeechRecognizedFailedEvent("Any Words Not Detected!");
            }
            else
            {
                if (SpeechRecognizedFailedEvent != null)
                    SpeechRecognizedFailedEvent("Process Parse Response failed with param: " + value);
            }
        }

        private void RecordFailedEventHandler()
        {
            Debug.Log("RecordFailedEventHandler");
        }

        private void FinishedRecordEventHandler()
        {
            Debug.Log("FinishedRecordEventHandler");

            if(isRuntimeDetection)
                Recognize(_runtimeSpeechDetector.GetRecordedAudio());
            else
                Recognize(_microphoneWorker.GetRecordedAudio());
        }

        private void StartedRecordEventHandler()
        {
            Debug.Log("StartedRecordEventHandler");
        }
    }
}