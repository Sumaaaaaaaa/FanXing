from PIL import Image, ImageDraw, ImageFont


def add_text_to_image(image_path, text, name = None):
    # 打开图片
    image = Image.open(image_path)

    # 创建一个可以在给定图像上绘图的对象
    draw = ImageDraw.Draw(image)

    # 定义字体 (这里使用默认字体，你可以替换成任意有效的字体路径)
    # 例如: font = ImageFont.truetype("arial.ttf", size=50)
    font = ImageFont.truetype("msyhbd.ttc",60)

    # 文字颜色
    text_color = (255, 0, 0)  # 红色

    # 获取文字大小
    textLength = font.getlength(text)

    # 获取图片大小
    image_width, image_height = image.size

    # 计算文字在图片上的位置（中间）
    x = image_width / 2 - textLength / 2
    y = image_height / 2

    # 在图片上添加文字
    draw.text((x, y), text, fill=text_color, font=font)

    # 保存图片
    if name is None:
        image.save(f"{image_path[:len(image_path)-4]}_{text}.png")
    else:
        image.save(f"{name}.png")


# 调用函数
imagePath = "Sample.png"
Emotions = ["普通", "轻松"]

add_text_to_image(imagePath, "星之伊吕波_无奈", "星之伊吕波_无奈")
add_text_to_image(imagePath, "星之伊吕波_轻松", "星之伊吕波_轻松")
# 角色和其对应Sprite……星之伊吕波_无奈 / 观察员_普通 / 考生1_普通 / 考生2_普通 / 星之伊吕波_轻松 / 佐仓美野里_兴奋 / 观察员_普通 /
print(f"已经完成…………")
