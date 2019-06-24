using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.Database.Models
{
    public class NewTenancyLastRun
    {
        [Key] public int Id { get; set; }

        public DateTime LastRun { get; set; }
    }

    /// <summary>
    /// Interface describing base entity fields
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        [Key]
        int Id { get; set; }

        /// <summary>
        /// Tracks when the entity was last modified
        /// </summary>
        DateTime LastModified { get; set; }

        /// <summary>
        /// Tracks when the entity was created
        /// </summary>
        DateTime CreatedAt { get; set; }
    }

    public class BaseEntity : IBaseEntity
    {
        [Key] public int Id { get; set; }
        

        public DateTime LastModified { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NamedBaseEntity : BaseEntity
    {
        /// <summary>
        /// Name of the entity
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; }
    }


    public class TRA: NamedBaseEntity
    {
        public ICollection<TRAEstate> Estates { get; set; }
    }

    public class TRAEstate : NamedBaseEntity
    {
        public string UHReference { get; set; }
    }

    public class HousingArea : NamedBaseEntity
    {
        public ICollection<HousingAreaPatch> Patches { get; set; }
    }

    public class HousingAreaPatch : NamedBaseEntity
    {
        public ICollection<TRA> TRAs { get; set; }
    }
}