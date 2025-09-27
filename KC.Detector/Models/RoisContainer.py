import numpy as np
from Models.BoundingBox import BoundingBox
class RoisContainer:
    dealer_roi: BoundingBox
    player_roi: BoundingBox
    message_roi: BoundingBox
    base_image: np.ndarray