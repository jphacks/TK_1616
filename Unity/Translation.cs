using UnityEngine;

public class Translation : MonoBehaviour
{
	void Update ()
	{
		float dx = Input.GetAxis ("Horizontal");
		float dy = Input.GetAxis ("Vertical");
		transform.Translate (dx * -0.1F, dy * 0.1F, 0.0F);
	}
}
