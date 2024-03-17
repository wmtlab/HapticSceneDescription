using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using HapticSceneDescription.GeomagicTouch;
using HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity;
using UnityEngine;
using UnityEngine.Networking;

namespace HapticSceneDescription.Gltf.Adapters
{
    public class HapticAdapter
    {
        public bool UseHapticDevice { get; set; } = false;
        private int _mediaIndex = -1;
        private List<int> _perceptionIndices = new List<int>();
        private float[] _signals;
        private int _currentSignalIndex = -1;
        private bool _isPlaying = false;
        private CancellationTokenSource _cts;
        private GeomagicTouchController _hapticSystem;
        public void TryPlay(SetHapticAction actionData)
        {
            if (_isPlaying)
            {
                Stop();
            }

            _mediaIndex = actionData.HapticActionNodes[0].HapticActionMedias[0].MediaIndex;
            _perceptionIndices = actionData.HapticActionNodes[0].HapticActionMedias[0].PerceptionIndices;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            PlayAsync(_cts.Token).Forget();
        }

        private async UniTask PlayAsync(CancellationToken token)
        {
            byte[] hmpg = await LoadHmpgFileAsync(_mediaIndex, token);
            _signals = Decode(hmpg);
            _currentSignalIndex = 0;
            _isPlaying = true;
        }

        public void Start()
        {
            if (UseHapticDevice)
            {
                _hapticSystem = new GeomagicTouchController();
                _hapticSystem.Start();
            }
        }

        public void Update()
        {
            if (!_isPlaying || _signals == null || _currentSignalIndex < 0)
            {
                return;
            }
            if (_currentSignalIndex >= _signals.Length)
            {
                if (UseHapticDevice)
                {
                    _hapticSystem.Stop();
                }
                _isPlaying = false;
                _mediaIndex = -1;
                return;
            }

            float signal = Mathf.Abs(_signals[_currentSignalIndex]);
            if (UseHapticDevice)
            {
                if (_perceptionIndices.Count > 0)
                {
                    foreach (var perceptionIndex in _perceptionIndices)
                    {
                        _hapticSystem.PlayValue(signal);
                    }
                }
                else
                {
                    _hapticSystem.PlayValue(signal);
                }
            }

            _currentSignalIndex++;
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _isPlaying = false;
            _mediaIndex = -1;
            if (UseHapticDevice)
            {
                _hapticSystem.OnDestroy();
            }
        }

        private async UniTask<byte[]> LoadHmpgFileAsync(int mediaIndex, CancellationToken token)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "HapticMedias", $"{mediaIndex}.hmpg");
            if (!File.Exists(path))
            {
                Debug.LogError($"Haptic media file not found: {path}");
                return null;
            }
            var req = UnityWebRequest.Get(path);
            await req.SendWebRequest().ToUniTask(cancellationToken: token);
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load haptic media file: {req.error}");
                return null;
            }
            return req.downloadHandler.data;
        }

        private static float[] Decode(byte[] hmpg)
        {
            IntPtr output = IntPtr.Zero;
            int outputSize = 0;
            int result = Decode(hmpg, hmpg.Length, ref output, ref outputSize);
            float[] signals = new float[outputSize];
            Marshal.Copy(output, signals, 0, outputSize);
            Marshal.FreeHGlobal(output);
            return signals;
        }

        [DllImport("MpegHapticCodec")]
        private static extern int Encode(float[] input, int inputSize, ref IntPtr output, ref int outputSize);

        [DllImport("MpegHapticCodec")]
        private static extern int Decode(byte[] input, int inputSize, ref IntPtr output, ref int outputSize);
    }
}