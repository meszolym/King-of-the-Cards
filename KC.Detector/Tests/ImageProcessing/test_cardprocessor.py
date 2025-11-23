from itertools import count

import numpy as np
import cv2 as cv
import pytest

from ImageProcessing import Utils
from ImageProcessing.CardProcessor import CardProcessor
from ImageProcessing.RoisAndCardDimIO import read_rois_and_card_dimensions
from Models.Card import Card
from Models.Enums import Suit, Rank, CardType
from Models.Table import Table


@pytest.mark.parametrize(
    "filename, expected_suit, expected_rank",
    [
        ("cardHeartsA.png", Suit.Hearts, Rank.Ace),
        ("cardSpades10.png", Suit.Spades, Rank.Ten),
        ("cardClubsJ.png", Suit.Clubs, Rank.Jack),
        ("cardDiamondsK.png", Suit.Diamonds, Rank.King),
        ("someOtherFile.png", Suit.Unknown, Rank.Unknown)
    ]
)
def test_parse_filename(filename, expected_suit, expected_rank):
    suit, rank = CardProcessor.parse_filename(filename)
    assert suit == expected_suit
    assert rank == expected_rank
@pytest.mark.parametrize(
    "imgfilename, jsonfilename, dealer, players,",
    [

    ])
def test_card_detection(imgfilename, jsonfilename, dealer, players):
    processor = CardProcessor()
    rois_container, sizes_container = read_rois_and_card_dimensions(f'Tests/ImageProcessing/TestImages/{jsonfilename}')
    processor.card_sizes = sizes_container
    img = cv.imread(f'Tests/ImageProcessing/TestImages/{imgfilename}')
    dealer_roi = Utils.get_roi(img, rois_container.dealer_roi)
    player_roi = Utils.get_roi(img, rois_container.player_roi)

    dealer_cards = processor.process_cards(dealer_roi, CardType.Dealer)
    player_cards = processor.process_cards(player_roi, CardType.Player)

    def extract_suits_and_ranks(cards):
        return [(card.suit, card.rank) for card in cards]

    dealer_cards = extract_suits_and_ranks(dealer_cards)
    player_cards = extract_suits_and_ranks(player_cards)
    dealer = extract_suits_and_ranks(dealer)
    players = extract_suits_and_ranks(players)

    assert dealer_cards == dealer
    assert player_cards == players



