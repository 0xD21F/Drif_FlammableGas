using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using XRL.Core;
using XRL.Rules;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World;
using XRL.UI;

namespace XRL.World.Parts
{
  [Serializable]
  public class Drif_GasFlammable : IPart
  {
    public string GasType = "Flammable";

    public int BaseForce = 1000;
    public string ExplosionDie = "d8";
    public int BaseTemperatureDelta = 500;
    public int ExplosionThreshold = 1000;

    public override bool SameAs(IPart p) => !((p as Drif_GasFlammable).GasType != this.GasType) && base.SameAs(p);

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID;

    public override bool WantTurnTick() => true;

    public override bool WantTenTurnTick() => true;

    public override bool WantHundredTurnTick() => true;

    public override void Register(GameObject Object)
    {
      Object.RegisterPartEvent((IPart) this, "DensityChange");
      base.Register(Object);
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "DensityChange" && this.StepValue(E.GetIntParameter("OldValue")) != this.StepValue(E.GetIntParameter("NewValue")))
        this.FlushNavigationCaches();

      return base.FireEvent(E);
    }

    public override bool HandleEvent(EndTurnEvent E)
    {
      Physics pPhysics = this.ParentObject.pPhysics;
      int temperature = pPhysics.Temperature;

      if(temperature > 200) {
        this.Combust();
      }

      return true;
    }

    public void Combust() {
      Gas gasPart = this.ParentObject.GetPart("Gas") as Gas;
      Cell currentCell = this.ParentObject.CurrentCell;
      int phase = this.ParentObject.GetPhase();

      // for each 200 density, gain 1 level (minimum 1, maximum 20)
      int densityLevel = Math.Min(gasPart.Density / 200 + 1, 20);

      // If density is low, burn
      if(gasPart.Density <= this.ExplosionThreshold) {
        this.DidX("combust", terminalPunctuation: "!");
        this.PlayWorldSound(this.GetPropertyOrTag("DetonatedSound", "grenade_heat"), 1f, combat: true);

        // Replace gas with "Burning Gas"
        GameObject GO = GameObjectFactory.Factory.CreateObject("Burning Gas");
        Drif_CombustingGasZone combustingGasPart = GO.GetPart("Drif_CombustingGasZone") as Drif_CombustingGasZone;
        combustingGasPart.Level = densityLevel;
        combustingGasPart.Owner = gasPart.Creator;
        currentCell.AddObject(GO);

        this.ParentObject.Destroy(Silent: true);
      }
      // Otherwise explode
      else {
        this.PlayWorldSound(this.GetPropertyOrTag("DetonatedSound", "grenade_heat"), 1f, combat: true);
        this.DidX("explode", terminalPunctuation: "!");

        int explosiveForce = (int)((this.BaseForce * densityLevel) / 2);

        List<Cell> adjacentCells = currentCell.GetAdjacentCells((int)Math.Min(densityLevel, 6));
        foreach (Cell cell in adjacentCells) 
        {
          cell.TemperatureChange(310 + 30 * densityLevel, gasPart.Creator);
        }

        this.ParentObject.Explode(explosiveForce, gasPart.Creator, densityLevel + this.ExplosionDie);
      }
    }
  }
}
