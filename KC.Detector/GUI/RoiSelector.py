from rx.subject import Subject
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
        rois = RoisContainer()
        rois.base_image = img.copy()

        # 3. Generic ROI picker using window title only
        def pick(window_name: str) -> BoundingBox:
            tmp = img.copy()
            # Create a named window to show the prompt in its title bar
            cv.namedWindow(window_name, cv.WINDOW_NORMAL)
            cv.resizeWindow(window_name, int(get_monitors()[0].width*0.9), int(get_monitors()[0].height*0.9))
            cv.imshow(window_name, tmp)
            # Use selectROI with the window title
            x, y, w, h = cv.selectROI(window_name, tmp, showCrosshair=True)
            cv.destroyWindow(window_name)
            print(f"{window_name} picked: x={x}, y={y}, w={w}, h={h}")
            return BoundingBox(float(x), float(y),
                               float(w), float(h))

        # 4. Pick ROIs with window titles
        rois.dealer_roi  = pick("DEALER ROI")
        rois.player_roi  = pick("PLAYER ROI")
        rois.message_roi = pick("MESSAGE ROI")

        # 5. Emit result
        self.rois_selected_observable.on_next(rois)
