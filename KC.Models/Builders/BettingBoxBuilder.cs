using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models.Builders
{
    public class BettingBoxBuilder
    {
        private readonly Guid _tableId;
        private readonly int _idx;
        private readonly Option<string> _ownerId;
        private readonly ImmutableList<Hand> _hands;
        public BettingBoxBuilder SetTableId(Guid tableId) => new(tableId, _idx, _ownerId, _hands);
        public BettingBoxBuilder SetIdx(int idx) => new(_tableId, idx, _ownerId, _hands);
        public BettingBoxBuilder SetOwner(Option<string> playerId) => new(_tableId, _idx, playerId, _hands);
        public BettingBoxBuilder SetHands(ImmutableList<Hand> hands) => new(_tableId, _idx, _ownerId, hands);
        public BettingBox Build() => new(_tableId, _idx, _ownerId, _hands);

        public BettingBoxBuilder() { }
        public BettingBoxBuilder(BettingBox bettingBox)
        {
            _tableId = bettingBox.TableId;
            _idx = bettingBox.BoxIdx;
            _ownerId = bettingBox.OwnerId;
            _hands = bettingBox.Hands;
        }

        private BettingBoxBuilder(Guid tableId, int idx, Option<string> ownerId, ImmutableList<Hand> hands)
        {
            _tableId = tableId;
            _idx = idx;
            _ownerId = ownerId;
            _hands = hands;
        }
    }

    public static class GetBuilderFromBettingBoxExtension
    {
        public static BettingBoxBuilder GetBuilder(this BettingBox bettingBox)
        {
            return new BettingBoxBuilder(bettingBox);
        }
    }
}
