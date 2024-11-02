using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Models;
using LanguageExt;

namespace KC.Logic.Services
{
    public class ShoeService
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


        /// <summary>
        /// Creates a shuffled shoe with the given set of properties.
        /// </summary>
        public static ShuffledShoe CreateShuffledShoe(ShoeProperties properties)
        {
            List<Card> cards = CardsForCompleteShoe(properties.NumberOfDecks).ToList();

            for (int i = 0; i < cards.Count; i++)
            {
                int j = RandomProvider.Random.Next(i, cards.Count);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }

            int penetration = (int)(cards.Count * properties.MaxShoePenetration);
            int shuffleCardIndex =
                RandomProvider.Random.Next(
                    penetration - properties.ShuffleCardRadius,
                    penetration + properties.ShuffleCardRadius);

            return new ShuffledShoe(shuffleCardIndex, new Queue<Card>(cards));
        }

    }
}
