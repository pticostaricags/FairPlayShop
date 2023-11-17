﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayShop.DataAccess.Models;

public partial class ErrorLog
{
    [Key]
    public long ErrorLogId { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    public string StackTrace { get; set; }

    [Required]
    public string FullException { get; set; }

    public DateTimeOffset RowCreationDateTime { get; set; }

    [Required]
    [StringLength(250)]
    public string SourceApplication { get; set; }
}