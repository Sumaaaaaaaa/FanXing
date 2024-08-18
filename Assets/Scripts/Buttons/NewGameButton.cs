using System.Collections;
using System.Collections.Generic;
using AVG_System.Scripts.Interface;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Title.Out();
        AVG_System.Scripts.AVG_System.BeginAuto(false, -1, -1, Title.In);
    }
}
