from GUI.Overlay import Overlay
from GUI.MainWindow import MainWindow
from GUI.RoiSelector import RoiSelector
from rx.subject import Subject
class GuiConductor:
    main_window : MainWindow
    overlay : Overlay
    roi_selector : RoiSelector

    rois_selected_observable : Subject
    start_detection_observable : Subject
    stop_detection_observable : Subject

    def __init__(self):
        self.main_window = MainWindow(False)
        self.overlay = Overlay()
        self.roi_selector = RoiSelector()

        self.rois_selected_observable = Subject()
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()

        self.main_window.start_detection_observable.subscribe(lambda _: self.start_detection())
        self.main_window.stop_detection_observable.subscribe(lambda _: self.stop_detection())
        self.main_window.select_roi_observable.subscribe(lambda _: self.select_roi())

        self.roi_selector.rois_selected_observable.subscribe(lambda rois: self.rois_selected_observable.on_next(rois))

    def start_main_window(self):
        self.main_window.window.mainloop()
        return

    def start_detection(self):
        self.overlay.show_overlay()
        #TODO: Manipulate main widow here?
        self.start_detection_observable.on_next(None)
        return
    def stop_detection(self):
        self.overlay.hide_overlay()
        #TODO: Manipulate main widow here?
        self.stop_detection_observable.on_next(None)
        return

    def select_roi(self):
        self.roi_selector.open_roi_selector()
        #TODO: Manipulate main widow here?
        return