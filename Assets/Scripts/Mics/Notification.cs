using System.Collections;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public static Notification Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI notifyText;
    private bool notifying = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        notifyText.text = "";
    }

    public void DisplayMessage(string message, float displaySpeed, float messageExistTime)
    {
        if (notifying) return;

        notifying = true;
        notifyText.color = Color.white;
        StartCoroutine(TypeLine(message, displaySpeed, messageExistTime));
    }
    public void DisplayMessage(string message, float displaySpeed, float messageExistTime, Color color)
    {
        if (notifying) return;

        notifying = true;
        notifyText.color = color;
        StartCoroutine(TypeLine(message, displaySpeed, messageExistTime));
    }

    private IEnumerator TypeLine(string message, float displaySpeed, float messageExistTime)
    {
        var messageArray = message.ToCharArray();
        for (int i = 0; i < messageArray.Length; i++)
        {
            notifyText.text += messageArray[i];
            yield return new WaitForSeconds(displaySpeed);
        }

        yield return new WaitForSeconds(messageExistTime);
        notifyText.text = "";
        notifying = false;
    }
}
