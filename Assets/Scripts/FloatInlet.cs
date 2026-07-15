using UnityEngine;
using LSL;
using System.Collections.Generic;

public class FloatInlet : MonoBehaviour
{
    [Tooltip("OpenVibe LSL Export에 적은 Stream Name과 똑같이 적어주세요! (예: OpenVibeEEG)")]
    public string streamName = "OpenVibeEEG";
    
    private StreamInlet inlet;
    private float[] sampleBuffer;
    public bool IsConnected { get; private set; } = false;

    void Update()
    {
        // 1. 아직 연결되지 않았다면 OpenVibe 스트림을 찾아서 연결합니다.
        if (inlet == null)
        {
            StreamInfo[] results = LSL.LSL.resolve_stream("name", streamName, 1, 0.0);
            if (results.Length > 0)
            {
                inlet = new StreamInlet(results[0]);
                sampleBuffer = new float[inlet.info().channel_count()];
                IsConnected = true;
                Debug.Log($"[LSL 연결 성공!] 스트림 이름: {streamName}, 채널 수: {sampleBuffer.Length}");
            }
        }
    }

    /// <summary>
    /// BCIManager가 매 프레임 호출하여 가장 최근의 뇌파 데이터 배열을 가져가는 함수입니다.
    /// </summary>
    public float[] GetLastSample()
    {
        if (inlet != null && IsConnected)
        {
            // 스트림에 쌓인 최근 데이터를 당겨옵니다 (0.0은 대기 없이 즉시 반환)
            double timestamp = inlet.pull_sample(sampleBuffer, 0.0);
            if (timestamp > 0.0)
            {
                return sampleBuffer;
            }
        }
        return null;
    }

    void OnDestroy()
    {
        if (inlet != null)
        {
            inlet.Dispose();
            inlet = null;
        }
    }
}