import os

import cv2 as cv
import numpy as np

input_path="../../KC.Frontend.Client/Assets/Cards"

fns = os.listdir(input_path)
for filename in fns:
    if filename.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', '.tiff')):
        img_path = os.path.join(input_path, filename)
        img = cv.imread(img_path)
        if img is None:
            continue
        h, w = img.shape[:2]
        crop = img[:h//3, :w//3]
        out_path = os.path.join("./CardsCut", filename)
        cv.imwrite(out_path, crop)