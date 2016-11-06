using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Friend_Speaking: MonoBehaviour {

	private AudioSource audio;
	private Animator animator;

	void Start() {
		audio = GetComponent<AudioSource> ();
	}

	IEnumerator SayVoiceText(string apiKey, string text) {

		string url = "https://api.voicetext.jp/v1/tts";
		WWWForm form = new WWWForm();
		form.AddField("speaker", "hikari");
		form.AddField("pitch", "140");
		form.AddField("speed", "130");
		form.AddField("emotion", "anger");
		form.AddField("emotion_level", "1");
		form.AddField("text", text);
		//Hashtable headers = form.headers;
		Dictionary<string, string> headers = form.headers;
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(apiKey + ":"));
		WWW www = new WWW(url, form.data, headers);
		//WWW www = new WWW(url, form.data);
		while( !www.isDone ) {
			yield return www;
		}

		if (www.error != null) {
			Debug.Log (www.error);
		} else {
			audio.clip = www.GetAudioClip(false, false, AudioType.WAV);
			audio.Play();
		}
	}

	void Update (){

		if (Input.GetKeyDown(KeyCode.A)) {
			StartCoroutine (SayVoiceText ("zjvjuxytwh1pkdob", "初めまして、烏丸です。よろしくね。"));
		}
	}
}
