//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQL2ActiveDirectory
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeOrganChart
    {
        public long Id { get; set; }
        public long OrganChart_Id { get; set; }
        public long Employee_Id { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Description { get; set; }
        public byte[] TimeStamp { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
