using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Spire1Revamped.Spire1RevampedCode.Cards;

#nullable enable
namespace Spire1Revamped.Spire1RevampedCode.Relics;

[Pool(typeof(EventRelicPool))]
public class MillenniumEgg : Spire1RevampedRelic
{
  public const string _starterCardKey = "StarterCard";
  public const string _ancientCardKey = "AncientCard";
  public SerializableCard? _serializableStarterCard;
  public SerializableCard? _serializableAncientCard;
  public List<IHoverTip> _extraHoverTips = new List<IHoverTip>();

  public static Dictionary<ModelId, CardModel> TranscendenceUpgrades
  {
    get
    {
      return new Dictionary<ModelId, CardModel>()
      {
        {
          ModelDb.Card<Bash>().Id,
          (CardModel) ModelDb.Card<Cripple>()
        },
        {
          ModelDb.Card<Survivor>().Id,
          (CardModel) ModelDb.Card<Rogue>()
        },
        {
          ModelDb.Card<Bodyguard>().Id,
          (CardModel) ModelDb.Card<Protector>()
        },
        {
          ModelDb.Card<Venerate>().Id,
          (CardModel) ModelDb.Card<Ascend>()
        },
        {
          ModelDb.Card<Zap>().Id,
          (CardModel) ModelDb.Card<Quadcast>()
        }
      };
    }
  }

  public static List<CardModel> TranscendenceCards
  {
    get => MillenniumEgg.TranscendenceUpgrades.Values.ToList<CardModel>();
  }

  public override RelicRarity Rarity => RelicRarity.Ancient;

  [SavedProperty]
  public SerializableCard? StarterCard
  {
    get => this._serializableStarterCard;
    set
    {
      this.AssertMutable();
      this._serializableStarterCard = value;
      this.UpdateHoverTips();
    }
  }

  [SavedProperty]
  public SerializableCard? AncientCard
  {
    get => this._serializableAncientCard;
    set
    {
      this.AssertMutable();
      this._serializableAncientCard = value;
      this.UpdateHoverTips();
    }
  }

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    (DynamicVar) new StringVar("StarterCard"),
    (DynamicVar) new StringVar("AncientCard")
  ];

  protected override void AfterCloned()
  {
    base.AfterCloned();
    this._extraHoverTips = new List<IHoverTip>();
  }

  public bool SetupForPlayer(Player player)
  {
    this.AssertMutable();
    CardModel transcendenceStarterCard = this.GetTranscendenceStarterCard(player);
    if (transcendenceStarterCard == null)
      return false;
    this.StarterCard = transcendenceStarterCard.ToSerializable();
    this.AncientCard = this.GetTranscendenceTransformedCard(transcendenceStarterCard).ToSerializable();
    this.UpdateHoverTips();
    return true;
  }

  public void SetupForTests(SerializableCard starterCard, SerializableCard ancientCard)
  {
    this.AssertMutable();
    this.StarterCard = starterCard;
    this.AncientCard = ancientCard;
    this.UpdateHoverTips();
  }

  public void UpdateHoverTips()
  {
    this._extraHoverTips.Clear();
    if (this.StarterCard != null)
    {
      CardModel card = CardModel.FromSerializable(this.StarterCard);
      this._extraHoverTips.AddRange(card.HoverTips);
      this._extraHoverTips.Add(HoverTipFactory.FromCard(card));
      ((StringVar) this.DynamicVars["StarterCard"]).StringValue = card.Title;
    }
    if (this.AncientCard == null)
      return;
    CardModel card1 = CardModel.FromSerializable(this.AncientCard);
    this._extraHoverTips.AddRange(card1.HoverTips);
    this._extraHoverTips.Add(HoverTipFactory.FromCard(card1));
    ((StringVar) this.DynamicVars["AncientCard"]).StringValue = card1.Title;
  }

  public CardModel? GetTranscendenceStarterCard(Player player)
  {
    return player.Deck.Cards.FirstOrDefault<CardModel>((Func<CardModel, bool>) (c => MillenniumEgg.TranscendenceUpgrades.ContainsKey(c.Id)));
  }

  public CardModel GetTranscendenceTransformedCard(CardModel starterCard)
  {
    CardModel canonicalCard;
    if (!MillenniumEgg.TranscendenceUpgrades.TryGetValue(starterCard.Id, out canonicalCard))
      return (CardModel) this.Owner.RunState.CreateCard<Doubt>(starterCard.Owner);
    CardModel card = starterCard.Owner.RunState.CreateCard(canonicalCard, starterCard.Owner);
    if (starterCard.IsUpgraded)
      CardCmd.Upgrade(card);
    if (starterCard.Enchantment != null)
    {
      EnchantmentModel enchantment = (EnchantmentModel) starterCard.Enchantment.MutableClone();
      CardCmd.Enchant(enchantment, card, (Decimal) enchantment.Amount);
    }
    return card;
  }

  protected override IEnumerable<IHoverTip> ExtraHoverTips
  {
    get => (IEnumerable<IHoverTip>) this._extraHoverTips;
  }

  public override async Task AfterObtained()
  {
    MillenniumEgg millenniumEgg = this;
    CardModel transcendenceStarterCard = millenniumEgg.GetTranscendenceStarterCard(millenniumEgg.Owner);
    CardPileAddResult? nullable = await CardCmd.Transform(transcendenceStarterCard, millenniumEgg.GetTranscendenceTransformedCard(transcendenceStarterCard));
  }
}