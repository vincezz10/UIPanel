﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour {

	public bool active;
	bool[] pressedButtons = {false, false, false, false, false, false};
	Color lineColor = Color.blue;

	Camera cam;
	Canvas canv;
	SceneManager sceneMgr;
	PointerManager ptrMgr;
	GestureMgr gstMgr;

	GameObject selected;
	GameObject panel;
	GameObject lineTop;
	GameObject lineBottom;
	Button scale, rotateX, rotateY, rotateZ, moveXY, moveZ;

	// Use this for initialization
	void Start () {
	
		active = false;

		cam = FindObjectOfType<Camera>();
		canv = FindObjectOfType<Canvas>();
		sceneMgr = FindObjectOfType<SceneManager> ();
		ptrMgr = FindObjectOfType<PointerManager> ();
		gstMgr = FindObjectOfType<GestureMgr> ();

        panel = GameObject.Find("Panel");

		scale = GameObject.Find ("Scale").GetComponent<Button> ();
		scale.onClick.AddListener (delegate { ToggleScale ();} );
		moveXY = panel.GetComponentsInChildren<Button> () [1];
		moveXY.onClick.AddListener (delegate { ToggleMotion ("XY");} );
		moveZ = panel.GetComponentsInChildren<Button> () [2];
		moveZ.onClick.AddListener (delegate { ToggleMotion ("Z");} );
		rotateX = panel.GetComponentsInChildren<Button> () [5];
		rotateX.onClick.AddListener (delegate { ToggleRotate ("X");} );
		rotateY = panel.GetComponentsInChildren<Button> () [4];
		rotateY.onClick.AddListener (delegate { ToggleRotate ("Y");} );
		rotateZ = panel.GetComponentsInChildren<Button> () [3];
		rotateZ.onClick.AddListener (delegate { ToggleRotate ("Z");} );

        //MUST DO BE HERE CUZ BEFORE BUTTON AND OTHERS OBJ CAN'T BE SEEN
        panel.SetActive(false);

    }
	
    public void setActive()
    {
        active = true;
    }
    public void setInactive()
    {
        active = false;
        for (int i = 0; i < panel.GetComponentsInChildren<Button>().Length; i++)
        {

            DisableButton(i);

        }
        gstMgr.disableScaling();
        gstMgr.disableRotating();
        ptrMgr.changeMovState(0);
        panel.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
		
		Vector3 dest1 = Vector3.zero;
		Vector3 dest2 = Vector3.zero;
		RectTransform rt = (RectTransform)panel.transform;
		selected = sceneMgr.getSelectedObject ();

		destroyLine (lineTop);
		destroyLine (lineBottom);

		if (selected != null && active) {

			dest1.x = rt.rect.width - 100; //this value is width/2 - posx
			dest1.y = panel.transform.position.y - (rt.rect.height / 2) + 150; //pointing a little bit toward the center
			dest1.z = canv.planeDistance;

			dest2.x = rt.rect.width - 100;
			dest2.y = panel.transform.position.y + (rt.rect.height / 2) - 30;
			dest2.z = canv.planeDistance;

            lineBottom = sceneMgr.drawLine(this.selected.transform.position, cam.ScreenToWorldPoint(dest1), lineColor, null);
            lineTop = sceneMgr.drawLine(this.selected.transform.position, cam.ScreenToWorldPoint(dest2), lineColor, null);

            panel.gameObject.SetActive(true);

        }
	
	}

	void destroyLine(GameObject line){

		GameObject.Destroy (line);

	}

	public void ToggleScale(){

		gstMgr.allowScaling();
		ToggleButton (0);

	}

	public void ToggleRotate(string axis){

		int n;
		RotateState rs;
		switch (axis) {

		case "X":
			n = 5;
			rs = RotateState.ROT_X;
			break;
		case "Y":
			n = 4;
			rs = RotateState.ROT_Y;
			break;
		case "Z":
		default:
			n = 3;
			rs = RotateState.ROT_Z;
			break;

		}

		gstMgr.allowRotating (rs);

		//adjust this with a dynamic way to get rotational button indexes
		for (int i = 0; i < 3; i++) {
			if (pressedButtons [i+3] && (i+3) != n)
				ToggleButton (i+3);
		
		}

		ToggleButton (n);


	}

	public void ToggleMotion(string plane){

		int n;
		switch (plane) {

		case "XY":
			n = 1;
			break;
		case "Z":
			n = 2;
			break;
		default:
			n = 0;
			break;

		}

		for (int i = 0; i < 2; i++) {
			if (pressedButtons [i + 1] && (i + 1) != n) {
				ToggleButton (i + 1);
			}

		}

		if(n != 0)
			ToggleButton (n);

		if (!pressedButtons [1] && !pressedButtons [2])
			n = 0;
		
		ptrMgr.changeMovState (n);

	}

	void ToggleButton(int buttonN){

		Button currButton = panel.GetComponentsInChildren<Button> () [buttonN];
		Sprite newSprite;
		Image currImage;
		string newImage;

		currImage = currButton.GetComponent<Image> ();

		newImage = (pressedButtons [buttonN]) ? currImage.name : currImage.name + "Pressed";
		pressedButtons [buttonN] = !pressedButtons [buttonN];

		newSprite = Resources.Load<Sprite>(newImage);
		currImage.sprite = newSprite;

	}

	void DisableButton(int buttonN){

		Button currButton = panel.GetComponentsInChildren<Button> () [buttonN];
		Sprite newSprite;
		Image currImage;
		string newImage;

		currImage = currButton.GetComponent<Image> ();

		newImage = currImage.name;
		pressedButtons [buttonN] = false;
		newSprite = Resources.Load<Sprite>(newImage);
		currImage.sprite = newSprite;

	}

}