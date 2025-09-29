import pytesseract

from ImageProcessing.ScreenCapturer import take_screenshot
from Models.Enums import Message
import json
import cv2 as cv
from PIL import Image

class MessageProcessor:
    possible_messages : dict[str, Message]

    def __init__(self):
        self.possible_messages = {}

    def read_possible_messages(self, filepath: str):
        with open(filepath) as f:
            self.possible_messages = json.load(f)

    def process_message(self, img) -> Message:
        img_rgb = cv.cvtColor(img, cv.COLOR_BGR2RGB)
        pil_img = Image.fromarray(img_rgb)
        extracted_text = pytesseract.image_to_string(pil_img)
        print(extracted_text)
        for k,v in self.possible_messages.items():
            if k in extracted_text:
                return v

        return Message.Unknown