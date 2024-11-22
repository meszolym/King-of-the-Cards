#This code is not in use, only added to git for reference (this is what the c# code is based on)

import sys
import pygetwindow as gw
import pyautogui
import time
from PyQt5.QtCore import Qt
from PyQt5.QtGui import QPainter, QColor
from PyQt5.QtWidgets import QApplication, QWidget
import cv2 as cv
import numpy as np

def getTopCards(img: cv.typing.MatLike) -> list:
    height,width = img.shape[:2]

    img = img[0:height//5*4,0:width]

    gray = cv.cvtColor(img, cv.COLOR_BGR2GRAY)

    canny = cv.Canny(gray,200,200)

    dilate = cv.dilate(canny,cv.getStructuringElement(cv.MORPH_ELLIPSE, (5,5)), iterations = 2)

    contours, hierarchy = cv.findContours(dilate, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)

    list = []

    for contour in contours:
        if cv.contourArea(contour) > 20000 and cv.contourArea(contour) < 50000 :
            epsilon = 0.1*cv.arcLength(contour,True)
            approx = cv.approxPolyDP(contour,epsilon,True)
            x, y, w, h = cv.boundingRect(contour)
            list.append((x,y,w,h))
    
    return list

class OverlayWindow(QWidget):
    def __init__(self, parent_window, rectangles):
        super().__init__()

        # Get the target window's position
        self.parent_window = parent_window
        rect = parent_window._rect
        self.left, self.top = rect.left, rect.top

        # Set overlay window size
        self.setGeometry(self.left, self.top, self.parent_window.width, self.parent_window.height)
        self.setWindowFlags(Qt.FramelessWindowHint | Qt.WindowStaysOnTopHint | Qt.Tool)
        self.setAttribute(Qt.WA_TranslucentBackground)  # Make the window background transparent
        self.setWindowOpacity(0.5)  # Set overlay opacity (optional)

        self.rectangles = rectangles  # List of (x, y, w, h) tuples

    def paintEvent(self, event):
        painter = QPainter(self)
        painter.setPen(QColor(255, 0, 0))  # Set rectangle color (red)
        painter.setBrush(QColor(255, 0, 0, 50))  # Set semi-transparent fill

        # Draw all the rectangles
        for rect in self.rectangles:
            x, y, w, h = rect
            painter.drawRect(x, y, w, h)

        painter.end()

    def update_rectangles(self, rectangles):
        self.rectangles = rectangles
        self.update()  # Trigger repaint

def select_window():
    windows = gw.getAllWindows()
    print("Select a window:")
    for i, window in enumerate(windows):
        print(f"{i}: {window.title}")

    choice = int(input("Enter the number of the window you want to capture: "))
    selected_window = windows[choice]
    return selected_window

def record_window(window):
    # Initial window capture
    while True:
        # Recheck the window's position and size in case it was moved
        rect = window._rect
        left, top, width, height = rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top

        # Capture the window's content
        screenshot = pyautogui.screenshot(region=(left, top, width, height))
        frame = np.array(screenshot)
        frame = cv.cvtColor(frame, cv.COLOR_RGB2BGR)

        # Get the rectangles (x, y, w, h) from your getTopCards function
        rectangles = getTopCards(frame)

        # Create and show overlay
        overlay = OverlayWindow(window, rectangles)
        overlay.show()

        # Update overlay with new rectangles
        overlay.update_rectangles(rectangles)

        # Exit if 'q' is pressed
        if cv.waitKey(1) & 0xFF == ord('q'):
            break

        # Wait before checking again to avoid overloading the CPU
        time.sleep(0.1)

    cv.destroyAllWindows()

if __name__ == '__main__':
    app = QApplication(sys.argv)
    selected_window = select_window()
    record_window(selected_window)
    sys.exit(app.exec_())