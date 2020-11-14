using System;
using System.Collections.Generic;
using System.Threading;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Capabilities;
using XRL.World.Parts;
using XRL.World.Effects;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{
	[Serializable]
	public class Drif_FlammableGasGeneration : GasGeneration
	{
    public Drif_FlammableGasGeneration()
    {
      this.GasObject = "FlammableGas";
      this.SyncFromBlueprint();
    }

    public override int GetReleaseDuration(int Level) => 1 + Level / 2;

    public override int GetReleaseCooldown(int Level) => 40;

    public override string GetReleaseAbilityName() => "Release Flammable Gas";

    public override int GetGasDensityForLevel(int Level) => 200 * Level;
  }
}
