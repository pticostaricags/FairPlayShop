﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.DataAccess.Models;

public partial class StoreCustomer
{
    [Key]
    public long StoreCustomerId { get; set; }

    public long StoreId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstSurname { get; set; }

    [Required]
    [StringLength(50)]
    public string MiddleSurname { get; set; }

    [Required]
    [StringLength(50)]
    public string EmailAddress { get; set; }

    [Required]
    [StringLength(50)]
    public string PhoneNumber { get; set; }

    [ForeignKey("StoreId")]
    [InverseProperty("StoreCustomer")]
    public virtual Store Store { get; set; }

    [InverseProperty("StoreCustomer")]
    public virtual ICollection<StoreCustomerAddress> StoreCustomerAddress { get; set; } = new List<StoreCustomerAddress>();

    [InverseProperty("StoreCustomer")]
    public virtual ICollection<StoreCustomerOrder> StoreCustomerOrder { get; set; } = new List<StoreCustomerOrder>();
}