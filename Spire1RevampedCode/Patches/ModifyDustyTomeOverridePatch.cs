using BaseLib;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Spire1Revamped.Spire1RevampedCode.Patches;

[HarmonyPatch(typeof (DustyTome), "SetupForPlayer")]
internal class DustyTomePatch
{
    private static bool _initialized = false;
    private static readonly Dictionary<ModelId, List<ModelId>> _customTome = new Dictionary<ModelId, List<ModelId>>();

    private static Dictionary<ModelId, List<ModelId>> CustomTome
    {
        get
        {
            if (DustyTomePatch._initialized)
                return DustyTomePatch._customTome;
            DustyTomePatch._initialized = true;
            int num = 0;
            foreach (CardModel allCard in ModelDb.AllCards)
            {
                if (allCard is ITomeCard tomeCard)
                {
                    List<ModelId> modelIdList;
                    if (!DustyTomePatch._customTome.TryGetValue(tomeCard.TomeCharacter.Id, out modelIdList))
                    {
                        modelIdList = new List<ModelId>();
                        DustyTomePatch._customTome[tomeCard.TomeCharacter.Id] = modelIdList;
                    }
                    modelIdList.Add(allCard.Id);
                    ++num;
                }
            }
            BaseLibMain.Logger.Info($"Initialized DustyTome dictionary; found {num} ITomeCard implementations");
            return DustyTomePatch._customTome;
        }
    }

    [HarmonyPrefix]
    private static bool DustyTomeCardOverride(DustyTome __instance, Player player)
    {
        List<ModelId> items;
        if (!DustyTomePatch.CustomTome.TryGetValue(player.Character.Id, out items))
        {
            switch (player.Character)
            {
                case Ironclad:
                    __instance.AncientCard = ModelDb.Card<Corruption>().Id;
                    return false;
                case Silent:
                    __instance.AncientCard = ModelDb.Card<WraithForm>().Id;
                    return false;
                case Regent:
                    __instance.AncientCard = ModelDb.Card<TheSealedThrone>().Id;
                    return false;
                case Necrobinder:
                    __instance.AncientCard = ModelDb.Card<WraithForm>().Id;
                    return false;
                case Defect:
                    __instance.AncientCard = ModelDb.Card<BiasedCognition>().Id;
                    return false;
            }
            return true;
        }
        __instance.AncientCard = player.PlayerRng.Rewards.NextItem<ModelId>((IEnumerable<ModelId>) items);
        return false;
    }
}