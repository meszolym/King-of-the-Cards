#This code is not in use, only added to git for reference (this is what the c# code is based on)

import cv2 as cv
import numpy as np

def getTopCards(img: cv.typing.MatLike) -> list:
    height,width = img.shape[:2]

    img = img[0:height//5*4,0:width]

    gray = cv.cvtColor(img, cv.COLOR_BGR2GRAY)

    canny = cv.Canny(gray,200,200)

    dilate = cv.dilate(canny,cv.getStructuringElement(cv.MORPH_ELLIPSE, (5,5)), iterations = 2)

    contours, hierarchy = cv.findContours(dilate, cv.RETR_LIST, cv.CHAIN_APPROX_SIMPLE)

    list = []

    for contour in contours:
        if cv.contourArea(contour) > 20000 and cv.contourArea(contour) < 50000 :
            epsilon = 0.1*cv.arcLength(contour,True)
            approx = cv.approxPolyDP(contour,epsilon,True)
            x, y, w, h = cv.boundingRect(contour)
            list.append((x,y,w,h))
    
    return list

x = getTopCards(cv.imread("tablefull.png"))

print(x)
# rectangle kirajzolása x,y,h,w alapján:
# cv.rectangle(img, (x,y), (x+w,y+h), (255,0,0), 2)


