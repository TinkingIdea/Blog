using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Blog.Core.Entities
{
    public abstract class Entity
    {
        public static readonly DateTime EntityInitialDate = new DateTime(2020, 3, 19, 0, 0, 0);
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime CreatedAtUtc { get; set; } = EntityInitialDate;
        public DateTime ModifiedAtUtc { get; set; } = EntityInitialDate;
    }
}
