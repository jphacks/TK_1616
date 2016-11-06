using UnityEngine;
using System.Collections;

public class CharacterAI : MonoBehaviour {


    Animator anim;
    public StoryManager_Ver2 story;
    private bool _isRandomBehaviorStarted;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        story = FindObjectOfType<StoryManager_Ver2>();
        _isRandomBehaviorStarted = false;




    }
	
    void Update()
    {
        if (!_isRandomBehaviorStarted)
        {
            if (story._isGameStarted || Input.GetKeyDown(KeyCode.C))
            {

                InvokeRepeating("RanddomBe", 2.0f, 4.0f);
                Debug.Log("start random behavior");
                _isRandomBehaviorStarted = true;
            }
        }
    }
    public void RanddomBe()
    {
        int num = Random.Range(0,2);
        
        anim.SetInteger("BehaviorPara", num);



        var num2 = Random.Range(0,4);
        if (num2 == 3)
        {
            anim.SetLayerWeight(1, 1.0f);
            int headNum = Random.Range(0, 4);
            Invoke("setHead",headNum);

        }
    }

    void setHead()
    {
        anim.SetLayerWeight(1, 0.0f);
    }
	// Update is called once per frame
	
}
