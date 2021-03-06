﻿using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class Ads : GUIObject
{
    public int BlockID = -1;
    public string ImageURL = "";
    public string OpenURL = "";
    public static string JSONText = "";
    protected Texture2D AdTexture = null;

    void Start()
    {
        /*
        Vector3 p1 = transform.parent.InverseTransformPoint(GUICameraController.Instance.camera.ScreenPointToRay(
            new Vector3(0,0,0)).origin);
        Vector3 p2 = transform.parent.InverseTransformPoint(GUICameraController.Instance.camera.ScreenPointToRay(
            new Vector3(Screen.width,Screen.height,0)).origin);
        switch (BlockID)
        {
            case 1:
                // 300x480
                transform.localScale = new Vector3(
                    (p2.x-p1.x)*300/Screen.width,
                    (p2.y-p1.y)*480/Screen.height
                    );
                break;

            case 2:
                transform.localScale = new Vector3(
                    (p2.x-p1.x)*450/Screen.width,
                    (p2.y-p1.y)*450/Screen.height
                    );
                break;
        }
        */
        renderer.material.SetColor("_Color", new Color(1, 1, 1, 0));
        collider.enabled = false;

        if (JSONText != "loading..." && JSONText != "")
            EventController.Instance.PostEvent("OnLoadJson", null);
    }

    protected override void AwakeProc()
    {
        base.AwakeProc();
        SubscrabeOnEvents.Add("OnLoadJson");
        SubscrabeOnEvents.Add("OnErrorJson");
        SubscrabeOnEvents.Add("OnLoadTexture");
        SubscrabeOnEvents.Add("OnLoadedMenuScene");
        SubscrabeOnEvents.Add("OnPressObject");
    }

    protected override void EventProc(string EventName, GameObject Sender)
    {
        if (EventName == base.ShowOnEvent && renderer.material.GetColor("_Color").a==1)
            if (GoogleAnalytics.instance)
                GoogleAnalytics.instance.LogScreen("Show ad ["+gameObject.name+"]");
        switch (EventName)
        {
            case "OnLoadJson":
                StartCoroutine(LoadTexture());
                break;
                
            case "OnErrorJson":
                if (OptionsController.IASURL != "" && JSONText == "")
                {
                    JSONText = "loading...";
                    StartCoroutine(LoadJSON(10));
                }
                break;
                
            case "OnLoadTexture":
                if (Sender == gameObject && AdTexture)
                {
                    AdTexture.filterMode = FilterMode.Trilinear;
                    renderer.material.mainTexture = AdTexture;
                    renderer.material.SetColor("_Color",new Color(1,1,1,1));
                    if (renderer.enabled)
                        if (GoogleAnalytics.instance)
                            GoogleAnalytics.instance.LogScreen("Show ad ["+gameObject.name+"]");
                }
                break;

            case "OnLoadedMenuScene":
                if (OptionsController.IASURL != "" && JSONText == "")
                {
                    JSONText = "loading...";
                    StartCoroutine(LoadJSON());
                }
                break;

            case "OnPressObject":
                if (Sender == gameObject && this.OpenURL != "")
                {
                    Application.OpenURL(this.OpenURL);
                    if (GoogleAnalytics.instance)
                        GoogleAnalytics.instance.LogScreen("Click ad ["+gameObject.name+"]");
                }
                break;

            case "OnHideGUI":
                if (renderer.enabled)
                    StartCoroutine(LoadTexture());
                break;
        }
        base.EventProc(EventName, Sender);
    }

    IEnumerator LoadJSON(float WaitTime = 0) 
    {
        yield return new WaitForSeconds(WaitTime);
        WWW w = new WWW(OptionsController.IASURL);
        yield return w;
        if (w.error != null)
        {
            JSONText="";
            EventController.Instance.PostEvent("OnErrorJson", null);
        }
        else
        {
            JSONText = w.text;
            EventController.Instance.PostEvent("OnLoadJson", null);
        }
    }

    private int GetNext(string Name,int Max)
    {
        if (PlayerPrefs.HasKey(Name))
        {
            int num = PlayerPrefs.GetInt(Name);
            num++;
            if (num >= Max)
                num = 0;
            PlayerPrefs.SetInt(Name, num);
            return num;
        } else
        {
            PlayerPrefs.SetInt(Name,0);
            return 0;
        }
    }

    IEnumerator LoadTexture()
    {
        JSONNode j = JSON.Parse(JSONText);
        List<JSONNode> l = new List<JSONNode>();
        if (j != null)
        {
            JSONNode n = j ["slots"];
            for (int i=0; i<n.Count; i++)
                if (n [i] ["slotid"].Value.StartsWith(BlockID.ToString()))
                    l.Add(n [i]);
            n = l [GetNext(BlockID.ToString(), l.Count)];
            OpenURL = n ["adurl"];
            WWW www = new WWW(ImageURL = n ["imgurl"]);
            yield return www;
            if (www.error == null)
            {
                AdTexture = www.texture;
                EventController.Instance.PostEvent("OnLoadTexture", gameObject);
            } else
            {
                EventController.Instance.PostEvent("OnLoadTexture", gameObject);
            }
        }
    }
}
