using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using LanguageExt;

namespace KC.Logic.Services
{
    public static class ShoeService
    {
        /// <summary>
        /// Generates a list of cards given the number of decks.
        /// </summary>
        /// <param name="numberOfDecks"></param>
        /// <returns></returns>
        private static IEnumerable<Card> CardsForCompleteShoe(int numberOfDecks)
            => Enumerable.Range(0, numberOfDecks)
                .SelectMany(i => Enum.GetValues<CardSuit>()
                    .SelectMany(s => Enum.GetValues<CardFace>()
                        .Select(f => new Card(s, f))));

        private static int NumOfCards(int numberOfDecks) => numberOfDecks * Enum.GetValues<CardSuit>().Length * Enum.GetValues<CardFace>().Length;

        /// <summary>
        /// Creates a shuffled shoe with the given set of properties.
        /// </summary>
        public static ShuffledShoe CreateShuffledShoe(Guid tableId, ShoeProperties properties)
            => new ShuffledShoe(
                TableId: tableId,
                ShuffleCardIndex: RandomProvider.Random.Next(
                    (int)(NumOfCards(properties.NumberOfDecks) * properties.MaxShoePenetration)
                    - properties.ShuffleCardRadius,

                    (int)(NumOfCards(properties.NumberOfDecks) * properties.MaxShoePenetration)
                    + properties.ShuffleCardRadius
                ),
                Cards: CardsForCompleteShoe(properties.NumberOfDecks).ToImmutableList().Shuffle()
            );

        private static ImmutableQueue<Card> Shuffle(this ImmutableList<Card> cards)
        {
            Card[] shuffled = new Card[cards.Count];

            for (int i = 0; i < cards.Count; i++)
            {
                int j = RandomProvider.Random.Next(i, cards.Count);
                shuffled[i] = cards[j];
                shuffled[j] = cards[i];
            }

            return ImmutableQueue.Create(shuffled);

        }

    }
}
