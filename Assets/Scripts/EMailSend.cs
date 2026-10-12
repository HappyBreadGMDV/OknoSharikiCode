using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmailSender : MonoBehaviour
{
    public TMP_InputField message;
    public GameObject textAll;

    // Укажите URL, полученный при развёртывании
    private string scriptUrl = "https://script.google.com/macros/s/AKfycbwIzwTzAZtUw97RP2YYAAFQuXD4xLVpp9Xe1hvLHTiIGvavAGV2cUM5ciJ6DQxXoDml/exec";
    private string secretKey = "Happy"; // тот же ключ

    public void SendEmail()
    {
        if(message.text == "")
        {
            return;
        }
        StartCoroutine(SendEmailCoroutine("happybreadgdv@gmail.com", "Hi", message.text, "replyTo"));
        message.text = "";
    }

    private IEnumerator SendEmailCoroutine(string to, string subject, string body, string replyTo = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("key", secretKey);
        form.AddField("to", to);
        form.AddField("subject", subject);
        form.AddField("body", body);
        if (!string.IsNullOrEmpty(replyTo))
            form.AddField("replyTo", replyTo);

        using (UnityWebRequest www = UnityWebRequest.Post(scriptUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Письмо отправлено: " + www.downloadHandler.text);
                textAll.SetActive(true);
            }
            else
                Debug.LogError("Ошибка отправки: " + www.error + " | " + www.downloadHandler.text);

            Invoke("EnableTextAll", 1f);
        }
    }

    public void EnableTextAll()
    {
        textAll.SetActive(false);
    }
}