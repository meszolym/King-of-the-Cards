import cv2 as cv
import numpy as np
from screeninfo import get_monitors
import pyscreenshot as ImageGrab

def get_primary_monitor_bbox() -> tuple[int, int, int, int]:
    monitor = get_monitors()[0]   # primary monitor
    left = monitor.x
    top = monitor.y
    right = monitor.x + monitor.width
    bottom = monitor.y + monitor.height
    return left, top, right, bottom

def take_screenshot() -> np.ndarray:
    """ Takes a screenshot of the primary monitor and returns it as a BGR numpy array. """
    try:
        print("Available backends:", ImageGrab.backends())
        bbox = get_primary_monitor_bbox()
        print(f"[INFO] Capturing screenshot with bbox: {bbox}")
        img_pil = ImageGrab.grab(bbox=bbox) #this is where it dies
        print("[INFO] Screenshot captured, converting to numpy array.")
        img_rgb = np.array(img_pil)
        print("[INFO] Converted to numpy array.")
        img_bgr = cv.cvtColor(img_rgb, cv.COLOR_RGB2BGR)
        print("[INFO] Screenshot captured of primary monitor.")
        return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed: {e}")
