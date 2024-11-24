using System.Collections.Immutable;
using System.Numerics;
using KC.App.Logic.SessionLogic.BettingBoxLogic;
using KC.App.Logic.SessionLogic.HandLogic;
using KC.App.Logic.SessionLogic.ShoeLogic;
using KC.App.Models.Classes;
using KC.App.Models.Classes.Hand;
using KC.App.Models.Enums;
using KC.App.Models.Records;
using KC.App.Models.Structs;

namespace KC.App.Logic.SessionLogic.TableLogic
{
    public static class TableExtensions
    {
        public static IEnumerable<BettingBox> BoxesInPlay(this Table table) =>
            table.Boxes.Where(box => box.Hands[0].Bet > 0).OrderBy(b => b.Idx);

        public static void Reset(this Table table)
        {
            table.Boxes.ForEach(b => b.ClearHands());
        }

        public static void StartDealing(this Table table)
        {

            //if shoe needs shuffling, shuffle
            if (table.DealingShoe.ShuffleCardIdx <= table.DealingShoe.NextCardIdx)
            {
                table.DealingShoe.Shuffle(Random.Shared);
            }

            //deal cards
            for (int i = 0; i < 2; i++)
            {
                foreach (BettingBox box in table.BoxesInPlay())
                {
                    box.Hands[0].Cards.Add(table.DealingShoe.TakeCard());
                }
                table.DealerHand.Cards.Add(table.DealingShoe.TakeCard());
            }
        }

        public static void MakeMove(this Table table, int boxIdx, int handIdx, Player player, Move move)
        {
            var box = table.Boxes[boxIdx];
            var hand = box.Hands[handIdx];
            if (!box.CheckOwner(player))
                throw new InvalidOperationException("Player does not own this box.");
            if (!hand.GetPossibleActions().Contains(move))
                throw new InvalidOperationException("Action not possible.");

            switch (move)
            {
                case Move.Stand:
                    hand.Finished = true;
                    break;
                case Move.Hit:
                    hand.Cards.Add(table.DealingShoe.TakeCard());
                    break;
                case Move.Double:
                    hand.Cards.Add(table.DealingShoe.TakeCard());
                    hand.Bet *= 2;
                    break;
                case Move.Split:
                    box.Hands.Insert(handIdx + 1,
                        new PlayerHand([box.Hands[handIdx].Cards[1]],
                            box.Hands[handIdx].Bet,
                            false));
                    hand.Cards.RemoveAt(1);
                    hand.Splittable = false;
                    hand.Cards.Add(table.DealingShoe.TakeCard());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, null);
            }

            //if bust, mark hand as finished
            if (hand.GetValue().Value > 21) hand.Finished = true;
        }

        public static Hand DealerPlayHand(this Table table)
        {
            var dealerHand = table.DealerHand;
            while (dealerHand.GetValue().Value < 17)
            {
                dealerHand.Cards.Add(table.DealingShoe.TakeCard());
            }
            dealerHand.Finished = true;
            return dealerHand;
        }

        public static void PayOutBets(this Table table)
        {
            var dealerHand = table.DealerHand;

            foreach (var box in table.BoxesInPlay())
            {
                foreach (var hand in box.Hands)
                {
                    if (hand.GetValue().Value > 21) //player bust
                    {
                        hand.Bet = 0; //lose bet
                        continue;
                    }

                    if (dealerHand.GetValue().Value >= 21) //dealer bust
                    {
                        if (hand.GetValue().IsBlackJack) hand.Bet += hand.Bet * 1.5; //if player has blackjack, pay out 1.5x bet
                        else hand.Bet += hand.Bet; //pay out bet
                        continue;
                    }

                    if (dealerHand.GetValue().IsBlackJack) //dealer has blackjack
                    {
                        if (!hand.GetValue().IsBlackJack) hand.Bet = 0; //if player doesn't have blackjack, lose bet, else bet stays the same
                        continue;
                    }

                    if (hand.GetValue().IsBlackJack) //player has blackjack
                    {
                        hand.Bet += hand.Bet * 1.5; //if player has blackjack, pay out 1.5x bet
                    }

                    if (hand.GetValue().Value > dealerHand.GetValue().Value) //player has stronger hand
                    {
                        hand.Bet += hand.Bet; //pay out bet
                        continue;
                    }

                    if (hand.GetValue().Value < dealerHand.GetValue().Value) //player has weaker hand
                    {
                        hand.Bet = 0; //lose bet
                    }

                    //if same value, bet stays the same
                }
            }
        }
    }
}
