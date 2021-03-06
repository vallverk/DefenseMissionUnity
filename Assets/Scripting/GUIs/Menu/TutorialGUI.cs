﻿using UnityEngine;
using System.Collections;

public class TutorialGUI : GUIObject
{
    public GameObject Shasis;
    public GameObject Throtle;
    public GameObject Joystick;

    public Targeting Targeting;
    public Transform Target;

    public GameObject EnemyPointer;
    public Camera UICamera;

    private bool _showTakeoffText = true;

    public GameObject TargetArrowPrefab;
    private GameObject _arrow;
    private bool _tookOff;
    private bool _targetingActive;

    protected override void AwakeProc()
    {
        base.AwakeProc();
        SubscrabeOnEvents.Add("Landing");
        SubscrabeOnEvents.Add("Takeoff");
        SubscrabeOnEvents.Add("WheelsUp");
        SubscrabeOnEvents.Add("WheelsDown");
        SubscrabeOnEvents.Add("OnHideGUI");
        SubscrabeOnEvents.Add("ViewZoneEnter");
        SubscrabeOnEvents.Add("ViewZoneExit");
        SubscrabeOnEvents.Add("MissionFinished");

        SubscrabeOnEvents.Add("TargetingActive");
        SubscrabeOnEvents.Add("TargetingDeactive");
    }

    void Start()
    {
        if (!(TransportGOController.Instance.SelectedMissionID == 0))
        {
            GameObject.Destroy(gameObject);
            return;
        }
        _arrow = GameObject.Instantiate(TargetArrowPrefab) as GameObject; 
        _arrow.transform.parent = CameraController.Instance.gameObject.transform;
        _arrow.transform.localPosition = new Vector3(0, 6, -14.8f);
        _arrow.transform.localScale = new Vector3(7, 7, 1);
        _arrow.transform.localRotation = Quaternion.Euler(300, 0, 270);
        Color col = _arrow.transform.GetChild(0).renderer.material.GetColor("_Color");
        col.a = 0;
        _arrow.transform.GetChild(0).renderer.material.SetColor("_Color", col);
    }

    void Update()
    {
        UpdateTutorialUI();
        UpdateMissionArrow();
        UpdateEnemyPointer();

    }

    private void UpdateEnemyPointer()
    {
        var missionObject = AirplaneController.Instance.GetMissionObject();
        if (missionObject == null || !_tookOff || Shasis.activeSelf)
        {
            EnemyPointer.SetActive(false);
            return;
        }
        EnemyPointer.SetActive(true);

//        Vector3 pos = Camera3DInstance.Instance.camera.WorldToScreenPoint(missionObject.transform.position);
//        pos.z = 0;
//        pos = GUICameraController.Instance.camera.ScreenToWorldPoint(pos);
//        pos.z = 0;
       


//        Vector3 pos = Camera3DInstance.Instance.camera.WorldToScreenPoint(missionObject.transform.position);
//        pos.z = 0;
//        pos = GUICameraController.Instance.camera.ScreenToWorldPoint(pos);
//        pos = Targeting.transform.InverseTransformPoint(pos);
//        pos.z = 0;
//
//        EnemyPointer.transform.localPosition = pos;
        //EnemyPointer.transform.localPosition = new Vector3(EnemyPointer.transform.localPosition.x, EnemyPointer.transform.localPosition.y + 0.5f, 6f);

        EnemyPointer.transform.position = Target.position;
        EnemyPointer.transform.localPosition = new Vector3(EnemyPointer.transform.localPosition.x, EnemyPointer.transform.localPosition.y + 0.05f, EnemyPointer.transform.localPosition.z);

//        float m = pos.magnitude;
//        //Debug.Log(m);
//        if (m < 4f)
//        {
//            EnemyPointer.SetActive(true);
//
//        }
//        else
//        {
//            EnemyPointer.SetActive(false);
//            
//        }



    }

    private void UpdateMissionArrow()
    {
        var missionObject = AirplaneController.Instance.GetMissionObject();
        if (missionObject == null)
        {
            return;
        }
        _arrow.transform.localRotation = Quaternion.Euler(280, 0, 270);
        Vector3 b = missionObject.transform.position - CameraController.Instance.transform.position;
        b.y = 0;
        Vector3 f = CameraController.Instance.transform.forward;
        f.y = 0;
        float a = Vector3.Angle(f, b);
        f = CameraController.Instance.transform.right;
        f.y = 0;
        if (Vector3.Angle(f, b) > 90)
            a *= -1;
        _arrow.transform.Rotate(_arrow.transform.forward, a, Space.World);
    }

    private void UpdateTutorialUI()
    {
        if (!_tookOff)
        {
            if (AirplaneController.Instance.Speed <= 0.5f)
            {
                Shasis.SetActive(false);
                Throtle.SetActive(true);
                Joystick.SetActive(false);
            }
            else
            {
                Joystick.SetActive(true);
                Throtle.SetActive(false);
            }
        }
        else
        {
            Throtle.SetActive(false);
        }
    }

    protected override void EventProc(string EventName, GameObject Sender)
    {
        //base.EventProc(EventName, Sender);

        switch (EventName)
        {
            case "OnHideGUI":
                break;

            case "WheelsUp":
                _showTakeoffText = false;
                Shasis.SetActive(false);

                break;

            case "WheelsDown":
                Shasis.SetActive(false);
                break;

            case "Takeoff":
                if (_showTakeoffText)
                {
                    Shasis.SetActive(true);
                    Joystick.SetActive(false);
                    //Text.text = "Hide landing gear\nto increase speed";
                    _arrow.transform.GetChild(0).renderer.enabled = true;
                    Color col = _arrow.transform.GetChild(0).renderer.material.GetColor("_Color");
                    col.a = 1;
                    _arrow.transform.GetChild(0).renderer.material.SetColor("_Color", col);

                    _tookOff = true;
                }
                break;

            case "ViewZoneEnter":
                if (MissionController.Instance.CurrentState.Type == MissionStateType.Landing
                    && !AirplaneController.Instance.ChassisEnable)
                {
                   // Text.text = "Lower gear\nto land";
                   // Text.renderer.enabled = true;
                }
                break;

            case "ViewZoneExit":
                if (!_showTakeoffText)
                {
                  //  Text.renderer.enabled = false;
                }
                break;

            case "MissionFinished":
                _arrow.transform.GetChild(0).renderer.enabled = false;
                break;

            case "TargetingActive":
                _targetingActive = true;
                break;
            case "TargetingDeactive":
                _targetingActive = false;
                break;
        }
    }
}
