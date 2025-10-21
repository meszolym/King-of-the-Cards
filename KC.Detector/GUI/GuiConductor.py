from tkinter import filedialog
from typing import Optional

from GUI.Overlay import Overlay, OverlayModel
from GUI.MainWindow import MainWindow
from GUI.BoxSelector import BoxSelector
from rx.subject import Subject

from Models.CardSizesContainer import CardSizesContainer
from Models.OverlayModel import HandRecord
from Models.RoisContainer import RoisContainer
from GUI.RoiShower import show_rois

class GuiConductor:
    main_window : MainWindow
    overlay : Overlay
    box_selector : BoxSelector

    rois_selected_observable : Subject
    card_sizes_selected_observable : Subject
    start_detection_observable : Subject
    stop_detection_observable : Subject
    read_rois_and_card_dimensions_json_observable : Subject
    write_rois_and_card_dimensions_json_observable : Subject

    rois : Optional[RoisContainer]

    def __init__(self):
        self.main_window = MainWindow(False)
        self.overlay = Overlay(OverlayModel([], None, 0,0))
        self.overlay.run()
        self.overlay.hide_overlay()

        self.box_selector = BoxSelector()

        self.rois_selected_observable = Subject()
        self.card_sizes_selected_observable = Subject()
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()
        self.read_rois_and_card_dimensions_json_observable = Subject()
        self.write_rois_and_card_dimensions_json_observable = Subject()

        self.main_window.start_detection_observable.subscribe(lambda _: self.start_detection())
        self.main_window.stop_detection_observable.subscribe(lambda _: self.stop_detection())
        self.main_window.select_roi_observable.subscribe(lambda _: self.box_selector.open_roi_selector())
        self.main_window.show_rois_observable.subscribe(lambda _: show_rois(self.rois))
        self.main_window.select_card_dimensions_observable.subscribe(lambda _: self.box_selector.open_card_box_selector())
        self.main_window.read_rois_and_card_dimensions_json_observable.subscribe(lambda _: self.read_rois_and_card_sizes())
        self.main_window.write_rois_and_card_dimensions_json_observable.subscribe(lambda _: self.write_rois_and_card_sizes())

        self.box_selector.rois_selected_observable.subscribe(lambda rois: self.rois_selected(rois))
        self.box_selector.card_box_selected_observable.subscribe(lambda sizes: self.card_sizes_selected(sizes))

    def start_main_window(self):
        self.main_window.window.mainloop()
        return

    def start_detection(self):
        self.overlay.show_overlay()
        self.main_window.detection_started = True
        self.main_window.update_state()
        # self.main_window.window.iconify()
        self.start_detection_observable.on_next(None)
        return
    def stop_detection(self):
        self.overlay.hide_overlay()
        self.main_window.detection_started = False
        self.main_window.update_state()
        self.stop_detection_observable.on_next(None)
        return

    def rois_selected(self, rois: RoisContainer):
        self.main_window.roi_selected = True
        self.main_window.update_state()
        self.rois = rois
        self.rois_selected_observable.on_next(rois)
        return

    def card_sizes_selected(self, sizes : CardSizesContainer):
        self.main_window.card_sizes_selected = True
        self.main_window.update_state()
        self.card_sizes_selected_observable.on_next(sizes)

        return

    def read_rois_and_card_sizes(self):
        filename = filedialog.askopenfilename(defaultextension=".json")
        if filename is None or filename == "":
            return
        self.read_rois_and_card_dimensions_json_observable.on_next(filename)
        return

    def write_rois_and_card_sizes(self):
        filename = filedialog.asksaveasfilename(defaultextension=".json")
        if filename is None or filename == "":
            return
        self.write_rois_and_card_dimensions_json_observable.on_next(filename)
        return