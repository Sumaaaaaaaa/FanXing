using System;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MessageWindowManager.Message("This is a test");
        }
    }
}
