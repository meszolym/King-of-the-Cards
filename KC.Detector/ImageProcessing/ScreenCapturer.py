import cv2 as cv
import numpy as np

try:
    import pyscreenshot as ImageGrab
    HAS_PYSCREENSHOT = True
except ImportError:
    HAS_PYSCREENSHOT = False
    raise ImportError("pyscreenshot is required. Install with: pip install pyscreenshot")


def take_screenshot() -> np.ndarray:
    if not HAS_PYSCREENSHOT:
        raise RuntimeError("pyscreenshot not available")

    try:
        img_pil = ImageGrab.grab()
        img_rgb = np.array(img_pil)
        img_bgr = cv.cvtColor(img_rgb, cv.COLOR_RGB2BGR)
        print("[INFO] Screenshot captured via pyscreenshot.")
        return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed: {e}")

