import json
from guizero import select_file
import logging
import re

# logging设置
LOG_FORMAT = "%(asctime)s - %(levelname)s - %(message)s"
DATE_FORMAT = "%m/%d/%Y %H:%M:%S %p"
logging.basicConfig(level=logging.DEBUG, format=LOG_FORMAT, datefmt=DATE_FORMAT)


# 选择txt文件
textFilePath = select_file(
    title = "选择剧本txt文本",
    filetypes=[["Txt",".txt"]],
    save=False,
)
logging.info("选择txt文件...."+textFilePath)


# 打开文件并导入到一个string对象当中
text = ""
with open(textFilePath,'r',encoding='utf-8') as textFile:
    text = textFile.read()
logging.info("读取文件完成....读取到字符数量 = "+str(len(text)))

# 输出json 

# 基本数据结构
jsonResult = {
    "AssetsRequire":{
        "Characters":[],
        "BackGround":[]
    },
    "Dialogs":[] #dialog
}
dialog = {"name":"","dialog":"",
          "characterAndEmotion":[],#characterAndEmotion
          "background":""}
characterAndEmotion = {"Character":"","Emotion":""}

# 数据抓取
matches = re.findall(R"(?P<name>(?<=\n).+(?=：))：“?(?P<dialog>.+)”?", text)
for match in matches:
    dialog["name"] = match[0]
    dialog["dialog"] = match[1]
    jsonResult["Dialogs"].append(dialog)
print(jsonResult)

# 保存文件
with open("Scene1.json",'w',encoding='utf-8') as file:
    json.dump(jsonResult,file,indent=4, ensure_ascii=False,sort_keys=False)