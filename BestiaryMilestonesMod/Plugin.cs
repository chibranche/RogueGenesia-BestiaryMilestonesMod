using System;
using HarmonyLib;
using ModGenesia;
using RogueGenesia.Data;
using UnityEngine;

namespace BestiaryMilestonesMod;

public class Plugin : RogueGenesiaMod
{
    public const int VanillaMaxMilestone = 5;
    public const int TotalMaxMilestone = 50;
    public const int MaxSpecialMilestone = TotalMaxMilestone - VanillaMaxMilestone; // 45
    public const float SpecialMilestoneXpBonus = 0.05f;

    private const string HarmonyId = "paper.bestiarymilestones";

    private static Harmony _harmony;

    public override void OnModLoaded(ModData modData)
    {
        Debug.Log("[BestiaryMilestones] OnModLoaded - applying Harmony patches");
        try
        {
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll(typeof(Plugin).Assembly);
            Debug.Log("[BestiaryMilestones] Harmony patches applied successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("[BestiaryMilestones] Failed to apply Harmony patches: " + e);
        }
    }

    public override void OnModUnloaded()
    {
        if (_harmony != null)
        {
            _harmony.UnpatchAll(HarmonyId);
            _harmony = null;
        }
    }

    public static int ComputeSpecialMilestone(double killCount, EEnemyTier tier, float powerOverride)
    {
        if (powerOverride > 0f)
        {
            if (killCount <= 0.0) return 0;
            int total = Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)killCount, powerOverride)) + 1, 0, TotalMaxMilestone);
            return Math.Max(0, total - VanillaMaxMilestone);
        }

        if (!TryGetExtensionParams(tier, out double firstThreshold, out double step)) return 0;
        if (killCount < firstThreshold) return 0;

        int count = (int)Math.Floor((killCount - firstThreshold) / step) + 1;
        return Mathf.Clamp(count, 0, MaxSpecialMilestone);
    }

    public static double ComputeNextSpecialMilestoneThreshold(double killCount, EEnemyTier tier, float powerOverride)
    {
        int current = ComputeSpecialMilestone(killCount, tier, powerOverride);
        if (current >= MaxSpecialMilestone) return -1.0;

        if (powerOverride > 0f)
        {
            int total = Mathf.Clamp(Mathf.FloorToInt(Mathf.Log(Math.Max(1f, (float)killCount), powerOverride)) + 1, 0, TotalMaxMilestone);
            int nextTotal = Math.Max(VanillaMaxMilestone + 1, total + 1);
            if (nextTotal > TotalMaxMilestone) return -1.0;
            return Mathf.Pow(powerOverride, nextTotal);
        }

        if (!TryGetExtensionParams(tier, out double firstThreshold, out double step)) return -1.0;
        return firstThreshold + step * current;
    }

    private static bool TryGetExtensionParams(EEnemyTier tier, out double firstThreshold, out double step)
    {
        switch (tier)
        {
            case EEnemyTier.Normal:   firstThreshold = 20_000; step = 10_000; return true;
            case EEnemyTier.Elite:    firstThreshold = 1_000;  step = 500;    return true;
            case EEnemyTier.Boss:     firstThreshold = 32;     step = 16;     return true;
            case EEnemyTier.MiniBoss: firstThreshold = 160;    step = 80;     return true;
            default: firstThreshold = 0; step = 0; return false;
        }
    }
}
