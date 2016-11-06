using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WebSocketSharp;
using System.Net;
using System.Text;
using System.IO;

public class InputFieldScript : MonoBehaviour
{
    public InputField iField;
    public Text currentText;
    public StoryManager_Ver2 story;
    public static string lineID;

    void Awake()
    {
        //lineID = "またすけ";
    }

     void Start()
    {
        //iField = GetComponent<InputField>();
        story = FindObjectOfType<StoryManager_Ver2>();
       // lineID = "またすけ";
    }
    public void MyFunction()
    {
        Debug.Log(iField.text);
      
    }
    public void Text_changed(string newText)
    {
        string lineID = newText;
        Debug.Log(newText);
    }
    public void Press()
    {
       lineID = iField.text;
       story.feedBack.AddField("userLineId", lineID);
       string feedBackString = story.feedBack.Print();
        Debug.Log(feedBackString);
        Debug.Log(lineID);
        //
        /*
        Debug.Log("Http request");
        WebRequest req2 = WebRequest.Create("http://localhost:3003/feedback");
        //HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://localhost:3003/feedback");
        req2.Method = "POST";
        byte[] byteArray = Encoding.UTF8.GetBytes(feedBackString);
        //req2.ContentType = "application/json";
        // Set the ContentLength property of the WebRequest.
        req2.ContentLength = byteArray.Length;
        // Get the request stream.
        Stream dataStream = req2.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();
        */
        Application.LoadLevel(1);
    }


    public string ReturnID()
    {
        return lineID;
    }
}