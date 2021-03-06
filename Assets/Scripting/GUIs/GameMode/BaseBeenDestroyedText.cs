﻿using UnityEngine;
using System.Collections;

public class BaseBeenDestroyedText : MonoBehaviour , IEventSubscriber
{
    public static bool BaseDestroyed = false;
	// Use this for initialization
	void Start ()
	{
	    BaseDestroyed = false;
        EventController.Instance.Subscribe("OnShowLoseScreen", this);
	}

    public void OnEvent(string EventName, GameObject Sender)
    {
        if (EventName == "OnShowLoseScreen")
        {
            renderer.enabled = BaseDestroyed;
        }
    }
}
