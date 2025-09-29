from screeninfo import get_monitors
from Models.BoundingBox import BoundingBox
import cv2 as cv
import numpy as np


def pick(window_name: str, img: np.ndarray) -> BoundingBox:
    tmp = img.copy()
    # Create a named window to show the prompt in its title bar
    cv.namedWindow(window_name, cv.WINDOW_NORMAL)
    cv.resizeWindow(window_name, int(get_monitors()[0].width * 0.9), int(get_monitors()[0].height * 0.9))
    cv.imshow(window_name, tmp)
    # Use selectROI with the window title
    x, y, w, h = cv.selectROI(window_name, tmp, showCrosshair=True)
    cv.destroyWindow(window_name)
    print(f"{window_name} picked: x={x}, y={y}, w={w}, h={h}")
    return BoundingBox(float(x), float(y),
                       float(w), float(h))