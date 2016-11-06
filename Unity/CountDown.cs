using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{


    private int timeRemaining = 3;
    //public Canvas canvas;
    public StoryManager_Ver2 storyMana;
   // private string currentText;
    public GameObject SpeechCountDown;
    private Text currentText;


    void Start()
    {
        storyMana = FindObjectOfType<StoryManager_Ver2>();
        //currentText = SpeechCountDown.GetComponent<Text>();
          
    }

     void SpeechRecCountDown()
    {
        //print(timeRemaining);
        Debug.Log("Count down" + timeRemaining);      
        timeRemaining--;
        //change text 
         
       // transform.GetComponent<Text>().text = timeRemaining.ToString();
        currentText.text = timeRemaining.ToString();


        if (timeRemaining <= 0) 
        {
            
            Debug.Log("start Speech Recongnition");
            timeRemaining = 3;
            storyMana._StartRecognition = true;
            storyMana._isRecongitionFinished = false;
            storyMana.ws_.Send("<start>");
            Debug.Log("sent");           
            SpeechCountDown.SetActive(false);
            
            CancelInvoke("SpeechRecCountDown");
            //print("reset time");       

        }
    }
   
    public void StartSpeechRecCountDown()
    {

        timeRemaining = 3;
        currentText = SpeechCountDown.GetComponent<Text>();
        currentText.text = timeRemaining.ToString();
        InvokeRepeating("SpeechRecCountDown", 1, 1);

    }

    public void MouseOut()
    {
        CancelInvoke("SpeechRecCountDown");
        timeRemaining = 3;
    }
}
