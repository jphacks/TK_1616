using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Girl_Speaking : MonoBehaviour
{

    private new AudioSource audio;
    private Animator animator;
    public int counter = 0;
    public StoryManager_Ver2 story;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        story = FindObjectOfType<StoryManager_Ver2>();
    }

    public IEnumerator SayVoiceText(string emotion, string emotionLevel, string text)
    {
        story._Speaking = true;
        string url = "https://api.voicetext.jp/v1/tts";
        string apiKey = "zjvjuxytwh1pkdob";
        WWWForm form = new WWWForm();
        form.AddField("speaker", "hikari");
        form.AddField("pitch", "120");
        form.AddField("speed", "120");
        form.AddField("emotion", emotion);
        form.AddField("emotion_level", emotionLevel);
        form.AddField("text", text);
        //Hashtable headers = form.headers;
        Dictionary<string, string> headers = form.headers;
        headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(apiKey + ":"));
        WWW www = new WWW(url, form.data, headers);
        //WWW www = new WWW(url, form.data);
        while (!www.isDone)
        {
            yield return www;
        }

        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            audio.clip = www.GetAudioClip(false, false, AudioType.WAV);
            audio.Play();
            
            story.speakState = StoryManager_Ver2.SpeakingStates.AgentSpeakTurn;
            if (story._isTurningSpeaker) // turn speaker
            {
                StartCoroutine(SetSpeakingState(3.0f, true));
            }
            else
            {
                StartCoroutine(SetSpeakingState(2.5f, false));
            }
        }
    }



    IEnumerator SetSpeakingState(float num, bool isTurnSpeaker)
    {
        yield return new WaitForSeconds(num);
        story._Speaking = false;
        if (isTurnSpeaker)
        {
            story.speakState = StoryManager_Ver2.SpeakingStates.UserSpeakTurn;
            story.ShowDialogScreen(true);
        }
    }

    void Update()
    {


        /*

         if (Input.GetKeyDown(KeyCode.Z)) {

             //1章
             if (counter == 0){
                 StartCoroutine (SayVoiceText ("happiness", "初めまして烏丸といいます、よろしくお願いします"));
                 //apikeyが直接入力されていますが、githubに上げるときどうしましょうか…。
             }
             if (counter == 1){
                     StartCoroutine (SayVoiceText ("happiness", "ご飯食べたい"));
             }
             if (counter == 2){
                     StartCoroutine (SayVoiceText ("happiness", "清水さんですね。よろしくお願いします。"));
             }
             //2章
             if (counter == 3){
                     StartCoroutine (SayVoiceText ("sadness", "え、ちょっとスポーツは苦手で、あまりやらないです…"));
             }
             if (counter == 4){
                 StartCoroutine (SayVoiceText ("sadness", "昔ちょっと怪我をしてしまって、今でも少し激しい運動は苦手なんですよね。清水さんはスポーツが得意なんですか？"));
             }
             if (counter == 5){
                     StartCoroutine (SayVoiceText ("sadness", "へーそうなんですか。私スポーツ得意な人かっこいいと思いますよ。"));
             }
             if (counter == 6){
                     StartCoroutine (SayVoiceText ("happiness", "朝、辛いです。今日も婚活やめて家でごろごろしてようかな、なんて、思っちゃいました。"));
             }
             if (counter == 7){
                     StartCoroutine (SayVoiceText ("happiness", "あ〜、やっぱりそうなりますよね。私も休日はだいたいツタヤで映画借りて家でみたりしてます。清水さんは映画は好きですか"));
             }
             if (counter == 8){
                     StartCoroutine (SayVoiceText ("happiness", "へ〜、そうなんですか。気が合いますねー。"));
             }
             if (counter == 9){
                     StartCoroutine (SayVoiceText ("happiness", "そうですよね。出かけようかと思うのですけど家が気持ちよくて出かける気がおきないんですよね。清水さんはどこか出かけるのにオススメの場所とかありますか？"));
             }
             if (counter == 10){
                     StartCoroutine (SayVoiceText ("sadness", "へーそうなんですか。今度私も行ってみようかな。"));
             }
             if (counter == 11){
                     StartCoroutine (SayVoiceText ("happiness", "小田原ですか！私も地元がその近くなんですよ〜"));
             }
             if (counter == 12){
                     StartCoroutine (SayVoiceText ("happiness", "私は駅の近くのツタヤで映画を借りて家で見ています。最近だとアイアンマンを見ました。かっこよかったです。"));
             }
             if (counter == 14){
                     StartCoroutine (SayVoiceText ("sadness", "へーそうなんですか。なるほどなー"));
             }
             if (counter == 15){
                     StartCoroutine (SayVoiceText ("happiness", "他はアベンジャーズとかオススメです。シリーズ化されているので長く楽しめますよ。"));
             }
             if (counter == 16){
                     StartCoroutine (SayVoiceText ("happiness", "私、最近たくさん洋画を見ていて、今月は１００本も見ちゃったんですよ。"));
             }
             if (counter == 17){
                     StartCoroutine (SayVoiceText ("happiness", "えへへ。そんなことないですよ。清水さんは好きな映画とかありますか？"));
             }
             if (counter == 18){
                     StartCoroutine (SayVoiceText ("happiness", "へーそうなんですか。なるほどなー。"));
             }
             if (counter == 19){
                     StartCoroutine (SayVoiceText ("happiness", "そういえば私、最近料理が趣味なんですよ。昨日も自分１人の為に鍋とかつくちゃったんですよね。"));
             }
             if (counter == 20){
                     StartCoroutine (SayVoiceText ("happiness", "えへへ。そんなことないですよ。清水さんは得意な料理とかありますか。"));
             }
             if (counter == 21){
                     StartCoroutine (SayVoiceText ("happiness", "へーそうなんですか。なるほどなー。"));
             }
             if (counter == 22){
                     StartCoroutine (SayVoiceText ("happiness", "うーん、そうですね。わかりました。あとで連絡しますから、連絡先を教えてもらってもいいですか？"));
             }
             if (counter == 23){
                     StartCoroutine (SayVoiceText ("happiness", "こちらこそ。いろいろありがとうございました。"));
             }
             counter++;
         }
     }*/
    }
}
