using HarmonyLib;
using RogueGenesia;
using RogueGenesia.Data;
using RogueGenesia.UI;

namespace BestiaryMilestonesMod;

[HarmonyPatch(typeof(EnemySO), nameof(EnemySO.GetBestiaryCompletionXP))]
public static class GetBestiaryCompletionXPPatch
{
    public static void Postfix(EnemySO __instance, ref double __result)
    {
        int special = Plugin.ComputeSpecialMilestone(
            __instance.killCount,
            __instance.EnemyTier,
            __instance.BestiaryPowerOverride);

        if (special > 0)
        {
            __result += Plugin.SpecialMilestoneXpBonus * special;
        }
    }
}

[HarmonyPatch(typeof(MonsterListPanel), nameof(MonsterListPanel.UpdateInformation))]
public static class MonsterListPanelUpdateInformationPatch
{
    private static readonly AccessTools.FieldRef<MonsterListPanel, EnemySO> EnemyRef =
        AccessTools.FieldRefAccess<MonsterListPanel, EnemySO>("m_enemy");

    public static void Postfix(MonsterListPanel __instance)
    {
        EnemySO enemy = EnemyRef(__instance);
        if (enemy == null) return;
        if (__instance.KillCount == null) return;

        int special = Plugin.ComputeSpecialMilestone(
            enemy.killCount,
            enemy.EnemyTier,
            enemy.BestiaryPowerOverride);

        bool atVanillaCap = enemy.BestiaryCompletion >= Plugin.VanillaMaxMilestone;

        if (special >= 1)
        {
            __instance.KillCount.text +=
                $"\n<color=#FFD24A>Special Milestone: {special} / {Plugin.MaxSpecialMilestone}</color>";
        }

        if (atVanillaCap && special < Plugin.MaxSpecialMilestone)
        {
            double next = Plugin.ComputeNextSpecialMilestoneThreshold(
                enemy.killCount,
                enemy.EnemyTier,
                enemy.BestiaryPowerOverride);

            if (next > 0)
            {
                __instance.KillCount.text +=
                    $"\nNext Special Milestone: {RGUtils.GetFormatedNumber((float)next)}";
            }
        }
    }
}
