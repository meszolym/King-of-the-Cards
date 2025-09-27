from Models.RoisContainer import RoisContainer
import cv2 as cv
from screeninfo import get_monitors

def show_rois(rois : RoisContainer):
    img = rois.base_image.copy()
    window_name = "Selected ROIs"
    cv.namedWindow(window_name, cv.WINDOW_NORMAL)

    cv.resizeWindow(window_name, int(get_monitors()[0].width*0.9), int(get_monitors()[0].height*0.9))

    for label, bb in (
            ("Dealer", rois.dealer_roi),
            ("Player", rois.player_roi),
            ("Message", rois.message_roi),
    ):
        x1, y1 = int(bb.x), int(bb.y)
        x2, y2 = int(bb.x + bb.w), int(bb.y + bb.h)
        cv.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 2)
        cv.putText(
            img,
            label,
            (x1, max(y1 - 10, 0)),
            cv.FONT_HERSHEY_SIMPLEX,
            0.6,
            (0, 255, 0),
            2,
            cv.LINE_AA
        )
    cv.imshow(window_name, img)
    cv.waitKey(0)
    cv.destroyWindow(window_name)
    return
