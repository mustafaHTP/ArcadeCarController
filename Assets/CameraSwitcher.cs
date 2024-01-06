using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> _cameras = new();

    private const int LowPriorityCamera = 10;
    private const int HighPriorityCamera = 20;

    private int _currentCameraIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            if(i == _currentCameraIndex)
            {
                _cameras[i].Priority = HighPriorityCamera;
            }
            else
            {
                _cameras[i].Priority = LowPriorityCamera;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        int oldCameraIndex = _currentCameraIndex;

        ++_currentCameraIndex;
        if(_currentCameraIndex == _cameras.Count)
        {
            _currentCameraIndex = 0;
        }

        _cameras[oldCameraIndex].Priority = LowPriorityCamera;
        _cameras[_currentCameraIndex].Priority = HighPriorityCamera;
    }
}
