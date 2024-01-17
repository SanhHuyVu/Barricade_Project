using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 100;
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
