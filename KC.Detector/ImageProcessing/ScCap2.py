#THIS IS ONLY FOR TESTING PURPOSES, DON'T USE IT.
import os
import platform
import subprocess
import mss
from screeninfo import get_monitors
import cv2 as cv
import numpy as np
from screeninfo import get_monitors
import pyscreenshot as ImageGrab
import platform
import os
import subprocess
from PIL import Image

class ScreenCapturer:
    def __init__(self):
        self.backends = self.detect_environment()
        print(f"[INFO] Detected backends: {self.backends}")
        return
    @staticmethod
    def get_primary_monitor_bbox() -> tuple[int, int, int, int]:
        monitor = get_monitors()[0]  # primary monitor
        left = monitor.x
        top = monitor.y
        right = monitor.x + monitor.width
        bottom = monitor.y + monitor.height
        return left, top, right, bottom

    @staticmethod
    def detect_environment():
        """Detect the current environment and return the best backends"""
        system = platform.system()
        is_wayland = os.environ.get('XDG_SESSION_TYPE') == 'wayland'

        if system == 'Windows':
            return ['mss', 'pillow', None]
        elif system == 'Darwin':  # macOS
            return ['mss', 'pillow', None]
        elif system == 'Linux':
            if is_wayland:
                # Wayland - check which portal tools are available
                backends = []

                # Check for spectacle (KDE's screenshot tool)
                try:
                    subprocess.run(['which', 'spectacle'],
                                   check=True, capture_output=True)
                    backends.append('spectacle')
                except:
                    pass

                # Check for gnome-screenshot (works on KDE too via portals)
                try:
                    subprocess.run(['which', 'gnome-screenshot'],
                                   check=True, capture_output=True)
                    backends.append('gnome-screenshot')
                except:
                    pass

                backends.extend(['imagemagick', 'mss', None])
                return backends
            else:
                # X11 - all backends work
                return ['mss', 'scrot', 'imagemagick', 'pillow', None]

        return [None]

    def take_screenshot(self) -> np.ndarray:
        """ Takes a screenshot of the primary monitor and returns it as a BGR numpy array. """
        try:
            bbox = self.get_primary_monitor_bbox()
            print(f"[INFO] Capturing screenshot with bbox: {bbox}")
            print(f"[INFO] System: {platform.system()}, Session: {os.environ.get('XDG_SESSION_TYPE', 'unknown')}")

            # Get the best backends for this environment
            backends_to_try = self.backends
            print(f"[INFO] Will try backends: {backends_to_try}")

            for backend in backends_to_try:
                try:
                    print(f"[INFO] Trying backend: {backend if backend else 'auto'}")

                    # Use MSS directly when possible (fastest)
                    if backend == 'mss':
                        try:
                            with mss.mss() as sct:
                                monitor = {
                                    'left': bbox[0], 'top': bbox[1],
                                    'width': bbox[2] - bbox[0], 'height': bbox[3] - bbox[1]
                                }
                                screenshot = sct.grab(monitor)
                                img_pil = Image.frombytes("RGB", screenshot.size, screenshot.bgra, "raw", "BGRX")
                        except ImportError:
                            img_pil = ImageGrab.grab(bbox=bbox, backend='mss')
                    else:
                        img_pil = ImageGrab.grab(bbox=bbox, backend=backend)

                    print(f"[INFO] Success with backend: {backend if backend else 'auto'}")

                    img_rgb = np.array(img_pil)
                    img_bgr = cv.cvtColor(img_rgb, cv.COLOR_RGB2BGR)
                    print("[INFO] Screenshot captured and converted successfully.")
                    return img_bgr

                except Exception as e:
                    print(f"[ERROR] Backend '{backend}' failed: {e}")
                    continue

            raise RuntimeError("All backends failed to capture screenshot")

        except Exception as e:
            raise RuntimeError(f"Screenshot failed: {e}")

    # Fallback using MSS directly (most reliable cross-platform)
    def take_screenshot_mss(self) -> np.ndarray:
        """Ultra-reliable MSS-only version - install with: pip install mss"""
        import mss
        from PIL import Image

        bbox = self.get_primary_monitor_bbox()
        with mss.mss() as sct:
            monitor = {
                'left': bbox[0], 'top': bbox[1],
                'width': bbox[2] - bbox[0], 'height': bbox[3] - bbox[1]
            }
            screenshot = sct.grab(monitor)
            img_pil = Image.frombytes("RGB", screenshot.size, screenshot.bgra, "raw", "BGRX")
            img_rgb = np.array(img_pil)
            return cv.cvtColor(img_rgb, cv.COLOR_RGB2BGR)
