using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;


public class StoryManager_Ver2 : MonoBehaviour
{
    public Transform castSpot;
    public Transform castSpot2;
    public GameObject[] dialogScenes;
    public GameObject speechImage;
    public GameObject speechRecognitionResultGUI;
    public GameObject speechRecognitionCountDown;
    public GameObject HelloWorld;
    public GameObject selfIntroduction_2;
    public GameObject dialogCanvas;
    public GameObject MainCharacter;
    public GameObject topics_Culinary;
    public GameObject askDinnerCanvas;
    public GameObject startCanvas;
    public GameObject []effectPrefab;
   
    public bool isDialogScreenActive; 
    public string currentButton;
    private string dialogChoice1;
    private string dialogChoice2;
    private string dialogChoice3;
    private int currentDialogSceneIndex;
    private bool isDialogSceneChanged;
    public Girl_Speaking agentSpeak;
    public Facial_Expression facialExpress;
    public InputFieldScript inputScript;
    public string[] lineArray = new string[30];
    private string line_2;
    public MMD4MFaceController faceController;
    private string facial_happy;
    private string facial_normal;
    private string facial_scorn;
    public bool _Speaking;
    public Text speechRecognitionResultString;
    public WebSocket ws_;
    public bool _StartRecognition;
    public bool _isSpeechGUIon;
    public bool _isRecongitionFinished;
    public bool _isTurningSpeaker;
    public string currentRecognitionResult = "";
    public bool _isTooLate;
    public bool _isGameStarted;
    public JSONObject feedBack;
    public JSONObject userPreference;
    public JSONObject disclosureReply;
    public JSONObject userHobby;
    public JSONObject cousineAsk;
    public JSONObject askDinner;
    public string feedBackString;
    public float point_sum;
    private string userLineID;
    

    public enum GameSates
    {
        Idle,
        HelloWorld,
        SelfIntroduction,
        Topics_Culinary,
        AskDinner,
        DialogScene_1,
        Topic_1,
        Topic_2,
        Topic_3,
        Topic_nonCommonTopic,
        Topic_CommonTopic,
        Topic_InvitingScene,        
        End
    }
    public enum SpeakingStates
    {
        AgentSpeakTurn,
        UserSpeakTurn,
        End
    }

    public enum Phrase
    {
        Phrase_S1,
        Phrase_S2,
        Phrase_S3,
        Phrase_S4,
        Phrase_S5,
        Phrase_U_Choice1,
        Phrase_U_Choice2,
        Phrase_Pause,
        End
    }
    public GameSates gameState;
    public SpeakingStates speakState;
    public Phrase phrase;
    // Use this for initialization

    void Awake()
    {
       

        lineArray[0] = "初めまして烏丸といいます、よろしくお願いします";
        lineArray[1] = "あなたの好きなものはなんですか？";
        lineArray[2] = "えーと、休日は何をされていますか？";
        lineArray[3] = "私は映画が好きです。他にも料理に興味があります";
        lineArray[4] = "日本食が好きですけど少し飽きました";
        lineArray[5] = "いいですね行きましょう";
        lineArray[6] = "また今後の機会にお願いします";
        lineArray[28] = "え？";
        lineArray[29] = "喋れーぽけー!";
        facial_happy = "happy";
        facial_normal = "normal";
        facial_scorn = "scorn";
       

        
    }

