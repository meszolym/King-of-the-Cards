#Do ROI, edge detection, etc here
import threading
import time
from threading import Thread
from ImageProcessing.Utils import get_roi, take_screenshot
from Models.BoundingBox import BoundingBox
from Models.RoisContainer import RoisContainer
from rx.subject import Subject
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

            self.dealer_image_observable.on_next(get_roi(img.copy(), self.rois.dealer_roi))
            self.player_image_observable.on_next(get_roi(img.copy(), self.rois.player_roi))
            self.message_image_observable.on_next(get_roi(img.copy(), self.rois.message_roi))
            time.sleep(1)
        return

    def stop(self):
        if self.worker_thread.is_alive():
            self.worker_thread.join(timeout=1.0)
        self.run = False
        return