
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
        // Создаем UnityWebRequest, чтобы отправить запрос по URL
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Ожидаем, пока запрос будет выполнен, отправляем запрос и ждем ответ
        yield return request.SendWebRequest();

        // Получаем текст ответа от сервера
        string jsonResponse = request.downloadHandler.text;

        // Десериализуем полученный JSON-ответ в объект TimeResponse
        TimeResponse response = JsonUtility.FromJson<TimeResponse>(jsonResponse);

        // Извлекаем значение времени в миллисекундах из объекта TimeResponse
        long milliseconds = response.time;

        // Преобразуем миллисекунды в читаемый формат времени    
        currentTime = TimeSpan.FromMilliseconds(milliseconds);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", currentTime.Hours, currentTime.Minutes, currentTime.Seconds);

        // Обновляем значение текста в компоненте TextMeshPro текстом отформатированного времени
        textMeshPro.text = formattedTime;

        // Запускаем корутину UpdateTimerContinuously для продолжения обновления времени
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
            Debug.Log("Неверный формат времени");
        }
    }

    

    IEnumerator UpdateTimerContinuously(TimeSpan initialTime)
    {

        currentTime = initialTime;

        while (true)
        {

            currentTime = currentTime.Add(new TimeSpan(0, 0, 1)); // Увеличить время на 1 секунду
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", currentTime.Hours, currentTime.Minutes, currentTime.Seconds);

            textMeshPro.text = formattedTime;

            float seconds = currentTime.Seconds;
            float rotationDegrees = seconds * -6f; // Каждая секунда - 6 градусов
            _secondArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees);

            float minute = currentTime.Minutes;
            float rotationDegrees2 = minute * -6f; // Каждая секунда - 6 градусов
            _minuteArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees2);

            float hours = currentTime.Hours;
            float rotationDegrees3 = hours * -30f; // Каждая секунда - 30 градусов
            _hourseArrow.transform.rotation = Quaternion.Euler(0f, 0f, rotationDegrees3);


            yield return new WaitForSeconds(1f); // Ждем одну секунду перед следующим обновлением времени
        }

   
    }

  


    [System.Serializable]
    public class TimeResponse
    {
        public long time;
    }
}
