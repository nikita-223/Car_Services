//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarServcing.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Customer
    {
        public int UserId { get; set; }
      
        [Display(Name ="Account Balance")]
        public Nullable<decimal> AccountBalance { get; set; }
    
        public virtual UserDetail UserDetail { get; set; }
    }
}