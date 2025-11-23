import pytest
from CardCounting.BoundingBoxLogic import *
from Models.BoundingBox import BoundingBox


@pytest.mark.parametrize(
    "b1,b2,expected",
    [
        (BoundingBox(0, 0, 10, 10), BoundingBox(20, 20, 5, 5), 0.0),
        (BoundingBox(0, 0, 10, 10), BoundingBox(5, 5, 10, 10), 25 / 100),
        (BoundingBox(2, 2, 4, 4), BoundingBox(0, 0, 10, 10), 1.0),
        (BoundingBox(0, 0, 10, 10), BoundingBox(2, 2, 4, 4), 1.0),
    ],
)
def test_boxes_overlap_param(b1, b2, expected):
    assert boxes_overlap(b1, b2) == pytest.approx(expected)


@pytest.mark.parametrize(
    "b1,b2,expected",
    [
        (BoundingBox(0, 0, 100, 100), BoundingBox(1, 0, 100, 100), True),
        (BoundingBox(0, 0, 100, 100), BoundingBox(6, 0, 100, 100), False),
    ],
)
def test_boxes_match_param(b1, b2, expected):
    assert boxes_match(b1, b2) is expected
