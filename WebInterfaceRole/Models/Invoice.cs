//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebInterfaceRole.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public System.DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
