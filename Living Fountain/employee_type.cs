﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Living_Fountain;

[Table("employee_types", Schema = "living_fountain")]
[Index("code", Name = "code")]
public partial class employee_type
{
    [Key]
    [StringLength(1)]
    public string code { get; set; }

    [StringLength(20)]
    public string emp_type_desc { get; set; }

    [InverseProperty("emp_type_codeNavigation")]
    public virtual ICollection<employee> employees { get; set; } = new List<employee>();

    [InverseProperty("emp_type_codeNavigation")]
    public virtual ICollection<salary_type> salary_types { get; set; } = new List<salary_type>();
}