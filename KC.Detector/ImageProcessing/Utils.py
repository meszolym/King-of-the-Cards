import numpy as np
from screeninfo import get_monitors
from ImageProcessing.ScreenShotUtils import take_screenshot as su_take_screenshot
import cv2 as cv

from Models.BoundingBox import BoundingBox


def get_roi(img: np.ndarray, box: BoundingBox) -> np.ndarray:
    x1 = int(box.x)
    y1 = int(box.y)
    x2 = int((box.x + box.w))
    y2 = int((box.y + box.h))
    return img[y1:y2, x1:x2]


def _get_primary_monitor_bbox() -> tuple[int, int, int, int]:
    monitor = get_monitors()[0]  # primary monitor
    left = monitor.x
    top = monitor.y
    right = monitor.x + monitor.width
    bottom = monitor.y + monitor.height
    return left, top, right, bottom


def take_screenshot() -> np.ndarray:
    bbox = _get_primary_monitor_bbox()
    try:
        return su_take_screenshot(bbox)
    except Exception as e:
        raise RuntimeError(f"Screenshot failed: {e}")

def auto_canny(image, sigma=0.33):
    v = np.median(image)
    lower = int(max(0, (1.0 - sigma) * v))
    upper = int(min(255, (1.0 + sigma) * v))
    return cv.Canny(image, lower, upper)
