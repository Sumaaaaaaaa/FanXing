import pyttsx3
import json
from guizero import select_file
engine = pyttsx3.init()
engine.setProperty('voice','HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech\Voices\Tokens\TTS_MS_ZH-CN_HUIHUI_11.0')


texts = [""]
filePath = select_file(
    title="选择剧本脚本文件",
    filetypes=[['Json', '.json']],
    save=False
)
with open(filePath, 'r', encoding="utf-8") as rawFile:
    file = json.load(rawFile)
    for i in file["Dialogs"]:
        if i["audio"] != "":
            engine.save_to_file(i['dialog'], f"{i['audio']}.mp3")
            print(f"{i['audio']}......{i['dialog']}")
    engine.runAndWait()
"""w
engine.save_to_file("测试测试测试测试测试", 'output.mp3')

engine.runAndWait()
"""