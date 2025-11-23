from collections import Counter
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
        ("MacOS/1.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None,None)],
         [Card(Rank.Two, Suit.Spades, None,None)]),
        ("MacOS/2.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None,None)],
         [Card(Rank.Two, Suit.Diamonds, None,None)]),
        ("MacOS/3.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None,None)],
         [Card(Rank.King, Suit.Hearts, None,None)]),
        ("MacOS/4.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None,None)],
         [Card(Rank.Three, Suit.Diamonds, None,None)]),
        ("MacOS/5.png", "MacOS/MacOS.json",
         [
             Card(Rank.Queen, Suit.Hearts, None,None),
             Card(Rank.Eight, Suit.Hearts, None,None)
         ],
         [Card(Rank.Three, Suit.Diamonds, None,None)]),
        ("MacOS/6.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Spades, None,None)],
         [
             Card(Rank.Seven, Suit.Diamonds, None,None),
             Card(Rank.Four, Suit.Clubs, None,None),
             Card(Rank.Seven, Suit.Spades, None,None)]),
        ("MacOS/7.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Spades, None,None)],
         [
             Card(Rank.Five, Suit.Hearts, None,None),
             Card(Rank.Jack, Suit.Clubs, None,None),
             Card(Rank.Queen, Suit.Spades, None,None)
         ]),
        ("MacOS/8.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None,None)],
         [
             Card(Rank.Six, Suit.Clubs, None,None),
             Card(Rank.Eight, Suit.Diamonds, None,None),
             Card(Rank.Seven, Suit.Clubs, None,None),
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Eight, Suit.Clubs, None,None)
         ]),
        ("MacOS/9.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None,None)],
         [
             Card(Rank.Four, Suit.Diamonds, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Three, Suit.Diamonds, None,None),
             Card(Rank.Five, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/10.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None,None)],
         [
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Three, Suit.Diamonds, None,None),
             Card(Rank.Five, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/11.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None,None)],
         [
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Seven, Suit.Diamonds, None,None),
             Card(Rank.Five, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/12.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None,None)],
         [
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Seven, Suit.Diamonds, None,None),
             Card(Rank.Ten, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/13.png", "MacOS/MacOS.json",
         [
             Card(Rank.Eight, Suit.Hearts, None,None),
             Card(Rank.Four, Suit.Clubs, None,None)
         ],
         [
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Seven, Suit.Diamonds, None,None),
             Card(Rank.Ten, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/14.png", "MacOS/MacOS.json",
         [
             Card(Rank.Eight, Suit.Hearts, None,None),
             Card(Rank.Four, Suit.Clubs, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None),
         ],
         [
             Card(Rank.Nine, Suit.Spades, None,None),
             Card(Rank.Queen, Suit.Spades, None,None),
             Card(Rank.Seven, Suit.Diamonds, None,None),
             Card(Rank.Ten, Suit.Hearts, None,None),
             Card(Rank.Queen, Suit.Hearts, None,None)
         ]),
        ("MacOS/15.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None,None)
         ],
         [
            Card(Rank.King, Suit.Diamonds, None,None),
            Card(Rank.Queen, Suit.Hearts, None,None),
            Card(Rank.Seven, Suit.Diamonds, None,None)
         ]),
        ("MacOS/16.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None,None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None,None),
            Card(Rank.Two, Suit.Clubs, None,None),
            Card(Rank.Queen, Suit.Spades, None,None)
         ]),
        ("MacOS/17.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None,None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None,None),
            Card(Rank.Jack, Suit.Clubs, None,None),
            Card(Rank.Queen, Suit.Spades, None,None)
         ]),
        ("MacOS/18.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None,None),
             Card(Rank.Three, Suit.Hearts, None,None),
         ],
         [
            Card(Rank.Eight, Suit.Spades, None,None),
            Card(Rank.Jack, Suit.Clubs, None,None),
            Card(Rank.Queen, Suit.Spades, None,None)
         ]),
        ("MacOS/19.png", "MacOS/MacOS.json",
         [
             Card(Rank.Jack, Suit.Spades, None,None),
             Card(Rank.Three, Suit.Hearts, None,None),
             Card(Rank.King, Suit.Clubs, None,None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None,None),
            Card(Rank.Jack, Suit.Clubs, None,None),
            Card(Rank.Queen, Suit.Spades, None,None)
         ])
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

    def extract_distinguishers(cards):
        return [card.rank for card in cards]

    dealer_cards = extract_distinguishers(dealer_cards)
    player_cards = extract_distinguishers(player_cards)
    dealer = extract_distinguishers(dealer)
    players = extract_distinguishers(players)

    assert Counter(dealer_cards) == Counter(dealer)
    assert Counter(player_cards) == Counter(players)



