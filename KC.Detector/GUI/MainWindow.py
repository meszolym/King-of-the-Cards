from tkinter import *
from rx.subject import Subject

class MainWindow:
    roi_selected : bool = False
    detection_started : bool = False
    window : Tk
    start_detection_observable : Subject
    stop_detection_observable : Subject
    select_roi_observable : Subject

    def __init__(self, roi_sel: bool):
        self.roi_selected = roi_sel
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()
        self.select_roi_observable = Subject()

        self.window = Tk()
        self.window.title("KC Detector")
        self.window.geometry("300x300")

        self.label = Label(self.window, text="Welcome to KC Detector")
        self.label.pack(pady=20)

        self.select_roi_button = Button(self.window, text="Select ROI", state="normal" if not self.roi_selected else "disabled", command=self.select_roi)
        self.select_roi_button.pack(pady=10)

        self.start_button = Button(self.window, text="Start Detection", state="normal" if self.roi_selected and not self.detection_started else "disabled", command=self.start_detection)
        self.start_button.pack(pady=10)

        self.stop_button = Button(self.window, text="Stop Detection",
                                   state="normal" if self.detection_started else "disabled", command=self.stop_detection)
        self.stop_button.pack(pady=10)

    def update_state(self):
        self.start_button.config(state="normal" if self.roi_selected and not self.detection_started else "disabled")
        self.stop_button.config(state="normal" if self.detection_started else "disabled")
        self.select_roi_button.config(state="normal" if not self.roi_selected else "disabled")
        return

    def select_roi(self):
        self.select_roi_observable.on_next(None)
        return

    def start_detection(self):
        self.start_detection_observable.on_next(None)
        return

    def stop_detection(self):
        self.stop_detection_observable.on_next(None)
        return