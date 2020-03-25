using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	[Header("Set in Inspecrot")]
	public float scrollSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
		if (Input.GetKeyDown("up")) {
			Vector3 temp = transform.position;
			temp.y += scrollSpeed;
			transform.position = temp;
		} else if (Input.GetKeyDown("down")) {
			Vector3 temp = transform.position;
			temp.y -= scrollSpeed;
			transform.position = temp;
		}
		if (Input.GetKeyDown("left")) {
			Vector3 temp = transform.position;
			temp.x -= scrollSpeed;
			transform.position = temp;
		} else if (Input.GetKeyDown("right")) {
			Vector3 temp = transform.position;
			temp.x += scrollSpeed;
			transform.position = temp;
		}
    }
}
