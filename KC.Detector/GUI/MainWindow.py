from tkinter import *
from rx.subject import Subject

class MainWindow:
    roi_selected : bool = False
    card_sizes_selected : bool = False
    detection_started : bool = False
    window : Tk
    start_detection_observable : Subject
    stop_detection_observable : Subject
    select_roi_observable : Subject
    show_rois_observable : Subject
    select_card_dimensions_observable : Subject
    read_rois_and_card_dimensions_json_observable : Subject
    write_rois_and_card_dimensions_json_observable : Subject

    def __init__(self, roi_sel: bool):
        self.roi_selected = roi_sel
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()
        self.select_roi_observable = Subject()
        self.show_rois_observable = Subject()
        self.select_card_dimensions_observable = Subject()
        self.read_rois_and_card_dimensions_json_observable = Subject()
        self.write_rois_and_card_dimensions_json_observable = Subject()

        self.window = Tk()
        self.window.title("KC Detector")
        self.window.geometry("300x320")
        self.window.minsize(300, 320)
        self.window.maxsize(300, 320)

        self.title_label = Label(self.window, text="Welcome to KC Detector")
        self.title_label.pack(pady=(20, 10))

        self.roi_frame = Frame(self.window)
        self.select_roi_button = Button(
            self.roi_frame,
            text="Re-select ROIs" if self.roi_selected else "Select ROIs",
            command=lambda: self.select_roi_observable.on_next(None)
        )
        self.show_rois_button = Button(
            self.roi_frame,
            text="Show ROIs",
            command=lambda: self.show_rois_observable.on_next(None)
        )
        self.select_roi_button.pack(side="left")
        self.show_rois_button.pack(side="left", padx=(10, 0))
        self.roi_frame.pack(pady=10)

        self.card_select_button = Button(
            text="Select Card Dimensions", command=lambda: self.select_card_dimensions_observable.on_next(None)
        )
        self.card_select_button.pack()

        self.card_sizes_label = Label(self.window, text="(Not selected)")
        self.card_sizes_label.pack(pady=10)

        self.read_json_button = Button(self.window, text="Read ROIs and dimensions from JSON", command=lambda: self.read_rois_and_card_dimensions_json_observable.on_next(None))
        self.read_json_button.pack()

        self.write_json_button = Button(self.window, text="Write ROIs and dimensions to JSON", command=lambda: self.write_rois_and_card_dimensions_json_observable.on_next(None))
        self.write_json_button.pack(pady=10)

        self.control_frame = Frame(self.window)
        self.start_button = Button(
            self.control_frame, text="Start Detection", command=lambda: self.start_detection_observable.on_next(None)
        )
        self.stop_button = Button(
            self.control_frame, text="Stop Detection", command=lambda: self.stop_detection_observable.on_next(None)
        )
        self.start_button.pack(side="left", padx=(0, 10))
        self.stop_button.pack(side="left")
        self.control_frame.pack(pady=10)

        self.update_state()

    def update_state(self) -> None:
        self.start_button.config(
            state="normal" if self.roi_selected and self.card_sizes_selected and not self.detection_started else "disabled"
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

        self.card_select_button.config(
            state="normal",
            text="Select Card Dimensions" if not self.card_sizes_selected else "Re-select Card Dimensions"
        )

        self.card_sizes_label.config(
            text="(Selected)" if self.card_sizes_selected else "(Not selected)"
        )

        self.write_json_button.config(
            state="normal" if self.roi_selected and self.card_sizes_selected else "disabled"
        )
