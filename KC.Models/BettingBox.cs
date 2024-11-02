using LanguageExt;

namespace KC.Models;

public class BettingBox(Table parentTable, int boxId, Option<Player> owner, List<Hand> hands)
{
    public Table ParentTable { get; set; } = parentTable;
    public int BoxId { get; set; } = boxId;
    public Option<Player> Owner { get; set; } = owner;
    public List<Hand> Hands { get; set; } = hands;
}