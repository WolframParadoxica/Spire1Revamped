using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Spire1Revamped.Spire1RevampedCode.Cards;

namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class MillenniumEgg : Spire1RevampedRelic
{
  public const string StarterCardKey = "StarterCard";
  public const string AncientCardKey = "AncientCard";
  private SerializableCard? _serializableStarterCard;
  private SerializableCard? _serializableAncientCard;
  public List<IHoverTip> ExtraHoverTips1 = [];

  private static Dictionary<ModelId, CardModel> TranscendenceUpgrades =>
    new()
    {
      {
        ModelDb.Card<Bash>().Id,
        ModelDb.Card<Cripple>()
      },
      {
        ModelDb.Card<Survivor>().Id,
        ModelDb.Card<Rogue>()
      },
      {
        ModelDb.Card<Bodyguard>().Id,
        ModelDb.Card<Keeper>()
      },
      {
        ModelDb.Card<Venerate>().Id,
        ModelDb.Card<Ascend>()
      },
      {
        ModelDb.Card<Zap>().Id,
        ModelDb.Card<Discharge>()
      }
    };

  public static List<CardModel> TranscendenceCards => TranscendenceUpgrades.Values.ToList();

  public override RelicRarity Rarity => RelicRarity.Ancient;

  [SavedProperty]
  private SerializableCard? StarterCard { get => _serializableStarterCard; set { AssertMutable(); _serializableStarterCard = value; } }

  [SavedProperty]
  private SerializableCard? AncientCard { get => _serializableAncientCard; set { AssertMutable(); _serializableAncientCard = value; } }

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new StringVar("StarterCard"),
    new StringVar("AncientCard")
  ];

  protected override void AfterCloned()
  {
    base.AfterCloned();
    ExtraHoverTips1 = [];
  }

  public bool SetupForPlayer(Player player)
  {
    AssertMutable();
    var transcendenceStarterCard = GetTranscendenceStarterCard(player);
    if (transcendenceStarterCard is null)
      return false;
    StarterCard = transcendenceStarterCard.ToSerializable();
    AncientCard = GetTranscendenceTransformedCard(transcendenceStarterCard).ToSerializable();
    return true;
  }

  public void SetupForTests(SerializableCard starterCard, SerializableCard ancientCard)
  {
    AssertMutable();
    StarterCard = starterCard;
    AncientCard = ancientCard;
  }

  private static CardModel? GetTranscendenceStarterCard(Player player)
  {
    return player.Deck.Cards.FirstOrDefault(c => TranscendenceUpgrades.ContainsKey(c.Id));
  }

  private CardModel GetTranscendenceTransformedCard(CardModel starterCard)
  {
    if (!TranscendenceUpgrades.TryGetValue(starterCard.Id, out var canonicalCard))
      return Owner.RunState.CreateCard<Doubt>(starterCard.Owner);
    var card = starterCard.Owner.RunState.CreateCard(canonicalCard, starterCard.Owner);
    if (starterCard.IsUpgraded)
      CardCmd.Upgrade(card);
    if (starterCard.Enchantment is null)
      return card;
    var enchantment = (EnchantmentModel) starterCard.Enchantment.MutableClone();
    CardCmd.Enchant(enchantment, card, enchantment.Amount);
    return card;
  }

  protected override IEnumerable<IHoverTip> ExtraHoverTips
  {
    get
    {
      var extraHoverTips2 = new List<IHoverTip>();

      if (StarterCard is not null)
      {
        var cardModel = CardModel.FromSerializable(StarterCard);
        extraHoverTips2.AddRange(cardModel.HoverTips);
        extraHoverTips2.Add(HoverTipFactory.FromCard(cardModel));
        ((StringVar) DynamicVars["StarterCard"]).StringValue = cardModel.Title;
      }

      if (AncientCard is null)
        return extraHoverTips2;
      var cardModel2 = CardModel.FromSerializable(AncientCard);
      extraHoverTips2.AddRange(cardModel2.HoverTips);
      extraHoverTips2.Add(HoverTipFactory.FromCard(cardModel2));
      ((StringVar) DynamicVars["AncientCard"]).StringValue = cardModel2.Title;

      return extraHoverTips2;
    }
  }

  public override async Task AfterObtained()
  {
    var millenniumEgg = this;
    var transcendenceStarterCard = GetTranscendenceStarterCard(millenniumEgg.Owner);
    await CardCmd.Transform(transcendenceStarterCard!, millenniumEgg.GetTranscendenceTransformedCard(transcendenceStarterCard!));
  }
}