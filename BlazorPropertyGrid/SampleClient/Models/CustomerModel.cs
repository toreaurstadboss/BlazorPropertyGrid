using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorSampleClient.Models
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            EnrollementDate = DateTime.Today;
            Address = new AddressInfo();
            Gender = new GenderInfo();
        }

        [Required]
        [StringLength(40, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        public AddressInfo Address { get; set; }

        [Range(0,125, ErrorMessage = "Age must be 0-125 years.")]
        public int Age { get; set; }

        public string City { get; set; }

        public DateTime EnrollementDate { get; set; }

        public GenderInfo Gender { get; set; }

    }

    public class GenderInfo
    {
        public Gender Gender { get; set; }

    }

    public enum Gender
    {
        Female,
        Male,
        Other
    }

    public class AddressInfo
    {
        public AddressInfo()
        {
            AddressDetails = new AddressInfoDetails();
        }

        [Required]
        [Range(0, 9999, ErrorMessage = "Zip code must be between 0 and 9999.")]
        public int Zipcode { get; set; }

        [StringLength(maximumLength:3, ErrorMessage = "Max length: 3 (Use abbreviation")]
        public string State { get; set; }

        public AddressInfoDetails AddressDetails { get; set; }


    }

    public class AddressInfoDetails
    {
        public string Box { get; set; }

        public string StreetAddress { get; set; }

    }


}
