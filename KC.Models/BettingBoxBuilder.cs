using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace KC.Models
{
    public class BettingBoxBuilder : IDisposable
    {
        private Guid _tableId;
        private int _idx;
        private Option<string> _ownerId;
        private ImmutableList<Hand> _hands;
        public BettingBoxBuilder SetTableId(Guid tableId)
        {
            _tableId = tableId;
            return this;
        }
        public BettingBoxBuilder SetIdx(int idx)
        {
            _idx = idx;
            return this;
        }
        public BettingBoxBuilder SetOwner(Option<string> playerId)
        {
            _ownerId = playerId;
            return this;
        }
        public BettingBoxBuilder SetHands(ImmutableList<Hand> hands)
        {
            _hands = hands;
            return this;
        }
        public BettingBox Build()
        {
            return new BettingBox(_tableId, _idx, _ownerId, _hands);
        }

        public BettingBoxBuilder() {}
        public BettingBoxBuilder(BettingBox bettingBox)
        {
            _tableId = bettingBox.TableId;
            _idx = bettingBox.BoxIdx;
            _ownerId = bettingBox.OwnerId;
            _hands = bettingBox.Hands;
        }

        public void Dispose()
        {
            // TODO release managed resources here
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
