from Overlay import Overlay
from MainWindow import MainWindow
from RoiSelector import RoiSelector
from rx.subject import Subject
class GuiConductor:
    main_window : MainWindow
    overlay : Overlay
    roi_selector : RoiSelector

    rois_selected_observable : Subject

    def __init__(self):
        self.main_window = MainWindow(False)
        self.overlay = Overlay()
        self.roi_selector = RoiSelector()

        self.main_window.start_detection_observable.subscribe(lambda _: self.overlay.show_overlay())
        self.main_window.stop_detection_observable.subscribe(lambda _: self.overlay.hide_overlay())
        self.main_window.select_roi_observable.subscribe(lambda _: self.roi_selector.open_roi_selector())

        self.rois_selected_observable = Subject()
        self.roi_selector.rois_selected_observable.subscribe(lambda rois: self.rois_selected_observable.on_next(rois))

    def start_main_window(self):
        self.main_window.window.mainloop()
        return