﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlaneSelecting_Buy : MonoBehaviour, IEventSubscriber
{
    public GameObject BuyButton;
    public GameObject Text;
    public GameObject PlaneCoin;
    public GameObject PlaneDollar;
    public GameObject NextButton;

    void Start()
    {
        EventController.Instance.Subscribe("OnShowBuyPlanePopup", this);
        EventController.Instance.Subscribe("OnHideGUI", this);
        EventController.Instance.Subscribe("OnPressBuyButton", this);

        var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmgMA4AV5/07AQhbU+X+E5CE845tAyG3HJlFjPpfrD5jKuOE1nyTEioUHaT168F2UN1O3I1AZZEi3s2XLouWgpNEvnEzGktTvCI61FCd+yTMZ3XTzjGpjEm4u4IWZJ/TF0M6WpYePcxSy93buFZzQD+hF1NGsAphKdtF+RmpOOorIQ/yD7w+blsyTxD7nrsG2EDrhS8byUehu9KxP8UI7KW8hudSsaxjqpdDc5ZXB97i6oXki2Aj6MQEfB9IwhRlOIH9tdTTNvO+XvSDbhHzQTazPCfj50lUiM+SRNN2YZsQvH1a0rI6nkAqFEf9ctWqHfXzZjCXfsepZylcCO5BTBQIDAQAB";
        GoogleIAB.init( key );
    }

    void HideGUI()
    {
        BuyButton.renderer.enabled = false;
        BuyButton.collider.enabled = false;
        Text.renderer.enabled = false;
        PlaneCoin.renderer.enabled = false;
        PlaneDollar.renderer.enabled = false;
    }

    #region IEventSubscriber implementation

    private static readonly Dictionary<Airplanes, int> _airplaneToCost = new Dictionary<Airplanes, int>()
    {
        {Airplanes.F_16, 5},
        {Airplanes.FA_22, 12000},
        {Airplanes.FA_38, 24000}
    };

    public void OnEvent(string EventName, GameObject Sender)
    {
//                for (int i = 0; i < TransportGOController.Instance.PlanesInfo.Length; i++)
//                {
//                    {
//                        TransportGOController.Instance.PlanesInfo[i].Locked = true;
//                        TransportGOController.Instance.PlanesInfo[i].Buyout = true;
//                    }
//                }
//        
//               EventController.Instance.PostEvent("OnSaveData", null);


        switch (EventName)
        {
            case "OnShowBuyPlanePopup":
                HideGUI();
                PlaneSelectButton s = Sender.GetComponent<PlaneSelectButton>();
                if (s)
                {
                    AirplaneInfo info = TransportGOController.GetPlaneInfo(s.SelectAirplane);
                    if (info.Locked)
                    {
                        BuyButton.collider.enabled = true;
                        BuyButton.renderer.enabled = true;
                        if (info.Buyout)
                        {
                            PlaneCoin.renderer.enabled = true;
                            Text.GetComponent<TextMesh>().text = _airplaneToCost[s.SelectAirplane].ToString();
                        }
                        else 
                        {
                            PlaneCoin.renderer.enabled = true;
                            Text.GetComponent<TextMesh>().text = info.BuyBack.ToString();
                        }
                        Text.renderer.enabled = true;

                        if (NextButton)
                        {
                            NextButton.collider.enabled = false;
                            NextButton.renderer.enabled = false;
                        }
                    }
                    else
                    {
                        if (NextButton)
                        {
                            NextButton.collider.enabled = true;
                            NextButton.renderer.enabled = true;
                        }
                    }
                }
                break;

            case "OnHideGUI":
                this.HideGUI();
                break;

            case "OnPressBuyButton":
                AirplaneInfo ainfo = TransportGOController.GetPlaneInfo(TransportGOController.Instance.SelectedPlane);
                if (ainfo.Locked)
                {
                    int playerMoney = OptionsController.Instance.PlayerMoney;
                    int planeCost = _airplaneToCost[ainfo.ID];

                    if (playerMoney - planeCost >= 0)
                    {
                        OptionsController.Instance.PlayerMoney -= planeCost;

                        for (int i=0;i< TransportGOController.Instance.PlanesInfo.Length;i++)
                        {
                            if (TransportGOController.Instance.PlanesInfo[i].ID == ainfo.ID)
                            {
                                TransportGOController.Instance.PlanesInfo[i].Locked = false;
                                TransportGOController.Instance.PlanesInfo[i].Buyout = false;
                            }
                        }

                        EventController.Instance.PostEvent("OnSaveData",null);
                        EventController.Instance.PostEvent("OnHideGUI",null);
                        EventController.Instance.PostEvent("OnShowAirplaneSelecting",null);

                        if (MissionController.Instance != null)
                        {
                            if(MissionController.Instance.Finished)
                            {
                                EventController.Instance.PostEvent("OnShowWinScreen", null);
                            }
                            else if(MissionController.Instance.Failed)
                            {
                                EventController.Instance.PostEvent("OnShowLoseScreen", null);
                            }
                        }
                    }

                    // _airplaneToCost


//                    if (ainfo.Buyout)
//                    {
//                        // goto purchase
//#if UNITY_EDITOR
//                        {
//                            for (int i=0;i< TransportGOController.Instance.PlanesInfo.Length;i++)
//                            {
//                                if (TransportGOController.Instance.PlanesInfo[i].ID == ainfo.ID)
//                                {
//                                    TransportGOController.Instance.PlanesInfo[i].Locked = false;
//                                    TransportGOController.Instance.PlanesInfo[i].Buyout = false;
//                                }
//                            }
//                            EventController.Instance.PostEvent("OnSaveData",null);
//                            EventController.Instance.PostEvent("OnHideGUI",null);
//                            EventController.Instance.PostEvent("OnShowAirplaneSelecting",null);
//                        }
//#else
//                        switch (ainfo.ID)
//                        {
//                            case Airplanes.FA_22:
//                                GoogleIAB.purchaseProduct( "airplane_f22" );
//                                break;
//
//                            case Airplanes.FA_38:
//                                GoogleIAB.purchaseProduct( "airplane_fa38" );
//                                break;
//                        }
//#endif
//                    }
//                    else 
//                    {
//                        if (ainfo.BuyBack <= OptionsController.Instance.PlayerMoney)
//                        {
//                            OptionsController.Instance.PlayerMoney-=ainfo.BuyBack;
//                            for (int i=0;i< TransportGOController.Instance.PlanesInfo.Length;i++)
//                            {
//                                if (TransportGOController.Instance.PlanesInfo[i].ID == ainfo.ID)
//                                {
//                                    TransportGOController.Instance.PlanesInfo[i].Locked = false;
//                                }
//                            }
//                            EventController.Instance.PostEvent("OnSaveData",null);
//                            EventController.Instance.PostEvent("OnHideGUI",null);
//                            EventController.Instance.PostEvent("OnShowAirplaneSelecting",null);
//                        }
//                    }
                }
                break;

        }
    }

    #endregion
}
