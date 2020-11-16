using System;
using System.Collections.Generic;
using XRL;
using XRL.Core;

/**
 * Population tables currently not modifiable via XML - this is a workaround to update the population tables at run-time.
 *
 * The [WantLoadBlueprint] tag and a corresponding object blueprint cause the LoadBlueprint method to run on mod load,
 * and from there we dynamically add our grenades to the Explosives lists. This will allow them to be sold by Grenadiers
 * and used in any other part of the game where the Explosives 1-3 population tables are used.
 */
namespace XRL.World.Parts
{
  [Serializable]
  [WantLoadBlueprint]
  public class Drif_FlammableGasInitialiser : IPart
  {
    public override void LoadBlueprint() {
      this.AddToPopTable("Explosives 1", new PopulationObject { Blueprint = "FlammableGasGrenade1", Number="1", Weight=50 });
      this.AddToPopTable("Explosives 2", new PopulationObject { Blueprint = "FlammableGasGrenade2", Number="1", Weight=50 });
      this.AddToPopTable("Explosives 3", new PopulationObject { Blueprint = "FlammableGasGrenade3", Number="1", Weight=50 });
    }

    public bool AddToPopTable(string table, params PopulationItem[] items) {
      PopulationInfo info;
      if (!PopulationManager.Populations.TryGetValue(table, out info))
        return false;
          
      // If this is a single group population, add to that group.
      if (info.Items.Count == 1 && info.Items[0] is PopulationGroup) { 
        var group = info.Items[0] as PopulationGroup;
        group.Items.AddRange(items);
        return true;
      }

      info.Items.AddRange(items);
      return true;
    }

  }
}