    void Start()
    {
        //feedBack data
        feedBack = new JSONObject(JSONObject.Type.OBJECT);
        
       
        Debug.Log(feedBackString);
        ////
        speechRecognitionResultString = speechRecognitionResultGUI.GetComponent<Text>();
        //speechRecognitionResultString.text = "";
        gameState = GameSates.Idle;
        speakState = SpeakingStates.AgentSpeakTurn;
        phrase = Phrase.Phrase_S1;
        agentSpeak = FindObjectOfType<Girl_Speaking>();
        //facialExpress =MainCharacter.GetComponent<Facial_Expression>();
        faceController =MainCharacter.GetComponent<MMD4MFaceController>();
        dialogCanvas.GetComponent<Canvas>();
        dialogChoice1 = "DialogChoice_1";
        dialogChoice2 = "DialogChoice_2";
        dialogChoice3 = "DialogChoice_3";
        isDialogScreenActive = false;
        currentDialogSceneIndex = 0;
        point_sum = 50f;
        isDialogSceneChanged = false;
        _StartRecognition = false;
        _Speaking = false;
        _isSpeechGUIon = false;
        _isRecongitionFinished = true;
        _isTooLate = false;
        _isTurningSpeaker = true;
        _isGameStarted = false;
        inputScript = FindObjectOfType<InputFieldScript>();
        userLineID =  inputScript.ReturnID();
        Debug.Log(userLineID);
        ////Websocket
        ws_ = new WebSocket("ws://localhost:3000");  //speech recon
        ws_.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };

        ws_.OnMessage += (sender, e) => {
            //speechRecognitionResultString.text = e.Data;
            if (e.Data == "<too_late>")
            {
                Debug.Log("too late");
                InvertIsTooLate(_isTooLate);
                         
            }
            else
            {
                currentRecognitionResult = e.Data;
                Debug.Log("message");
                //Debug.Log(sender + speechRecognitionResultString.text);
                Debug.Log(sender + currentRecognitionResult);
                InvertBool(_isRecongitionFinished);
            }

        };
        ws_.OnClose += (sender, e) =>
        {
            Debug.Log("connect close");
            // ws_.Connect();
        };

