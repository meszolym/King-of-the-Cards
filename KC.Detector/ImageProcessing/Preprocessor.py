#Do ROI, edge detection, etc here
import threading
import time
from threading import Thread

from Models.BoundingBox import BoundingBox
from Models.RoisContainer import RoisContainer
from rx.subject import Subject
from ImageProcessing.ScreenCapturer import  take_screenshot
import numpy as np

class Preprocessor:
    run : bool = False
    rois : RoisContainer
    message_image_observable : Subject
    dealer_image_observable : Subject
    player_image_observable : Subject
    worker_thread : Thread

    def __init__(self, rois: RoisContainer):
        self.rois = rois

        self.message_image_observable = Subject()
        self.dealer_image_observable = Subject()
        self.player_image_observable = Subject()
        return

    def start(self):
        if self.run:
            return

        self.worker_thread = threading.Thread(target=self.mainloop, daemon=True)
        self.worker_thread.start()

    def mainloop(self):
        self.run = True

        while self.run:
            img = take_screenshot().copy()

            self.dealer_image_observable.on_next(self.get_roi(img.copy(), self.rois.dealer_roi))
            self.player_image_observable.on_next(self.get_roi(img.copy(), self.rois.player_roi))
            self.message_image_observable.on_next(self.get_roi(img.copy(), self.rois.message_roi))
            time.sleep(1)
        return

    @staticmethod
    def get_roi(img: np.ndarray, box: BoundingBox) -> np.ndarray:
        """Extracts the ROI from the image based on the bounding box."""
        h, w, _ = img.shape
        x1 = int(box.x * w)
        y1 = int(box.y * h)
        x2 = int((box.x + box.w) * w)
        y2 = int((box.y + box.h) * h)
        return img[y1:y2, x1:x2]

    def stop(self):
        if self.worker_thread.is_alive():
            self.worker_thread.join(timeout=1.0)
        self.run = False
        return