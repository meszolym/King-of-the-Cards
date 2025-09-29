import numpy as np
from rx.subject import Subject
from ImageProcessing.BoxPicker import pick
from Models.BoundingBox import BoundingBox
from Models.RoisContainer import RoisContainer
import cv2 as cv
from ImageProcessing.ScreenCapturer import take_screenshot
from screeninfo import get_monitors

class RoiSelector:
    rois_selected_observable: Subject

    def __init__(self):
        self.rois_selected_observable = Subject()
        return

    def open_roi_selector(self):
        # 1. Capture screenshot
        img = take_screenshot()  # BGR numpy array

        # 2. Prepare container
        base_img = img.copy()

        # 3. Generic ROI picker using window title only

        # 4. Pick ROIs with window titles
        rois = RoisContainer(
            dealer_roi=pick("Select Dealer ROI", img.copy()),
            player_roi=pick("Select Player ROI", img.copy()),
            message_roi=pick("Select Message ROI", img.copy()),
            base_image=base_img
        )

        # 5. Emit result
        self.rois_selected_observable.on_next(rois)
