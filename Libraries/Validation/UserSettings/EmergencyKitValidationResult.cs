﻿using DataLayer.Mongo.Entities;

namespace Validation.UserSettings
{
    public class EmergencyKitValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set; }
    }
}