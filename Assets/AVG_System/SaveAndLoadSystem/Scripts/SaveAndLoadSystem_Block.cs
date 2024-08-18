using System;
using System.Collections;
using System.Collections.Generic;
using AVG_System.SaveAndLoadSystem;
using AVG_System.Scripts;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadSystem_Block : MonoBehaviour
{
    // 组件
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _title;

    // 自定义Block位置
    [SerializeField] private int position;
    [SerializeField] private Sprite nullImage;
    
    // 初始化根据存储更新所有内容
    private void Start()
    {
        SaveAndLoadSystemModel.Load(position,out var result,out var sprite);
        if (result.hasData)
        {
            Renew(ScriptsManager.Titles[result.scriptIndex],result.Time,sprite);
        }
    }

    public void Renew(string title, DateTime time, Sprite sprite)
    {
        _image.sprite = sprite ?? nullImage;
        _image.preserveAspect = true;
        _time.text = time.ToLocalTime().ToString("yyyy/M/d HH:mm");
        _title.text = title;
        return;
    }
    public void OnClick()
    {
        if (SaveAndLoadSystem.IsSaving)
        {
            // 保存模式
            ConfirmWindow.Confirm(() => { StartCoroutine(ConfirmAction_IE_Saving(SaveAndLoadSystem.Screenshot)); },"确认是否要保存吗。");
        }
        else
        {
            // 读取模式
            SaveAndLoadSystemModel.Load(position,out var result,out var sprite);
            if (!result.hasData)
            {
                MessageWindowManager.Message("无保存数据");
                return;
            }
            ConfirmWindow.Confirm(() => {SaveAndLoadSystem.Close();Title.Out();
                AVG_System.Scripts.AVG_System.BeginAuto(true,result.scriptIndex,result.dialogIndex,Title.In);
            },"确认要读取吗。");
        }
    }
    private IEnumerator ConfirmAction_IE_Saving(Texture2D screenShot)
    {
        ProcessWaitingWindow.SetActive(true);
        Time.timeScale = 0;
        var time = DateTime.UtcNow;
        // 模型层
        yield return SaveAndLoadSystemModel.SaveIE(ScriptsManager.Now,
                                                    AVG_System.Scripts.AVG_System.FraseNow,
                                                    ScriptsManager.NowTitle,
                                                    position,
                                                    time,
                                                    screenShot
                                                    );
        Time.timeScale = 1;
        ProcessWaitingWindow.SetActive(false);
        // 视觉层
        var sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), 
                                    new Vector2(0.5f, 0.5f));
        Renew(ScriptsManager.NowTitle,time,sprite);
    }
}
