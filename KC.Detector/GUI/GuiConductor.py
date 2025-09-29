from typing import Optional

from GUI.Overlay import Overlay
from GUI.MainWindow import MainWindow
from GUI.RoiSelector import RoiSelector
from rx.subject import Subject

from Models.BoundingBox import BoundingBox
from Models.RoisContainer import RoisContainer
from GUI.RoiShower import show_rois
from ImageProcessing.BoxPicker import pick as pick_box
from ImageProcessing.ScreenCapturer import take_screenshot

class GuiConductor:
    main_window : MainWindow
    overlay : Overlay
    roi_selector : RoiSelector

    rois_selected_observable : Subject
    start_detection_observable : Subject
    stop_detection_observable : Subject
    rois : Optional[RoisContainer]
    card_box : Optional[BoundingBox]

    def __init__(self):
        self.main_window = MainWindow(False)
        self.overlay = Overlay()
        self.roi_selector = RoiSelector()

        self.rois_selected_observable = Subject()
        self.start_detection_observable = Subject()
        self.stop_detection_observable = Subject()

        self.main_window.start_detection_observable.subscribe(lambda _: self.start_detection())
        self.main_window.stop_detection_observable.subscribe(lambda _: self.stop_detection())
        self.main_window.select_roi_observable.subscribe(lambda _: self.roi_selector.open_roi_selector())
        self.main_window.show_rois_observable.subscribe(lambda _: self.show_rois())
        self.main_window.select_card_dimensions_observable.subscribe(lambda _: self.pick_card_box())

        self.roi_selector.rois_selected_observable.subscribe(lambda rois: self.rois_selected(rois))

    def start_main_window(self):
        self.main_window.window.mainloop()
        return

    def start_detection(self):
        self.overlay.show_overlay()
        self.main_window.detection_started = True
        self.main_window.update_state()
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

    def show_rois(self):
        show_rois(self.rois)
        return

    def pick_card_box(self):
        varim = take_screenshot()
        self.card_box = pick_box("Select card dimensions", varim.copy())
        from ImageProcessing.CardProcessor import process_cards
        import numpy as np
        print("Meret:" + str(float(self.card_box.h)*float(self.card_box.w)))
        bboxl = process_cards(varim.copy(), float(self.card_box.h)*float(self.card_box.w))

        def show_bboxes(bboxes: list[BoundingBox], img: np.ndarray) -> None:
            import cv2 as cv
            window_name = "GECIm"
            cv.namedWindow(window_name, cv.WINDOW_NORMAL)
            from screeninfo import get_monitors
            cv.resizeWindow(window_name, int(get_monitors()[0].width * 0.9), int(get_monitors()[0].height * 0.9))

            i = 0
            for bb in bboxes:
                x1, y1 = int(bb.x), int(bb.y)
                x2, y2 = int(bb.x + bb.w), int(bb.y + bb.h)
                cv.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 2)
                cv.putText(
                    img,
                    str(i),
                    (x1, max(y1 - 10, 0)),
                    cv.FONT_HERSHEY_SIMPLEX,
                    0.6,
                    (0, 255, 0),
                    2,
                    cv.LINE_AA
                )
                i = i + 1
            cv.imshow(window_name, img)
            cv.waitKey(0)
            cv.destroyWindow(window_name)
            return
        show_bboxes(bboxl, varim.copy())