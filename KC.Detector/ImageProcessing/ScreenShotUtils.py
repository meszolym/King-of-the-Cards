import platform
import numpy as np
from typing import Optional, Tuple


def take_screenshot(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    try:
        return _take_screenshot_mss(bbox)
    except ImportError:
        system = platform.system()
        if system == "Darwin":  # macOS
            return _take_screenshot_macos(bbox)
        elif system == "Windows":
            return _take_screenshot_windows(bbox)
        elif system == "Linux":
            return _take_screenshot_linux(bbox)
        else:
            raise RuntimeError(f"Unsupported platform: {system}")


def _take_screenshot_mss(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    import mss

    try:
        with mss.mss() as sct:
            if bbox:
                left, top, right, bottom = bbox
                monitor = {
                    "top": top,
                    "left": left,
                    "width": right - left,
                    "height": bottom - top
                }
                screenshot = sct.grab(monitor)
            else:
                # Capture the primary monitor
                monitor = sct.monitors[1]
                screenshot = sct.grab(monitor)

            # Convert BGRA to BGR numpy array
            img = np.array(screenshot)
            img_bgr = img[:, :, :3]  # Discard alpha channel
            return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed with MSS: {e}")


def _take_screenshot_windows(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    from PIL import ImageGrab

    try:
        img_pil = ImageGrab.grab(bbox=bbox)
        img_np = np.array(img_pil)
        # Convert RGB to BGR
        img_bgr = img_np[..., ::-1]
        return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed on Windows: {e}")


def _take_screenshot_macos(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    import subprocess
    import tempfile
    import os
    from PIL import Image

    try:
        with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
            tmp_path = tmp.name

        try:
            if bbox:
                left, top, right, bottom = bbox
                width = right - left
                height = bottom - top
                # screencapture syntax: -R left,top,width,height
                cmd = ["screencapture", "-R", f"{left},{top},{width},{height}", tmp_path]
            else:
                cmd = ["screencapture", tmp_path]

            subprocess.run(cmd, check=True, capture_output=True)

            img_pil = Image.open(tmp_path)
            img_np = np.array(img_pil)

            img_bgr = img_np[..., ::-1]
            return img_bgr
        finally:
            if os.path.exists(tmp_path):
                os.unlink(tmp_path)
    except Exception as e:
        raise RuntimeError(f"Screenshot failed on macOS: {e}")


def _take_screenshot_linux(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:

    backends = [
        ("gnome-screenshot", _gnome_screenshot),
        ("import", _imagemagick_screenshot),
        ("scrot", _scrot_screenshot),
    ]

    for backend_name, backend_func in backends:
        try:
            return backend_func(bbox)
        except Exception:
            continue

    # Final fallback: try PIL/pyscreenshot
    try:
        import pyscreenshot
        img_pil = pyscreenshot.grab(bbox=bbox)
        img_np = np.array(img_pil)
        img_bgr = img_np[..., ::-1]
        return img_bgr
    except Exception as e:
        raise RuntimeError(f"Screenshot failed on Linux: all backends failed: {e}")


def _gnome_screenshot(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    import subprocess
    import tempfile
    import os
    from PIL import Image

    with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
        tmp_path = tmp.name

    try:
        cmd = ["gnome-screenshot", "-f", tmp_path]
        if bbox:
            left, top, right, bottom = bbox
            width = right - left
            height = bottom - top
            cmd.extend(["-a", "-d", "0", str(left), str(top), str(width), str(height)])

        subprocess.run(cmd, check=True, capture_output=True, timeout=5)

        img_pil = Image.open(tmp_path)
        img_np = np.array(img_pil)
        img_bgr = img_np[..., ::-1]
        return img_bgr
    finally:
        if os.path.exists(tmp_path):
            os.unlink(tmp_path)


def _imagemagick_screenshot(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    import subprocess
    import tempfile
    import os
    from PIL import Image

    with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
        tmp_path = tmp.name

    try:
        if bbox:
            left, top, right, bottom = bbox
            width = right - left
            height = bottom - top
            cmd = ["import", "-window", "root", "-crop", f"{width}x{height}+{left}+{top}", tmp_path]
        else:
            cmd = ["import", "-window", "root", tmp_path]

        subprocess.run(cmd, check=True, capture_output=True, timeout=5)

        img_pil = Image.open(tmp_path)
        img_np = np.array(img_pil)
        img_bgr = img_np[..., ::-1]
        return img_bgr
    finally:
        if os.path.exists(tmp_path):
            os.unlink(tmp_path)


def _scrot_screenshot(bbox: Optional[Tuple[int, int, int, int]] = None) -> np.ndarray:
    import subprocess
    import tempfile
    import os
    from PIL import Image

    with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
        tmp_path = tmp.name

    try:
        if bbox:
            left, top, right, bottom = bbox
            width = right - left
            height = bottom - top
            cmd = ["scrot", "-c", "-o", tmp_path, "-s", f"{width}x{height}+{left}+{top}"]
        else:
            cmd = ["scrot", "-c", "-o", tmp_path]

        subprocess.run(cmd, check=True, capture_output=True, timeout=5)

        img_pil = Image.open(tmp_path)
        img_np = np.array(img_pil)
        img_bgr = img_np[..., ::-1]
        return img_bgr
    finally:
        if os.path.exists(tmp_path):
            os.unlink(tmp_path)