        ws_.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) ///send feedback report
        {
            //speechRecognitionResultString.text = currentRecognitionResult; //sync text
            /*
            string url = "http://localhost:3003/script-chat?reply-key=";
            //string apiKey = "zjvjuxytwh1pkdob";
            WWWForm form = new WWWForm();
            //form = feedBackString;
            Dictionary<string, string> headers = form.headers;
            headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(feedBackString + ":"));
            //Dictionary<string, string> headers = form.headers;
            //headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(apiKey + ":"));
            WWW www = new WWW(url, headers);
            
           */
            //saving report 
            feedBack.AddField("userPreference", userPreference);
            feedBack.AddField("disclosureReply", disclosureReply);
            feedBack.AddField("userHobby", userHobby);
            feedBack.AddField("cousineAsk", cousineAsk);
            feedBack.AddField("askDinner", askDinner);
            feedBack.AddField("sum", point_sum);
            feedBack.AddField("userLineId", userLineID);
            feedBackString = feedBack.Print();
            Debug.Log(feedBackString);

            //send request
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

            ///
            /*
            HttpWebResponse res2 = (HttpWebResponse)req2.GetResponse();
            Stream s2 = res2.GetResponseStream();
            
            StreamReader sr2 = new StreamReader(s2);
            string content2 = sr2.ReadToEnd();
            JSONObject json = new JSONObject(content2);
            */

        }
        if (_isTooLate)
        {
            StartCoroutine(TooLateReaction());
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            int num = UnityEngine.Random.Range(0, 4);
            CreateHeartEffect(num);
        }

        if (Input.GetMouseButtonDown(1)) //check connection state 
        {
            ws_.Send("<start>");
            //ws_.Send("unity");
            //InvertBool(_isRecongitionFinished);
            Debug.Log("sent");
        }
        switch (gameState)
        {
            case GameSates.Idle:
                if(Input.GetKeyDown(KeyCode.C) || _isGameStarted)
                {

                    startCanvas.SetActive(false);
                    if (!_isGameStarted)
                    {
                        _isGameStarted = true;
                    }
                    gameState = GameSates.HelloWorld;
                }
                break;

            case GameSates.HelloWorld:

                if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking )
                //start Hello world
                {
                    ShowDialogScreen(true);
                    HelloWorld.SetActive(true);
                   
                    //StartCoroutine(SwitchSpeechGUI(true, 5));
                    agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[0]));
                    int num = UnityEngine.Random.Range(3, 5);
                    // facialExpress.StartCoroutine(facialExpress.StartFacialExpress("happy", num));
                    StartCoroutine(StartFacialExpress(facial_happy, num));
                    // StartCoroutine(agentSpeak.SayVoiceText("happiness", line_Hello));

                }
                else if (speakState == SpeakingStates.UserSpeakTurn && !_Speaking)
                { //user turn

                    if (!_isSpeechGUIon)
                    {
                        StartCoroutine(SwitchSpeechGUI(true));
                        speechRecognitionResultString.text = "";
                        // _isSpeechGUIon = true;
                    }

                    else if (_StartRecognition)
                    {

                        speechRecognitionResultString.text = "音声認識中";


                        if (_isRecongitionFinished) //if speech recongnition finished
                        {
                            speechRecognitionResultString.text = currentRecognitionResult;
                            StartCoroutine("ResultShowTime", 3);
                            isDialogSceneChanged = false;
                            _StartRecognition = false;


                        }

                    }
                }
                break;

            case GameSates.SelfIntroduction:
                {
                    string userText = "スポーツとポケモンが好きです";

                    if (!isDialogSceneChanged && speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S1) //only once
                    {

                        selfIntroduction_2.SetActive(true);
                        HelloWorld.SetActive(false);
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[1]));
                        int num = UnityEngine.Random.Range(3, 5);
                        StartCoroutine(StartFacialExpress(facial_happy, num));
                        isDialogSceneChanged = true;

                    }

                    /*   if (_isSpeechGUIon)
                       {
                           Debug.Log("change scene");
                           StartCoroutine(SwitchSpeechGUI(false, 0));
                           // StartCoroutine(SwitchSpeechGUI(false, 0));
                           //StartCoroutine(SwitchSelftroduction(6));
                           isDialogSceneChanged = true;
                           _isSpeechGUIon = false;
                       }
                     */

                    else if (speakState == SpeakingStates.UserSpeakTurn && !_Speaking)
                    { //user turn

                        if (!_isSpeechGUIon)
                        {

                            StartCoroutine(SwitchSpeechGUI(true));
                            ShowDialogScreen(true);
                            speechRecognitionResultString.text = "";
                            // _isSpeechGUIon = true;
                        }


                        else if (_StartRecognition)
                        {

                            speechRecognitionResultString.text = "音声認識中";


                            if (_isRecongitionFinished) //if speech recongnition finished
                            {
                                speechRecognitionResultString.text = currentRecognitionResult;
                                StartCoroutine("ResultShowTime", 3);

                                _StartRecognition = false;

                                if (phrase == Phrase.Phrase_U_Choice1)
                                {
                                    /*  1. #{disclosureが含まれる発言} | 評価+20p @可能なら評価の極性判別
                                        2. #{相槌}(e.g. そうなんですね、へー) | 評価-10p
                                        3. #{complement of disclosure} | 評価-10p
                                        4. 時間制限超過 | 評価-10p
                                     */
                                    Debug.Log(phrase);
                                    JSONObject chatInfo = GetChatInfo("disclosure", currentRecognitionResult);
                                   // JSONObject chatInfo = GetChatInfo("preference", currentRecognitionResult);

                                    float point = chatInfo.GetField("point").n;
                                    CountSumPoint(point);
                                    string responsePharse = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("text").str);
                                    string actionDisclosure = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("action").str);
                                    Debug.Log(point);
                                    Debug.Log(responsePharse);
                                    _isTurningSpeaker = false;
                                    if (point > 0)
                                    {
                                        //action1
                                        disclosureReply = new JSONObject(JSONObject.Type.OBJECT);
                                        disclosureReply.AddField("action", actionDisclosure);
                                        disclosureReply.AddField("point", point);
                                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "3", responsePharse));
                                        int num = UnityEngine.Random.Range(3, 6);
                                        StartCoroutine(StartFacialExpress(facial_happy, num));
                                        int effectNum = UnityEngine.Random.Range(0, 4);
                                        CreateHeartEffect(effectNum);
                                    }
                                    else if(point < 0)
                                    {
                                        disclosureReply = new JSONObject(JSONObject.Type.OBJECT);
                                        disclosureReply.AddField("action", actionDisclosure);
                                        disclosureReply.AddField("point", point);
                                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("anger", "4", responsePharse));
                                        int num = UnityEngine.Random.Range(3, 6);
                                        StartCoroutine(StartFacialExpress(facial_scorn, num));

                                    }
                                    else if (_isTooLate)
                                    {
                                        point = -10;
                                        disclosureReply = new JSONObject(JSONObject.Type.OBJECT);
                                        disclosureReply.AddField("action", "action4");
                                        disclosureReply.AddField("point", point);
                                    }

                                    phrase = Phrase.Phrase_S4;

                                }

                                if (phrase == Phrase.Phrase_U_Choice2)
                                {
                                    //[hobby]をしています@自由回答/~をしていますというテンプレートを促す
                                    Debug.Log(phrase);

                                    phrase = Phrase.Phrase_S5;

                                }

                            }

                        }
                    }

                    else if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S2)
                    {
                        //speechRecognitionResultString.text = userText;
                        //normal version 
                        selfIntroduction_2.SetActive(false);
                        ShowDialogScreen(false);
                        JSONObject chatInfo = GetChatInfo("preference", currentRecognitionResult);
                        float point = chatInfo.GetField("point").n;
                        CountSumPoint(point);
                        string keyWord = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("kword").str); //key-word      
                        string responsePharse = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("text").str);

                        userPreference = new JSONObject(JSONObject.Type.OBJECT);
                        userPreference.AddField("keyword", keyWord);
                        userPreference.AddField("point", point);
                        Debug.Log(point);
                        Debug.Log(responsePharse);
                        Debug.Log(keyWord);
                        //speechRecognitionResultString.text = currentRecognitionResult +" "+ point+ "点";
                        _isTurningSpeaker = false;
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", responsePharse));

                        phrase = Phrase.Phrase_S3;
                        /*
                        ///test version
                        JSONObject chatInfo = GetChatInfo("preference",userText);
                        Debug.Log(chatInfo);
                        float point = chatInfo.GetField("point").n;                      
                        Debug.Log(point);                     
                        //speechRecognitionResultString.text = currentRecognitionResult + " " + point + "点";
                        _isTurningSpeaker = false;
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", "ポケモンですか、ポケモンはいいですね"));
                        phrase = Phrase.Phrase_S3;
                        */
                    }
                    else if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S3)
                    {
                        ///S: 私は[disclosure]が好きです。他にも[disclosure]に興味があります。                     
                        _isTurningSpeaker = true;
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[3]));
                        Debug.Log("Phrase3");
                        phrase = Phrase.Phrase_U_Choice1;

                    }

                    else if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S4)
                    {
                        ///えーと、休日は何をされていますか？  
                        ShowDialogScreen(false);
                        _isTurningSpeaker = true;
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[2]));
                        Debug.Log(phrase);
                        phrase = Phrase.Phrase_U_Choice2;

                    }
                    else if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S5)
                    {
                        ///[hobby]ですか、[hobby]には[いい印象があります！ +10p~ 20p|いい印象がないです... -20p~ -10p]
                        ShowDialogScreen(false);
                        Debug.Log(phrase);
                        JSONObject chatInfo = GetChatInfo("hobby", currentRecognitionResult);
                        float point = chatInfo.GetField("point").n;
                        CountSumPoint(point);
                        string keyWord = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("kword").str); //key-word      
                        string responsePharse = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("text").str);

                        userHobby = new JSONObject(JSONObject.Type.OBJECT);
                        userHobby.AddField("keyword", keyWord);
                        userHobby.AddField("point", point);
                        Debug.Log(point);
                        Debug.Log(responsePharse);
                        Debug.Log(keyWord);
                        _isTurningSpeaker = false;
                        //S: 上記選択肢/視線情報により表情変化
                        if (point == 10)
                        {
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "4", responsePharse));
                            int num = UnityEngine.Random.Range(3, 6);
                            StartCoroutine(StartFacialExpress(facial_happy, num));
                            int effectNum = UnityEngine.Random.Range(0, 4);
                            CreateHeartEffect(effectNum);
                        }
                        else if (point == 5)
                        {
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "3", responsePharse));
                            int num = UnityEngine.Random.Range(3, 6);
                            StartCoroutine(StartFacialExpress(facial_happy, num));
                            int effectNum = UnityEngine.Random.Range(0, 4);
                            CreateHeartEffect(effectNum);
                        }
                        else if (point == 0)
                        {
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "1", responsePharse));
                            int num = UnityEngine.Random.Range(3, 6);
                            StartCoroutine(StartFacialExpress(facial_normal, num));
                        }
                        else if (point == -5)
                        {
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("anger", "1", responsePharse));
                            int num = UnityEngine.Random.Range(3, 6);
                            StartCoroutine(StartFacialExpress(facial_scorn, num));
                        }
                        else if (point == -10)
                        {
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("anger", "3", responsePharse));
                            int num = UnityEngine.Random.Range(3, 6);
                            StartCoroutine(StartFacialExpress(facial_scorn, num));
                        }
                        phrase = Phrase.Phrase_Pause;

                    }
                    else if (!_Speaking && phrase == Phrase.Phrase_Pause)
                    {

                        StartCoroutine(PhrasePause(3));
                        if (isDialogSceneChanged)
                        {
                            isDialogSceneChanged = false;
                        }
                    }
                }
                break;

            case GameSates.Topics_Culinary:
                {
                    if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S1) //only once
                    {
                        //"日本食が好きですけど少し飽きました";
                        ShowDialogScreen(false);
                        topics_Culinary.SetActive(false);
                        Debug.Log(phrase);
                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[4]));
                        int num = UnityEngine.Random.Range(3, 5);
                        StartCoroutine(StartFacialExpress(facial_happy, num));
                        phrase = Phrase.Phrase_U_Choice1;

                    }
                    else if (speakState == SpeakingStates.UserSpeakTurn && !_Speaking)
                    { //user turn

                        if (!isDialogSceneChanged) //once
                        {                           
                            dialogCanvas.SetActive(true);
                            topics_Culinary.SetActive(true);
                            isDialogSceneChanged = true;
                            _isTurningSpeaker = true;
                        }
                            if (!_isSpeechGUIon)
                            {

                                StartCoroutine(SwitchSpeechGUI(true));
                                ShowDialogScreen(true);
                                speechRecognitionResultString.text = "";
                                // _isSpeechGUIon = true;
                            }


                            else if (_StartRecognition)
                            {

                                speechRecognitionResultString.text = "音声認識中";

                                if (_isRecongitionFinished) //if speech recongnition finished
                                {
                                    speechRecognitionResultString.text = currentRecognitionResult;
                                    StartCoroutine("ResultShowTime", 3);
                                    _StartRecognition = false;
                                if (phrase == Phrase.Phrase_U_Choice1)
                                {
                                    /*   1. #{日本食} |if 疑問文 ? #{はぐらかす-秘密です} : [あいづち]
                                         2. #{日本食以外の料理名が含まれる発言=cuisine}は好き？| イタリア料理/フランス料理/ロシア料理 ? [cuisine]も好きです : はあまり食べませんね
                                         3. #{相槌}(e.g. そうなんですね、へー) | 評価-10p
                                         4. #{complement of 料理名} | 評価-10p | 疑問文 ? #{はぐらかす-秘密です} : [あいづち]
                                         5. 時間制限超過 | 気まずいため評価-10p
                                     */
                                    Debug.Log(phrase);
                                    //JSONObject chatInfo = GetChatInfo("disclosure", currentRecognitionResult);
                                    JSONObject chatInfo = GetChatInfo("ask-cousine", currentRecognitionResult);
                                    float point = chatInfo.GetField("point").n;
                                    CountSumPoint(point);
                                    //Debug.Log(chatInfo);                                  
                                    string responsePharse = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("text").str);
                                    Debug.Log(point);
                                    Debug.Log(responsePharse);
                                   
                                    if (point >= 0)
                                    {
                                        //action1
                                        cousineAsk = new JSONObject(JSONObject.Type.OBJECT);
                                      
                                        cousineAsk.AddField("point", point);
                                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", responsePharse));
                                        int num2 = UnityEngine.Random.Range(3, 6);
                                        StartCoroutine(StartFacialExpress(facial_happy, num2));
                                    }
                                    else if(point <0)
                                    {
                                        cousineAsk = new JSONObject(JSONObject.Type.OBJECT);
                                       
                                        cousineAsk.AddField("point", point);
                                        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("anger", "3", responsePharse));
                                        int num2 = UnityEngine.Random.Range(3, 6);
                                        StartCoroutine(StartFacialExpress(facial_scorn, num2));
                                    }
                                    else if (_isTooLate)
                                    {
                                        point = -10;
                                        cousineAsk = new JSONObject(JSONObject.Type.OBJECT);
                                        cousineAsk.AddField("action", "action5");
                                        cousineAsk.AddField("point", point);
                                    }
                                    /*
                                    agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "3", responsePharse));
                                    int num = UnityEngine.Random.Range(3, 6);
                                    StartCoroutine(StartFacialExpress(facial_happy, num));
                                    */
                                    //if loop clear
                                    isDialogSceneChanged = false;

                                    gameState = GameSates.AskDinner;
                                   
                                   
                                  }
                                  }
                            }
                          }
                    }
                    break;

            case GameSates.AskDinner:
                {
                    if (speakState == SpeakingStates.AgentSpeakTurn && !_Speaking && phrase == Phrase.Phrase_S1) //only once
                    {
                        ShowDialogScreen(false);
                        Debug.Log(phrase);
                        JSONObject chatInfo = GetChatInfo("ask-dinner", currentRecognitionResult);
                        float point = chatInfo.GetField("point").n;
                        CountSumPoint(point);
                        //Debug.Log(chatInfo);                                  
                        string responsePharse = System.Text.RegularExpressions.Regex.Unescape(chatInfo.GetField("text").str);
                        if (point ==  10)
                        {
                            //行きましょう
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "4", lineArray[5]));
                            int num = UnityEngine.Random.Range(3, 7);
                            StartCoroutine(StartFacialExpress(facial_happy, num));
                            int effectNum = UnityEngine.Random.Range(0, 4);
                            CreateHeartEffect(effectNum);
                            CreateHeartEffect(effectNum);
                        }
                        else
                        {
                            //また今度
                            agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("happiness", "2", lineArray[6]));
                            int num = UnityEngine.Random.Range(3, 4);
                            StartCoroutine(StartFacialExpress(facial_normal, num));
                        }
                       phrase = Phrase.Phrase_S2;

                    }
                    else if (speakState == SpeakingStates.UserSpeakTurn && !_Speaking)
                    { //user turn

                        if (!isDialogSceneChanged) //once
                        {
                            ShowDialogScreen(true);
                            askDinnerCanvas.SetActive(true);
                            topics_Culinary.SetActive(false);
                            
                            isDialogSceneChanged = true;
                        }
                        if (!_isSpeechGUIon)
                        {

                            StartCoroutine(SwitchSpeechGUI(true));
                            ShowDialogScreen(true);
                            speechRecognitionResultString.text = "";
                            // _isSpeechGUIon = true;
                        }


                        else if (_StartRecognition)
                        {

                            speechRecognitionResultString.text = "音声認識中";

                            if (_isRecongitionFinished) //if speech recongnition finished
                            {
                                speechRecognitionResultString.text = currentRecognitionResult;
                                StartCoroutine("ResultShowTime", 3);
                                phrase = Phrase.Phrase_S1;
                                _StartRecognition = false;
                            }
                        }
                    }
                }
                break;
                    /*
                                case GameSates.DialogScene_1: //going to chose one topic 
                                    {
                                        if (!isDialogSceneChanged)
                                        {
                                            selfIntroduction.SetActive(false); // close selfIntroduction     
                                            ChangeDialogScene(currentDialogSceneIndex);
                                            Debug.Log("back to Scene1" + currentDialogSceneIndex);
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
                                        if (currentButton != null && isDialogSceneChanged && currentButton == dialogChoice2) //back to 1-2 1-3
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
                                        if (currentButton != null && isDialogSceneChanged)  //Topic2 to InvitingScene
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
                    */
                
                default:
                break;


                
        } }

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

    public void InvertBool(bool val)
    {
        Debug.Log("inverse");
        _isRecongitionFinished = true;
       // _StartRecognition = false
    }

    public void InvertIsTooLate(bool val)
    {
        _isTooLate = true;
        // _StartRecognition = false
    }

    IEnumerator SwitchSpeechGUI(bool set)
    {
        //yield return new WaitForSeconds(num);
        speechImage.SetActive(set);
        speechRecognitionCountDown.SetActive(set);
        speechRecognitionResultGUI.SetActive(set);
        _isSpeechGUIon = set;
        if (set)
        {
            //Debug.Log("strart count");
            speechRecognitionCountDown.GetComponent<CountDown>().StartSpeechRecCountDown();        
            set = !set;
        }
        yield return null; 
    }


    IEnumerator SwitchSelftroduction(int num)
    {
        speakState = SpeakingStates.UserSpeakTurn;
        yield return new WaitForSeconds(num);
        HelloWorld.SetActive(false);
        selfIntroduction_2.SetActive(true);
        Debug.Log("selfintro000");
        //StartCoroutine(SwitchSpeechGUI(true, 3));
        speechImage.SetActive(true);
        speechRecognitionCountDown.SetActive(true);
        speechRecognitionResultGUI.SetActive(true);
        speechRecognitionCountDown.GetComponent<CountDown>().StartSpeechRecCountDown();
        
      
    }

     public IEnumerator StartFacialExpress(string fical, int num)
    {
        Debug.Log("start ficalExp");
        faceController.SetFace(fical);
        yield return new WaitForSeconds(num);
        faceController.SetFace("normal");
    }
    IEnumerator PhrasePause(int num)
    {
        yield return new WaitForSeconds(num);
        if(gameState == GameSates.SelfIntroduction)
        {
            Debug.Log(phrase);
            gameState = GameSates.Topics_Culinary;
            speakState = SpeakingStates.UserSpeakTurn;
            phrase = Phrase.Phrase_S1;
        }
    }

    IEnumerator ResultShowTime(int time)
    {
        Debug.Log("showing recognition");
        yield return new WaitForSeconds(time);
        speakState = SpeakingStates.AgentSpeakTurn;
        if (gameState == GameSates.HelloWorld)
        {
            gameState = GameSates.SelfIntroduction;
        }
        else if (gameState == GameSates.SelfIntroduction)
        {
            if(phrase == Phrase.Phrase_S1)
            {
                phrase = Phrase.Phrase_S2;
            }
        }
        StartCoroutine(SwitchSpeechGUI(false));
    }

    IEnumerator StartRandomBehavior() //agent random behavior
    {

        yield return null;
    }

    

    public IEnumerator TooLateReaction()
    {
        agentSpeak.StartCoroutine(agentSpeak.SayVoiceText("anger","4", lineArray[28]));
        int num =UnityEngine.Random.Range(3, 5);
        StartCoroutine(StartFacialExpress(facial_scorn, num));
        _isTooLate = false;

        yield return null;
    }
    void CountSumPoint(float num)
    {
        point_sum += num;
        Debug.Log(point_sum);
    }

    private JSONObject GetChatInfo(string replyKey, string userText)
    {
        HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("http://localhost:3003/script-chat?reply-key="+replyKey+ "&text="+userText+"&sum-point="+point_sum);
        req2.Method = "GET";

        HttpWebResponse res2 = (HttpWebResponse)req2.GetResponse();

        Stream s2 = res2.GetResponseStream();
        StreamReader sr2 = new StreamReader(s2);
        string content2 = sr2.ReadToEnd();
        JSONObject json = new JSONObject(content2);

        return json;
    }
    void CreateHeartEffect(int num)
    {
        
            GameObject newSpell = (GameObject)Instantiate(effectPrefab[num], castSpot.position, castSpot.rotation);
            Destroy(newSpell, 4.0f);
          //  isMagicOn = true;
            //timeLimit = 5.0f;
           
    }

}
