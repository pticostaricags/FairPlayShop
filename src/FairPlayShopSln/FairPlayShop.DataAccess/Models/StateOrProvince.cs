﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.DataAccess.Models;

[Index("CountryId", "Name", Name = "UI_StateOrProvince_Name", IsUnique = true)]
public partial class StateOrProvince
{
    [Key]
    public int StateOrProvinceId { get; set; }

    [Required]
    [StringLength(500)]
    public string Name { get; set; }

    public int CountryId { get; set; }

    [InverseProperty("StateOrProvince")]
    public virtual ICollection<City> City { get; set; } = new List<City>();

    [ForeignKey("CountryId")]
    [InverseProperty("StateOrProvince")]
    public virtual Country Country { get; set; }
}