using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;
using XRL.World;

namespace XRL.World.Parts
{
  [Serializable]
  public class Drif_CombustingGasZone : IPart
  {
    public GameObject Owner;
    public int nFrameOffset = Stat.RandomCosmetic(0, 60);
    public int Duration = 3;
    public int Turn = 1;
    public int Level = 1;

    public override bool SameAs(IPart p) => true;

    public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == GeneralAmnestyEvent.ID;

    public override bool HandleEvent(GeneralAmnestyEvent E)
    {
      this.Owner = (GameObject) null;
      return true;
    }

    public override void Register(GameObject Object)
    {
      Object.RegisterPartEvent((IPart) this, "EndTurn");
      base.Register(Object);
    }

    public override bool FinalRender(RenderEvent E, bool bAlt)
    {
      if (bAlt || !this.Visible())
        return true;
      int num = (XRLCore.CurrentFrame + this.nFrameOffset) % 60;
      if (num < 15)
      {
        E.BackgroundString = "^W";
        E.DetailColor = "W";
      }
      else if (num < 30)
      {
        E.BackgroundString = "^r";
        E.DetailColor = "r";
      }
      else if (num < 45)
      {
        E.BackgroundString = "^R";
        E.DetailColor = "R";
      }
      else
      {
        E.BackgroundString = "^r";
        E.DetailColor = "r";
      }
      if (Stat.RandomCosmetic(1, 5) == 1)
      {
        E.RenderString = "°";
        E.ColorString = "&W";
      }
      else if (Stat.RandomCosmetic(1, 5) == 1)
      {
        E.RenderString = "±";
        E.ColorString = "&R";
      }
      else
      {
        E.RenderString = "±";
        E.ColorString = "&r";
      }
      this.ParentObject.pRender.ColorString = "&r^r";
      return true;
    }

    public override bool Render(RenderEvent E)
    {
      int num = (XRLCore.CurrentFrame + this.nFrameOffset) % 60;
      if (num < 15)
      {
        E.RenderString = "°";
        E.BackgroundString = "^W";
      }
      else if (num < 30)
      {
        E.RenderString = "±";
        E.BackgroundString = "^r";
      }
      else if (num < 45)
      {
        E.RenderString = "\x00B2";
        E.BackgroundString = "^R";
      }
      else
      {
        E.RenderString = "Û";
        E.BackgroundString = "^r";
      }
      this.ParentObject.pRender.ColorString = "&r^r";
      return true;
    }

    public override bool FireEvent(Event E)
    {
      if (E.ID == "EndTurn")
      {
        GameObject.validate(ref this.Owner);
        if (this.Duration > 0)
        {
          this.ParentObject.TemperatureChange((310 + 30 * this.Level) / 2, this.Owner);

          List<Cell> adjacentCells = this.ParentObject.CurrentCell.GetAdjacentCells(true);
          foreach (Cell cell in adjacentCells) 
          {
            cell.TemperatureChange((310 + 30 * this.Level) / 2, this.Owner);
          }

          ++this.Turn;
        }
        --this.Duration;
        if (this.Duration == 0)
        {
          this.ParentObject.Destroy();
          return false;
        }
      }
      return base.FireEvent(E);
    }
  }
}
