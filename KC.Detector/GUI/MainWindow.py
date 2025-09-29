from tkinter import *
from rx.subject import Subject

class MainWindow:
    roi_selected : bool = False
    detection_started : bool = False
    window : Tk
    start_detection_observable : Subject
    stop_detection_observable : Subject
    select_roi_observable : Subject
    show_rois_observable : Subject
    select_card_dimensions_observable : Subject

    def __init__(self, roi_sel: bool):
        self.roi_selected = roi_sel
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()
        self.select_roi_observable = Subject()
        self.show_rois_observable = Subject()
        self.select_card_dimensions_observable = Subject()

        self.window = Tk()
        self.window.title("KC Detector")
        self.window.geometry("300x200")

        self.title_label = Label(self.window, text="Welcome to KC Detector")
        self.title_label.pack(pady=(20, 10))

        self.roi_frame = Frame(self.window)
        self.select_roi_button = Button(
            self.roi_frame,
            text="Re-select ROIs" if self.roi_selected else "Select ROIs",
            command=self.select_roi
        )
        self.show_rois_button = Button(
            self.roi_frame,
            text="Show ROIs",
            command=self.show_rois
        )
        self.select_roi_button.pack(side="left")
        self.show_rois_button.pack(side="left", padx=(10, 0))
        self.roi_frame.pack(pady=10)

        self.card_select_button = Button(
            text="Select Card Dimensions", command=self.select_card_dimensions
        )
        self.card_select_button.pack(pady=10)

        self.control_frame = Frame(self.window)
        self.start_button = Button(
            self.control_frame, text="Start Detection", command=self.start_detection
        )
        self.stop_button = Button(
            self.control_frame, text="Stop Detection", command=self.stop_detection
        )
        self.start_button.pack(side="left", padx=(0, 10))
        self.stop_button.pack(side="left")
        self.control_frame.pack(pady=10)

        self.update_state()

    def update_state(self) -> None:
        self.start_button.config(
            state="normal" if self.roi_selected and not self.detection_started else "disabled"
        )
        self.stop_button.config(
            state="normal" if self.detection_started else "disabled"
        )
        self.select_roi_button.config(
            state="normal",
            text="Re-select ROIs" if self.roi_selected else "Select ROIs"
        )
        self.show_rois_button.config(
            state="normal" if self.roi_selected else "disabled"
        )

    def select_roi(self):
        self.select_roi_observable.on_next(None)
        return

    def start_detection(self):
        self.start_detection_observable.on_next(None)
        return

    def stop_detection(self):
        self.stop_detection_observable.on_next(None)
        return

    def show_rois(self):
        self.show_rois_observable.on_next(None)
        return

    def select_card_dimensions(self):
        self.select_card_dimensions_observable.on_next(None)
        return
