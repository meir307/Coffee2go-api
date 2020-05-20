using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Tools
{
    public static class Validator
    {
        public static  void validateLocation(float lat, float lng)
        {
            if (lat == 0 || lng == 0)
                throw new Exception("Bad location data.");
        }

        public static void validatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new Exception("Password is missing.");
        }

        public static void validateEmail( string Email)
        {
            if (string.IsNullOrEmpty(Email))
                throw new Exception("Email is missing.");
        }

        public static void validatePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                throw new Exception("phone is missing.");
        }
    }
}
