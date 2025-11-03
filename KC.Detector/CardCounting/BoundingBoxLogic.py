from Models.BoundingBox import BoundingBox
def box_area(box: BoundingBox) -> int:
    return box.w * box.h


def boxes_overlap(box1: BoundingBox, box2: BoundingBox) -> float:
    x_left = max(box1.x, box2.x)
    y_top = max(box1.y, box2.y)
    x_right = min(box1.x + box1.w, box2.x + box2.w)
    y_bottom = min(box1.y + box1.h, box2.y + box2.h)

    if x_right <= x_left or y_bottom <= y_top:
        return 0.0

    intersection_area = (x_right - x_left) * (y_bottom - y_top)
    smaller_area = min(box_area(box1), box_area(box2))

    return intersection_area / smaller_area

def boxes_match(box1: BoundingBox, box2: BoundingBox) -> bool:
    return boxes_overlap(box1, box2) >= 0.95
