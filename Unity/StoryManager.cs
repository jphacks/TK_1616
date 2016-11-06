using UnityEngine;
using System.Collections;


public class StoryManager : MonoBehaviour
{

    public GameObject dialogCanvas;
    public GameObject dialogScene1;
    public GameObject selfIntroduction;
    public GameObject dialogScene2;
    public GameObject[] dialogScenes;
    public bool isDialogScreenActive;
    public string currentButton;
    private string dialogChoice1;
    private string dialogChoice2;
    private string dialogChoice3;
    private int currentDialogSceneIndex;
    private bool isDialogSceneChanged;


    public enum GameSates
    {
        SelfIntroduction,
        DialogScene_1,
        Topic_1,
        Topic_2,
        Topic_3,
        Topic_nonCommonTopic,
        Topic_CommonTopic,
        Topic_InvitingScene,
        Sportdislike_1_1,
        ChangeTopic_1_2,
        Animating,
        End
    }
    public enum SpeakingStates
    {
        AgentSpeakTurn,
        UserSpeakTurn,
        End
    }
    public GameSates gameState;
    public SpeakingStates speakState;
    // Use this for initialization
    void Start ()
    {
        gameState = GameSates.SelfIntroduction;
        speakState = SpeakingStates.AgentSpeakTurn;
        dialogCanvas.GetComponent<Canvas>();
        dialogChoice1 = "DialogChoice_1";
        dialogChoice2 = "DialogChoice_2";
        dialogChoice3 = "DialogChoice_3";
        isDialogScreenActive = false;
        currentDialogSceneIndex = 0;
        isDialogSceneChanged = false;

    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameSates.SelfIntroduction:

                if(Input.GetKeyDown(KeyCode.C)) //start selfIntroduction
                {
                    ShowDialogScreen(true);
                }
                
                break;
           
            case GameSates.DialogScene_1: //going to chose one topic 
                {
                    if (!isDialogSceneChanged)
                    {
                        selfIntroduction.SetActive(false); // close selfIntroduction     
                        ChangeDialogScene(currentDialogSceneIndex);
                        Debug.Log("back to Scene1"+currentDialogSceneIndex);
                        isDialogSceneChanged = true;
                        currentButton = null;
                    }
                    else if (currentButton != null && currentButton == dialogChoice1)
                    {
                        //ChoosePiece();
                        //gameState = GameSates.Topic_1;
                        Debug.Log(currentButton);
                        gameState = GameSates.Topic_1;
                        isDialogSceneChanged = false;

                    }
                    else if (currentButton != null && currentButton == dialogChoice2)
                    {
                        //ChoosePiece();
                        //gameState = GameSates.Topic_1;
                        Debug.Log(currentButton);
                        gameState = GameSates.Topic_2;
                        isDialogSceneChanged = false;
                    }
                    else if (currentButton != null && currentButton == dialogChoice3)
                    {
                        //ChoosePiece();
                        //gameState = GameSates.Topic_1;
                        Debug.Log(currentButton);
                        gameState = GameSates.Topic_3;
                        isDialogSceneChanged = false;
                    }
                }
                break;

            case GameSates.Topic_1:
                {                   
                    if (!isDialogSceneChanged) //enable Scene_Topic1
                    {
                        Debug.Log("chosen topic1");
                        UpdateDialogScene(currentDialogSceneIndex, 1);                        
                    }
                    if(currentButton!=null && isDialogSceneChanged && currentButton == dialogChoice2) //back to 1-2 1-3
                    {
                        dialogScenes[currentDialogSceneIndex].SetActive(false);
                        isDialogSceneChanged = false;
                        currentDialogSceneIndex = 0;
                        gameState = GameSates.DialogScene_1;
                        print("going to 1-2");
                        
                    }
                    if (currentButton != null && isDialogSceneChanged && currentButton == dialogChoice1) 
                    {
                        dialogScenes[currentDialogSceneIndex].SetActive(false);
                        isDialogSceneChanged = false;
                        
                        gameState = GameSates.Topic_InvitingScene;

                    }
                }
                break;

            case GameSates.Topic_2:
                {
                    if (!isDialogSceneChanged)
                    {
                        // Debug.Log("chosen topic2");
                        UpdateDialogScene(currentDialogSceneIndex, 2);                    
                    }
                    if(currentButton != null && isDialogSceneChanged)  //Topic2 to InvitingScene
                    {
                        dialogScenes[currentDialogSceneIndex].SetActive(false);
                        isDialogSceneChanged = false;
                        gameState = GameSates.Topic_InvitingScene;
                    }
                }
                break;

            case GameSates.Topic_3:
                {
                    if (!isDialogSceneChanged)
                    {
                        //Debug.Log("chosen topic1");
                        UpdateDialogScene(currentDialogSceneIndex, 3);                       
                    }
                    if (currentButton != null && isDialogSceneChanged)  //Topic2 to InvitingScene
                    {
                        dialogScenes[currentDialogSceneIndex].SetActive(false);
                        isDialogSceneChanged = false;
                        gameState = GameSates.Topic_InvitingScene;
                    }
                }
                break;
            case GameSates.Topic_nonCommonTopic:
                {
                    Debug.Log("enter noncommonTopic");
                    InvertBool(isDialogScreenActive);
                    ShowDialogScreen(isDialogScreenActive);  //close dialogWindow
                }
                break;


            case GameSates.Topic_InvitingScene:
                {
                    Debug.Log("enter Topic_InvitingScene");
                    InvertBool(isDialogScreenActive);
                    ShowDialogScreen(isDialogScreenActive);  //close dialogWindow
                    //ShowDialogScreen(false);  //close dialogWindow
                }

                break;

            default:
                break;
              
        }
        
    }

    public void ShowDialogScreen(bool show)
    {
       
       
        dialogCanvas.SetActive(show);
        
    }

    public void ChangeDialogScene(int index)
    {                     
        //dialogScene1.SetActive(true); // show up dialogScene1
        dialogScenes[index].SetActive(true);
   
    }


    public void UpdateDialogScene(int postScene, int currentScene) //now and to be
    {
        dialogScenes[postScene].SetActive(false);
        currentDialogSceneIndex = currentScene;
        ChangeDialogScene(currentDialogSceneIndex); //change to next scene
        isDialogSceneChanged = true;
        currentButton = null;
    }

    public void ChoiceReciver(int index)
    {

    }

    public static bool InvertBool(bool val)
    {
        return !val;
    }

}
