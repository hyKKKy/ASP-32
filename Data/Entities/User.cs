﻿using System.Text.Json.Serialization;

namespace ASP_32.Data.Entities
{
    public class User
    {
        public Guid         Id              { get; set; }
        public string       Name            { get; set; } = null!;
        public string       Email           { get; set; } = null!;
        public DateTime?    Birthdate       { get; set; }
        public DateTime     RegisteredAt    { get; set; }
        public DateTime?    DeleteAt        {  get; set; }

        [JsonIgnore]
        public ICollection<UserAccess> Accesses { get; set; } = [];

        public ICollection<Cart> Carts { get; set; } = [];
    }
}
