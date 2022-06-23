using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AccountBalancer.Models
{
    public class AccountBalance
    {
        [Key]
        public string Id { get; set; }
        public decimal RDBalance { get; set; }
        public decimal CanteenBalance { get; set; }
        public decimal CarBalance { get; set; }
        public decimal MarketingBalance { get; set; }
        public decimal ParkingFinesBalance { get; set; }


    }
}
