import numpy as np
from Models.Enums import Message
import json
import easyocr

class MessageProcessor:
    possible_messages : dict[str, Message]
    reader : easyocr.Reader

    def __init__(self):
        self.possible_messages = {}
        self.reader = easyocr.Reader(['en'])

    def read_possible_messages(self, filepath: str):
        with open(filepath) as f:
            self.possible_messages = json.load(f)

    def process_message(self, img : np.ndarray) -> Message:
        results = self.reader.readtext(img)

        extracted_text = ' '.join([text for (bbox, text, confidence) in results])
        print(extracted_text)

        for k, v in self.possible_messages.items():
            if k in extracted_text:
                return v

        return Message.Unknown