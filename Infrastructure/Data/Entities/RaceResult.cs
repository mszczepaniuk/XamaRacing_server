﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Data.Entities
{
    public class RaceResult
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        [JsonIgnore]
        public RaceMap Race { get; set; }
        public string Nickname { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}