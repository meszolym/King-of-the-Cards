import numpy as np
from rx.subject import Subject
from ImageProcessing.Utils import take_screenshot
from Models.BoundingBox import BoundingBox
from Models.CardSizesContainer import CardSizesContainer
from Models.RoisContainer import RoisContainer
import cv2 as cv
from screeninfo import get_monitors

class BoxSelector:
    rois_selected_observable: Subject
    card_box_selected_observable: Subject

    def __init__(self):
        self.rois_selected_observable = Subject()
        self.card_box_selected_observable = Subject()
        return

    @staticmethod
    def _pick_bounding_box(window_name: str, img: np.ndarray) -> BoundingBox:
        tmp = img.copy()
        # Create a named window to show the prompt in its title bar
        cv.namedWindow(window_name, cv.WINDOW_NORMAL)
        cv.resizeWindow(window_name, int(get_monitors()[0].width * 0.9), int(get_monitors()[0].height * 0.9))
        cv.imshow(window_name, tmp)
        # Use selectROI with the window title
        x, y, w, h = cv.selectROI(window_name, tmp, showCrosshair=True)
        cv.destroyWindow(window_name)
        print(f"{window_name} picked: x={x}, y={y}, w={w}, h={h}")
        return BoundingBox(x,y,w,h)

    def open_roi_selector(self):
        # 1. Capture screenshot
        img = take_screenshot()  # BGR numpy array

        # 2. Prepare container
        base_img = img.copy()

        # 3. Generic ROI picker using window title only

        # 4. Pick ROIs with window titles
        rois = RoisContainer(
            dealer_roi=self._pick_bounding_box("Select Dealer ROI", img.copy()),
            player_roi=self._pick_bounding_box("Select Player ROI", img.copy()),
            message_roi=self._pick_bounding_box("Select Message ROI", img.copy()),
            base_image=base_img
        )

        # 5. Emit result
        self.rois_selected_observable.on_next(rois)

    def open_card_box_selector(self):
        img = take_screenshot()  # BGR numpy array
        dealer_card_box = self._pick_bounding_box("Select Dealer Card", img.copy())
        player_card_box = self._pick_bounding_box("Select Player Card", img.copy())

        self.card_box_selected_observable.on_next(CardSizesContainer(dealer_card_box.w * dealer_card_box.h,
                                                                     player_card_box.w * player_card_box.h))
