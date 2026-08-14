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
    private static bool _initialized;
    private static readonly Dictionary<ModelId, List<ModelId>> CustomTomeDict = new();

    private static Dictionary<ModelId, List<ModelId>> CustomTome
    {
        get
        {
            if (_initialized)
                return CustomTomeDict;
            _initialized = true;
            var num = 0;
            foreach (var allCard in ModelDb.AllCards)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (allCard is not ITomeCard tomeCard) continue;
                if (!CustomTomeDict.TryGetValue(tomeCard.TomeCharacter.Id, out var modelIdList))
                {
                    modelIdList = [];
                    CustomTomeDict[tomeCard.TomeCharacter.Id] = modelIdList;
                }
                modelIdList.Add(allCard.Id);
                ++num;
            }
            BaseLibMain.Logger.Info($"Initialized DustyTome dictionary; found {num} ITomeCard implementations");
            return CustomTomeDict;
        }
    }

    [HarmonyPrefix]
    private static bool DustyTomeCardOverride(DustyTome __instance, Player player)
    {
        if (!CustomTome.TryGetValue(player.Character.Id, out var items))
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
                    __instance.AncientCard = ModelDb.Card<ForbiddenGrimoire>().Id;
                    return false;
                case Defect:
                    __instance.AncientCard = ModelDb.Card<BiasedCognition>().Id;
                    return false;
            }
            return true;
        }
        __instance.AncientCard = player.PlayerRng.Rewards.NextItem(items);
        return false;
    }
}