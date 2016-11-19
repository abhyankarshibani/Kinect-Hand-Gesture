using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureData
{
   public class GestureEventArgs : EventArgs
    {
       public RecogniedResult Result1{get; internal set;}
       public GestureEventArgs(RecogniedResult result)
         {
             this.Result1 = result;
         }
    }
}
