from rx.subject import Subject
from Models.RoisContainer import RoisContainer
class RoiSelector:
    rois_selected_observable : Subject
    #TODO: Implement ROI selection logic
    def __init__(self):
        self.rois_selected_observable = Subject()
        pass

    def open_roi_selector(self):
        rois = RoisContainer()
        # Placeholder for ROI selection logic
        self.rois_selected_observable.on_next(rois) # Emit empty RoisContainer for
        pass