import hashlib
import base64

def hash(data: str) -> str:
    # 使用 SHA-256 生成哈希值
    sha256_hash = hashlib.sha256(data.encode()).digest()
    
    # 将哈希值进行 Base64 编码
    base64_encoded = base64.urlsafe_b64encode(sha256_hash)
    
    # 获得字符串并去掉结尾的等号 (填充字符)
    return base64_encoded.rstrip(b'=').decode('utf-8')