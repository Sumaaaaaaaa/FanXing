"""
    该脚本只对应AVG演出表（版本：v1.1），使用不同的版本可能导致错误，请注意。
    角色和其表情最大可以查到Z列为止，有需求再修改这个限制。
"""

import logging.config
import openpyxl
from tkinter import filedialog
from tkinter import messagebox
import logging
import json 
from typing import Union
import HashFunc as HS

# 设定logging系统
logging.basicConfig(level=logging.DEBUG,
                    format = "%(asctime)s - %(levelname)s - %(message)s")


# 读取文件
def LoadFile() -> str:
    filePath = filedialog.askopenfilename(
        title="选择目标脚本文件",
        filetypes=[("Excel files", ".xlsx")]
    )
    if filePath == "":
        logging.error("未选择文件，请选择Excle文件")
        messagebox.showerror("未选择文件","未选择文件，请选择Excle文件")
        return None
    logging.info(f"选择脚本文件为 - {filePath}")
    return filePath


def ReadFile(filePath:str) -> Union[bool,list,str]:
    if filePath is None:
        raise ValueError("")
    # 读取表格文件
    file = openpyxl.load_workbook(filename=filePath)
    try:
        sheet = file['工作区']
    except KeyError:
        logging.error("选择并非标准的演出表文件，转换终止")
        messagebox.showerror("选择文件错误","选择并非标准的演出表文件，转换终止")
        return
    if sheet['A1'].value != "AVG演出表（版本：v1.1）":    
        logging.error("选择并非标准的演出表文件，转换终止")
        messagebox.showerror("选择文件错误","选择并非标准的演出表文件，转换终止")
        return False, None, None
    
    # 角色对应表创建
    charactersTable = {}
    inFieldCharacters = []
    for i in range(ord('G'), ord('Z')):
        # 如果表格不为空，说明有角色存在
        name = sheet[f"{chr(i)}5"].value
        if name != None:
            logging.debug(f"在第{chr(i)}5单元获得“{name}”角色存在，存储对应...{chr(i)}-{name}")
            charactersTable[str(chr(i))] = name
    logging.debug("完成角色对应表创建")
    
    # 创建空数据
    Dialogs = []

    # 获得表格的幕名
    sceneName = sheet['A2'].value
    if (sceneName is None) or (sceneName == "幕名："):
        logging.error("没有在A2单位填写幕名，无法完成转换，请填写幕名，")
        messagebox.showerror("幕名错误","没有在A2单位填写幕名，无法完成转换，请填写幕名，")
        return False, None, None
    if(sceneName[0:3]) != "幕名：":
        logging.error("幕名格式错误，没有以“幕名：”开头，请以“幕名：”开头，在后面写幕名")
        messagebox.showerror("幕名错误","幕名格式错误，没有以“幕名：”开头，请以“幕名：”开头，在后面写幕名")
        return False, None, None
    sceneName = sceneName[3:]
    logging.debug(f"获得到幕名...{sceneName}")
    
    # 主循环
    lineIndex = 6
    while True:
        # 追加数据
        name = "" if sheet[f'C{lineIndex}'].value is None else sheet[f'C{lineIndex}'].value 
        dialog = "" if sheet[f'D{lineIndex}'].value is None else sheet[f'D{lineIndex}'].value
        background = "" if sheet[f'A{lineIndex}'].value is None else sheet[f'A{lineIndex}'].value 
        CGImage = "" if sheet[f'B{lineIndex}'].value is None else sheet[f'B{lineIndex}'].value
        audio = "" if sheet[f'E{lineIndex}'].value is None else f"{sceneName}_{HS.hash(name+dialog)}"
        # 追加数据_角色和表情 + 屏幕角色数量统计检查
        characterAndEmotion = []
        for i in range(70, 91):
            if sheet[f"{chr(i)}{lineIndex}"].value is not None:
                character = charactersTable[chr(i)]
                emotion = sheet[f"{chr(i)}{lineIndex}"].value
                characterAndEmotion.append(character)
                characterAndEmotion.append(emotion)
                # 数量检查
                if character in inFieldCharacters:
                    if emotion.lower() == "out":
                        inFieldCharacters.remove(character)
                else:
                    if emotion.lower() == 'out':
                        logging.error(f"在第{chr(i)}{lineIndex}的单元，角色还没进入画面就使用了\'OUT\'，这是一个错误的使用方式")
                        messagebox.showerror("转换错误",f"在第{chr(i)}{lineIndex}的单元，角色还没进入画面就使用了\'OUT\'，这是一个错误的使用方式")
                        return False, None, None
                    inFieldCharacters.append(character)
        logging.debug(f"现在是第{lineIndex}行，在场角色数量为{len(inFieldCharacters)}")
        if len(inFieldCharacters) > 3:
            logging.error(f"在第{lineIndex}，同时在场的角色数量超过了三个，将不会进行脚本生成。")
            messagebox.showerror("转换错误",f"在第{lineIndex}，同时在场的角色数量超过了三个，将不会进行脚本生成。")
            return False, None, None
        dic = {"name":name,"dialog":dialog,"characterAndEmotion":characterAndEmotion,"background":background,"CGImage":CGImage,"audio":audio}
        Dialogs.append(dic)
        logging.debug(dic)
        
        # 最后增加行数1
        lineIndex += 1
        
        # 此时如果对应的这行的所有的相关内容为空，说明已经结束了
        isEmpty = True
        for i in range(ord('A'),ord('Z')):
            if sheet[f'{chr(i)}{lineIndex}'].value is not None:
                isEmpty = False
                break
        if isEmpty:
            logging.debug("判定行为全空，主程序结束")
            break
    return True, Dialogs, sceneName


def SaveJsonFile(dialogs:list, sceneName:str):
    filePath = filedialog.askdirectory(title="选择脚本文件保存位置")
    if filePath == "":
        logging.error("未选择保存的文件夹，转换程序终止")
        messagebox.showerror("未选择保存文件夹","未选择保存的文件夹，转换程序终止")
        return
    data = {"Dialogs":dialogs}
    filePath = filePath + '/' + sceneName +'.json'
    with open(filePath,'w',encoding='utf-8') as file:
        json.dump(data, file, ensure_ascii=False, indent=4)
        logging.info(f"完成转换，文件存储为{filePath}")
        messagebox.showinfo("完成",f"完成转换，文件存储为{filePath}")
        
if __name__=='__main__':
    result = ReadFile(LoadFile())
    if result[0]:
        SaveJsonFile(result[1],result[2])