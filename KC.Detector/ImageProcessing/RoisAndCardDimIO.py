import json

from Models.BoundingBox import BoundingBox
from Models.CardSizesContainer import CardSizesContainer
from Models.RoisContainer import RoisContainer
import cv2 as cv

def read_rois_and_card_dimensions(filepath: str):
    with open(filepath, "r") as f:
        data = json.load(f)
    rois = data.get("Rois", {})
    sizes = data.get("Sizes", {})

    dealer_roi_raw = rois.get("DealerRoi", [0, 0, 0, 0])
    dealer_roi = BoundingBox(dealer_roi_raw[0], dealer_roi_raw[1], dealer_roi_raw[2], dealer_roi_raw[3])
    player_roi_raw = rois.get("PlayerRoi", [0, 0, 0, 0])
    player_roi = BoundingBox(player_roi_raw[0], player_roi_raw[1], player_roi_raw[2], player_roi_raw[3])
    message_roi_raw = rois.get("MessageRoi", [0, 0, 0, 0])
    message_roi = BoundingBox(message_roi_raw[0], message_roi_raw[1], message_roi_raw[2], message_roi_raw[3])
    base_image_path = rois.get("BaseImagePath", "")

    dealer_card_size_raw = sizes.get("DealerCardSize", [0, 0])
    dealer_card_size = BoundingBox(0, 0, dealer_card_size_raw[0], dealer_card_size_raw[1])
    player_card_size_raw = sizes.get("PlayerCardSize", [0, 0])
    player_card_size = BoundingBox(0, 0, player_card_size_raw[0], player_card_size_raw[1])

    img = cv.imread(base_image_path) if base_image_path != "" else None

    rois_container = RoisContainer(dealer_roi, player_roi, message_roi, img)
    sizes_container = CardSizesContainer(dealer_card_size, player_card_size)

    return rois_container, sizes_container

def write_rois_and_card_dimensions(filepath: str, rois: RoisContainer, sizes: CardSizesContainer):
    image_path = filepath[:-5] + ".jpg"

    data = {
        "Rois": {
            "DealerRoi": [rois.dealer_roi.x, rois.dealer_roi.y, rois.dealer_roi.w, rois.dealer_roi.h],
            "PlayerRoi": [rois.player_roi.x, rois.player_roi.y, rois.player_roi.w, rois.player_roi.h],
            "MessageRoi": [rois.message_roi.x, rois.message_roi.y, rois.message_roi.w, rois.message_roi.h],
            "BaseImagePath": image_path
        },
        "Sizes": {
            "DealerCardSize": [sizes.dealer_card_box.w, sizes.dealer_card_box.h],
            "PlayerCardSize": [sizes.player_card_box.w, sizes.player_card_box.h]
        },
    }
    with open(filepath, "w") as f:
        json.dump(data, f, indent=4)

    cv.imwrite(image_path, rois.base_image)
    return