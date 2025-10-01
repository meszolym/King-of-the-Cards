import numpy as np
from screeninfo import get_monitors
import pyscreenshot as psc
import cv2 as cv

from Models.BoundingBox import BoundingBox


def get_roi(img: np.ndarray, box: BoundingBox) -> np.ndarray:
    """Extracts the ROI from the image based on the bounding box."""
    h, w, _ = img.shape
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
    """ Takes a screenshot of the primary monitor and returns it as a BGR numpy array. """
    try:
        bbox = _get_primary_monitor_bbox()
        print(f"[INFO] Capturing screenshot with bbox: {bbox}")
        img_pil = psc.grab(bbox=bbox)  #this is where it dies
        print("[INFO] Screenshot captured, converting to numpy array.")
        img_rgb = np.array(img_pil)
        print("[INFO] Converted to numpy array.")
        img_bgr = cv.cvtColor(img_rgb, cv.COLOR_RGB2BGR)
        print("[INFO] Screenshot captured of primary monitor.")
        return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed: {e}")
