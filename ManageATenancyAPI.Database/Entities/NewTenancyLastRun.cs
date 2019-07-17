using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace ManageATenancyAPI.Database.Entities
{
    public class NewTenancyLastRun
    {
        [Key] public int Id { get; set; }

        public DateTime LastRun { get; set; }
    }

    public interface IEntity
    {
        int Id { get; set; }

        DateTime Created { get; set; }
    }

    public class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
    }
}