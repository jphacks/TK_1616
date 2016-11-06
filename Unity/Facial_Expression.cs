using UnityEngine;
using System.Collections;

public class Facial_Expression : MonoBehaviour {

	public GameObject chitose;
	public MMD4MFaceController faceController;

	// Use this for initialization
	void Start () {
		faceController = chitose.GetComponent<MMD4MFaceController>();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if (Input.GetKeyDown(KeyCode.B)) {
			faceController.SetFace ("scorn");
            
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			faceController.SetFace ("happy");
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			faceController.SetFace ("normal");
		}
        */
	
	}

    public void test1(string fia)
    {
        Debug.Log(fia);
    }

    public IEnumerator StartFacialExpress(string fical, int num)
    {
       
        faceController.SetFace(fical);
        yield return new WaitForSeconds(num);
        faceController.SetFace("normal");
    }
}
