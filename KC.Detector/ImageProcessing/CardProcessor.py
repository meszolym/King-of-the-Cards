#Do the processing of cards (rank, suit, etc.) here
import numpy as np
from Models.Card import Card
from Models.BoundingBox import BoundingBox
import cv2 as cv

CONST_KERNEL_SIZE = (3,3)
CONST_ITERATIONS = 2
CONST_CANNY_THRESHOLD1 = 200
CONST_CANNY_THRESHOLD2 = 200
CONST_CONTOUR_AREA_PCT_MIN = 0.9
CONST_CONTOUR_AREA_PCT_MAX = 1.1

def process_cards(img : np.ndarray, approx_size: float): #-> list[Card]:
    cards : list[Card] = []

    gray = cv.cvtColor(img.copy(), cv.COLOR_BGR2GRAY)
    canny = cv.Canny(gray, CONST_CANNY_THRESHOLD1,CONST_CANNY_THRESHOLD2)
    dilated = cv.dilate(canny,cv.getStructuringElement(cv.MORPH_ELLIPSE, CONST_KERNEL_SIZE), iterations = CONST_ITERATIONS)
    contours, hierarchy = cv.findContours(dilated, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)

    card_boxes : list[BoundingBox] = []

    for contour in contours:
        if approx_size*CONST_CONTOUR_AREA_PCT_MIN < cv.contourArea(contour) < approx_size*CONST_CONTOUR_AREA_PCT_MIN:
            epsilon = 0.1*cv.arcLength(contour,True)
            approx = cv.approxPolyDP(contour,epsilon,True)
            x, y, w, h = cv.boundingRect(contour)
            card_boxes.append(BoundingBox(x,y,w,h))

    print(card_boxes)
    print(len(card_boxes))
    return card_boxes
    #TODO: Implement card processing here
    return cards