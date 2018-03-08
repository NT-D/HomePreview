using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class SwitchSkyBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        if (obj.state.source.handedness == InteractionSourceHandedness.Right
            && obj.pressType == InteractionSourcePressType.Select)
        {
            Console.WriteLine("ぽぽ！！ぽぴーーー！！！！ぽぺーー！");
        }
    }
    private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        if (obj.state.source.handedness == InteractionSourceHandedness.Right
            && obj.pressType == InteractionSourcePressType.Select)
        {

            Console.WriteLine("＊＊＊＊＊＊＊＊＊＊");
        }
    }
}
