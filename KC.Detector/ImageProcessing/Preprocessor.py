#Do ROI, edge detection, etc here
from ..Models.BoundingBox import BoundingBox
from ..Models.RoisContainer import RoisContainer
from rx.subject import Subject
class Preprocessor:
    run : bool = False
    rois : RoisContainer
    message_image_observable : Subject
    dealer_image_observable : Subject
    player_image_observable : Subject

    def __init__(self, rois: RoisContainer):
        self.rois = rois

        self.message_image_observable = Subject()
        self.dealer_image_observable = Subject()
        self.player_image_observable = Subject()
        return

    def mainloop(self):
        #Every second, take an image, preprocess it, emit event via observable with processed image
        # 3 observables, one for each ROI
        self.run = True

        while self.run:
            #TODO: implement image capture and preprocessing
            print("Preprocessing image...") #placeholder
            #get image
            #preprocess image
            #emit event via observable with processed image

            #wait 1 second (asynchronous)
        return

    def stop(self):
        self.run = False
        return