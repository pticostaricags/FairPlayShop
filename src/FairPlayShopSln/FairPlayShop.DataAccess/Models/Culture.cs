﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.DataAccess.Models;

[Index("Name", Name = "UI_Culture_Name", IsUnique = true)]
public partial class Culture
{
    [Key]
    public int CultureId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [InverseProperty("Culture")]
    public virtual ICollection<Resource> Resource { get; set; } = new List<Resource>();
}