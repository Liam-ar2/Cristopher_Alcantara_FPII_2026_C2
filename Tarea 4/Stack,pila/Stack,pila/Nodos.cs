using System;
using System.Collections.Generic;
using System.Text;

namespace Stack_pila
{
   class CNodo
   {
     private int dato;

     private CNodo siguiente = null;

     public int Dato { get => dato; set => dato = value; }
     internal CNodo Siguiente { get => siguiente; set => siguiente = value; }

     public override string ToString()
     {
       return string.Format("[{0}]", dato);
     }
   }
}
