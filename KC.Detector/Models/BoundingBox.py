class BoundingBox:
    x : float
    y : float
    w : float
    h : float

    def __init__(self, x: float, y: float, w: float, h: float) -> None:
        self.x = x
        self.y = y
        self.w = w
        self.h = h
        return