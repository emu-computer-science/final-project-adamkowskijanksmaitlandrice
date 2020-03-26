using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	[Header("Set in Inspecrot")]
	public float scrollSpeed = 0.05f;
	private enum scrollingDirection {
		neutral, up, down, left, right
	}
	private scrollingDirection xDirection;
	private scrollingDirection yDirection;

    // Start is called before the first frame update
    void Start()
    {
        xDirection = scrollingDirection.neutral;
		yDirection = scrollingDirection.neutral;
    }

    // Update is called once per frame
    void Update() {
		if (Input.GetKeyUp("up") || Input.GetKeyUp("down")) {
			yDirection = scrollingDirection.neutral;
		} else if (yDirection == scrollingDirection.up || Input.GetKeyDown("up")) {
			Vector3 temp = transform.position;
			temp.y += scrollSpeed;
			transform.position = temp;
			yDirection = scrollingDirection.up;
		} else if (yDirection == scrollingDirection.down || Input.GetKeyDown("down")) {
			Vector3 temp = transform.position;
			temp.y -= scrollSpeed;
			transform.position = temp;
			yDirection = scrollingDirection.down;
		}
		if (Input.GetKeyUp("left") || Input.GetKeyUp("right")) {
			xDirection = scrollingDirection.neutral;
		} else if (xDirection == scrollingDirection.left || Input.GetKeyDown("left")) {
			Vector3 temp = transform.position;
			temp.x -= scrollSpeed;
			transform.position = temp;
			xDirection = scrollingDirection.left;
		} else if (xDirection == scrollingDirection.right || Input.GetKeyDown("right")) {
			Vector3 temp = transform.position;
			temp.x += scrollSpeed;
			transform.position = temp;
			xDirection = scrollingDirection.right;
		}
    }
}
