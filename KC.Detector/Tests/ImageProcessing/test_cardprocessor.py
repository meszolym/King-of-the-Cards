from collections import Counter
import cv2 as cv
import pytest

from CardCounting.BoundingBoxLogic import boxes_match, box_area
from ImageProcessing import Utils
from ImageProcessing.CardProcessor import CardProcessor
from ImageProcessing.RoisAndCardDimIO import read_rois_and_card_dimensions
from Models.Card import Card
from Models.Enums import Suit, Rank, CardType


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
    "imgfilename, jsonfilename, dealer, players",
    [
        ("MacOS/1.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None, None)],
         [Card(Rank.Two, Suit.Spades, None, None)]),
        ("MacOS/2.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None, None)],
         [Card(Rank.Two, Suit.Diamonds, None, None)]),
        ("MacOS/3.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None, None)],
         [Card(Rank.King, Suit.Hearts, None, None)]),
        ("MacOS/4.png", "MacOS/MacOS.json",
         [Card(Rank.Queen, Suit.Hearts, None, None)],
         [Card(Rank.Three, Suit.Diamonds, None, None)]),
        ("MacOS/5.png", "MacOS/MacOS.json",
         [
             Card(Rank.Queen, Suit.Hearts, None, None),
             Card(Rank.Eight, Suit.Hearts, None, None)
         ],
         [Card(Rank.Three, Suit.Diamonds, None, None)]),
        ("MacOS/6.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Spades, None, None)],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Four, Suit.Clubs, None, None),
             Card(Rank.Seven, Suit.Spades, None, None)]),
        ("MacOS/7.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Spades, None, None)],
         [
             Card(Rank.Five, Suit.Hearts, None, None),
             Card(Rank.Jack, Suit.Clubs, None, None),
             Card(Rank.Queen, Suit.Spades, None, None)
         ]),
        ("MacOS/8.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None, None)],
         [
             Card(Rank.Six, Suit.Clubs, None, None),
             Card(Rank.Eight, Suit.Diamonds, None, None),
             Card(Rank.Seven, Suit.Clubs, None, None),
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Eight, Suit.Clubs, None, None)
         ]),
        ("MacOS/9.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None, None)],
         [
             Card(Rank.Four, Suit.Diamonds, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Five, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/10.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None, None)],
         [
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Five, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/11.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None, None)],
         [
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Five, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/12.png", "MacOS/MacOS.json",
         [Card(Rank.Eight, Suit.Hearts, None, None)],
         [
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/13.png", "MacOS/MacOS.json",
         [
             Card(Rank.Eight, Suit.Hearts, None, None),
             Card(Rank.Four, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/14.png", "MacOS/MacOS.json",
         [
             Card(Rank.Eight, Suit.Hearts, None, None),
             Card(Rank.Four, Suit.Clubs, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None),
         ],
         [
             Card(Rank.Nine, Suit.Spades, None, None),
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Hearts, None, None),
             Card(Rank.Queen, Suit.Hearts, None, None)
         ]),
        ("MacOS/15.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None, None)
         ],
         [
            Card(Rank.King, Suit.Diamonds, None, None),
            Card(Rank.Queen, Suit.Hearts, None, None),
            Card(Rank.Seven, Suit.Diamonds, None, None)
         ]),
        ("MacOS/16.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None, None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None, None),
            Card(Rank.Two, Suit.Clubs, None, None),
            Card(Rank.Queen, Suit.Spades, None, None)
         ]),
        ("MacOS/17.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None, None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None, None),
            Card(Rank.Jack, Suit.Clubs, None, None),
            Card(Rank.Queen, Suit.Spades, None, None)
         ]),
        ("MacOS/18.png", "MacOS/MacOS.json",
         [
            Card(Rank.Jack, Suit.Spades, None, None),
             Card(Rank.Three, Suit.Hearts, None, None),
         ],
         [
            Card(Rank.Eight, Suit.Spades, None, None),
            Card(Rank.Jack, Suit.Clubs, None, None),
            Card(Rank.Queen, Suit.Spades, None, None)
         ]),
        ("MacOS/19.png", "MacOS/MacOS.json",
         [
             Card(Rank.Jack, Suit.Spades, None, None),
             Card(Rank.Three, Suit.Hearts, None, None),
             Card(Rank.King, Suit.Clubs, None, None)
         ],
         [
            Card(Rank.Eight, Suit.Spades, None, None),
            Card(Rank.Jack, Suit.Clubs, None, None),
            Card(Rank.Queen, Suit.Spades, None, None)
         ]),
        ("Windows/1.png", "Windows/Windows.json",
         [
             Card(Rank.Queen, Suit.Spades, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Jack, Suit.Diamonds, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None)
         ]),
        ("Windows/2.png", "Windows/Windows.json",
         [
             Card(Rank.Queen, Suit.Spades, None, None),
         ],
         [
             Card(Rank.Jack, Suit.Diamonds, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None)
         ]),
        ("Windows/3.png", "Windows/Windows.json",
         [
             Card(Rank.Three, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Clubs, None, None)
         ]),
        ("Windows/4.png", "Windows/Windows.json",
         [
             Card(Rank.Three, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.King, Suit.Spades, None, None)
         ]),
        ("Windows/5.png", "Windows/Windows.json",
         [
             Card(Rank.Jack, Suit.Hearts, None, None),
             Card(Rank.Three, Suit.Clubs, None, None),
         ],
         [
             Card(Rank.King, Suit.Spades, None, None)
         ]),
        ("Windows/6.png", "Windows/Windows.json",
         [
             Card(Rank.Jack, Suit.Hearts, None, None),
             Card(Rank.Three, Suit.Clubs, None, None),
             Card(Rank.Four, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.King, Suit.Spades, None, None)
         ]),
        ("Windows/7.png", "Windows/Windows.json",
         [
             Card(Rank.Ace, Suit.Hearts, None, None),
         ],
         [
             Card(Rank.Two, Suit.Spades, None, None),
             Card(Rank.King, Suit.Spades, None, None),
             Card(Rank.Jack, Suit.Diamonds, None, None)
         ]),
        ("Windows/8.png", "Windows/Windows.json",
         [
             Card(Rank.Ace, Suit.Hearts, None, None),
         ],
         [
             Card(Rank.Ten, Suit.Clubs, None, None),
             Card(Rank.Eight, Suit.Clubs, None, None),
             Card(Rank.Four, Suit.Hearts, None, None)
         ]),
        ("Windows/9.png", "Windows/Windows.json",
         [
             Card(Rank.Seven, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Jack, Suit.Diamonds, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Four, Suit.Hearts, None, None),
             Card(Rank.Six, Suit.Spades, None, None),
             Card(Rank.Three, Suit.Spades, None, None)
         ]),
        ("Windows/10.png", "Windows/Windows.json",
         [
             Card(Rank.Seven, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Ace, Suit.Clubs, None, None),
             Card(Rank.Five, Suit.Spades, None, None),
             Card(Rank.Eight, Suit.Hearts, None, None),
             Card(Rank.Six, Suit.Spades, None, None),
             Card(Rank.Ten, Suit.Diamonds, None, None)
         ]),
        ("Windows/11.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Jack, Suit.Spades, None, None),
             Card(Rank.Jack, Suit.Hearts, None, None),
             Card(Rank.Four, Suit.Diamonds, None, None)
         ]),
        ("Windows/12.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Jack, Suit.Clubs, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None)
         ]),
        ("Windows/13.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Jack, Suit.Clubs, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None)
         ]),
        ("Windows/14.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Three, Suit.Hearts, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None)
         ]),
        ("Windows/15.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None)
         ]),
        ("Windows/16.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None)
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Clubs, None, None)
         ]),
        ("Windows/17.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Clubs, None, None)
         ]),
        ("Windows/18.png", "Windows/Windows.json",
         [
             Card(Rank.Six, Suit.Clubs, None, None),
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Jack, Suit.Diamonds, None, None),
         ],
         [
             Card(Rank.Seven, Suit.Diamonds, None, None),
             Card(Rank.Nine, Suit.Clubs, None, None),
             Card(Rank.Three, Suit.Diamonds, None, None),
             Card(Rank.Ten, Suit.Clubs, None, None)
         ]),
        ("Windows/19.png", "Windows/Windows.json",
         [
             Card(Rank.Queen, Suit.Spades, None, None),
         ],
         [
             Card(Rank.Eight, Suit.Clubs, None, None),
             Card(Rank.Seven, Suit.Spades, None, None),
             Card(Rank.Ten, Suit.Hearts, None, None)
         ]),
        ("Windows/20.png", "Windows/Windows.json",
         [
             Card(Rank.Queen, Suit.Spades, None, None),
         ],
         [
             Card(Rank.Jack, Suit.Diamonds, None, None),
             Card(Rank.Ace, Suit.Diamonds, None, None),
             Card(Rank.Six, Suit.Hearts, None, None)
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

    def deduplicate_cards(cards): # Normally done in Organizer
        to_remove = set()

        for i, c1 in enumerate(cards):
            if i in to_remove:
                continue

            for j, c2 in enumerate(cards):
                if i >= j or j in to_remove:
                    continue

                if boxes_match(c1.box, c2.box):
                    # Keep the one with the smaller box area (tighter fit)
                    if box_area(c1.box) <= box_area(c2.box):
                        to_remove.add(j)
                    else:
                        to_remove.add(i)
                        break  # c1 is being removed, skip to next c1

        return [card for i, card in enumerate(cards) if i not in to_remove]

    dealer_cards = deduplicate_cards(dealer_cards)
    player_cards = deduplicate_cards(player_cards)

    def extract_distinguishers(cards):
        return [card.rank for card in cards if card.rank != Rank.Unknown] # we only care about rank here, and ignore unknowns

    dealer_cards = extract_distinguishers(dealer_cards)
    player_cards = extract_distinguishers(player_cards)
    dealer = extract_distinguishers(dealer)
    players = extract_distinguishers(players)

    assert Counter(dealer_cards) == Counter(dealer)
    assert Counter(player_cards) == Counter(players)



