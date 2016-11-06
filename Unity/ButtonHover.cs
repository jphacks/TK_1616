using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour {


    public int timeRemaining = 3;
    public Canvas canvas;
    public StoryManager storyMana;
    private string currentText;


    void Start()
    {
        storyMana = FindObjectOfType<StoryManager>();
        currentText = GetComponent<Text>().text;
    }

    void countDown()
    {
        //print(timeRemaining);
        timeRemaining--;
        //change text 
        Text text = GetComponent<Text>();
        text.text = currentText+timeRemaining.ToString();
        

        if (timeRemaining <= 0 )
        {            
            CancelInvoke("countDown");
            timeRemaining = 3;
            //print("reset time");
          
            if (storyMana.gameState == StoryManager.GameSates.SelfIntroduction)
            {
                storyMana.gameState = StoryManager.GameSates.DialogScene_1;
            }
            else if(storyMana.gameState == StoryManager.GameSates.DialogScene_1)
            {                            
                var currentObj = transform.name;
             //   print(currentObj);
                storyMana.currentButton = currentObj;
            }           
            var currentObj2 = transform.name;
            //   print(currentObj);
            storyMana.currentButton = currentObj2;
            Debug.Log(currentObj2);

        }
    }


    public void MouseOver()
    {
        InvokeRepeating("countDown", 1, 1);
        
    }

    public void MouseOut()
    {
        CancelInvoke("countDown");
        timeRemaining = 3;
    }
}
