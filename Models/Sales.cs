//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sales
    {
        public int sales_id { get; set; }
        public int user_id { get; set; }
        public System.DateTime date { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public int unit_cost { get; set; }
        public int total_cost { get; set; }
    
        public virtual Inventory Inventory { get; set; }
        public virtual User User { get; set; }
    }
}
