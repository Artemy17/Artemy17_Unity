
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Collections;
public class TimeSync : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    private readonly string url = "https://yandex.com/time/sync.json";

    public RectTransform _secondArrow;
    public RectTransform _minuteArrow;
    public RectTransform _hourseArrow;

    public TMP_InputField _hoursInputField;
    public TMP_InputField _minutesInputField;

    private Coroutine _updateCoroutine1;
    private TimeSpan currentTime;

    void Start()
    {
        _hoursInputField.characterLimit = _minutesInputField.characterLimit = 2;
        _hoursInputField.contentType = _minutesInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        StartCoroutine(FetchTime());

    }

    IEnumerator FetchTime()
    {
        // ������� UnityWebRequest, ����� ��������� ������ �� URL
        UnityWebRequest request = UnityWebRequest.Get(url);

        // �������, ���� ������ ����� ��������, ���������� ������ � ���� �����
        yield return request.SendWebRequest();

        // �������� ����� ������ �� �������
        string jsonResponse = request.downloadHandler.text;

        // ������������� ���������� JSON-����� � ������ TimeResponse
        TimeResponse response = JsonUtility.FromJson<TimeResponse>(jsonResponse);

        // ��������� �������� ������� � ������������� �� ������� TimeResponse
        long milliseconds = response.time;

        // ����������� ������������ � �������� ������ �������    
        currentTime = TimeSpan.FromMilliseconds(milliseconds);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", currentTime.Hours, currentTime.Minutes, currentTime.Seconds);

        // ��������� �������� ������ � ���������� TextMeshPro ������� ������������������ �������
        textMeshPro.text = formattedTime;

        // ��������� �������� UpdateTimerContinuously ��� ����������� ���������� �������
        StartCoroutine(UpdateTimerContinuously(currentTime));

      
        yield return null;
    }

    public void UpdateTimeManual()
    {

        if (int.TryParse(_hoursInputField.text, out int hours) && int.TryParse(_minutesInputField.text, out int minutes))
        {
            currentTime = new(hours, minutes, 0);

        }
        else
        {
            Debug.Log("�������� ������ �������");
        }
    }

    

    IEnumerator UpdateTimerContinuously(TimeSpan initialTime)
    {

        currentTime = initialTime;

        while (true)
        {

            currentTime = currentTime.Add(new TimeSpan(0, 0, 1)); // ��������� ����� �� 1 �������
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", currentTime.Hours, currentTime.Minutes, currentTime.Seconds);

            textMeshPro.text = formattedTime;

            float seconds = currentTime.Seconds;
            float rotationDegrees = seconds * -6f; // ������ ������� - 6 ��������
            _secondArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees);

            float minute = currentTime.Minutes;
            float rotationDegrees2 = minute * -6f; // ������ ������� - 6 ��������
            _minuteArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees2);

            float hours = currentTime.Hours;
            float rotationDegrees3 = hours * -30f; // ������ ������� - 30 ��������
            _hourseArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees3);


            yield return new WaitForSeconds(1f); // ���� ���� ������� ����� ��������� ����������� �������
        }

   
    }

  


    [System.Serializable]
    public class TimeResponse
    {
        public long time;
    }
}
