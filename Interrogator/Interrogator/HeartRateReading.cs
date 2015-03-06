using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interrogator
{
  public enum BandReadingState
  {
    NotWorn, Acquiring, Locked
  }

  public struct HeartRateReading
  {
    public int? Reading { get; set; }
    public BandReadingState ReadingState { get; set; }
  }
}
