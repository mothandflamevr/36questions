using UnityEngine;
using System;
using System.Collections;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public class MicrophoneWorker
    {
        public event Action StartedRecordEvent;
        public event Action FinishedRecordEvent;
        public event Action RecordFailedEvent;

        private AudioClip _audioClip;
        private string _microphoneDevice;
        private bool _isLoop;
        private int _recordLength;
        private int _sampleRate;

        public bool IsCanWork { get; private set; }
        public bool IsRecording { get; private set; }

        #region ctors
        public MicrophoneWorker(int recordLength, bool isLoop, int sampleRate)
        {
            _recordLength = recordLength;
            _isLoop = isLoop;
            _sampleRate = sampleRate;

#if UNITY_WEBPLAYER
            SpeechRecognitionModule.Instance.StartCoroutine(MicrophoneAuthorization());
#else
            CheckMicrophones();
#endif
        }
#endregion

        public void StartRecord()
        {
            if (!IsCanWork)
            {
                if (RecordFailedEvent != null)
                    RecordFailedEvent();

                return;
            }

            if (_audioClip != null)
                MonoBehaviour.Destroy(_audioClip);

            _audioClip = Microphone.Start(_microphoneDevice, _isLoop, _recordLength, _sampleRate);
            IsRecording = true;

            if (StartedRecordEvent != null)
                StartedRecordEvent();
        }

        public void StopRecord()
        {
            if (!IsRecording)
                return;

            Microphone.End(_microphoneDevice);
            IsRecording = false;

            if (FinishedRecordEvent != null)
                FinishedRecordEvent();
        }

        public AudioClip GetRecordedAudio()
        {
            return _audioClip;
        }

        private IEnumerator MicrophoneAuthorization()
        {
            var auth = Application.RequestUserAuthorization(UserAuthorization.Microphone);
            while (!auth.isDone)
            {
                yield return auth;
            }

            CheckMicrophones();
        }

        private void CheckMicrophones()
        {
            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                IsCanWork = true;
            }
            else
            {
                Debug.Log("Microphone devices not found!");
                IsCanWork = false;
            }
        }
    }
}