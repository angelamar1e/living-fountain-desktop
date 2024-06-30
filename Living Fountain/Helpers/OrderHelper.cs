using Azure.Core;
using Living_Fountain.Models;
using System.Windows.Controls;
using System.Windows;

namespace Living_Fountain.Helpers
{
    public class OrderHelper
    { 
        public static DateOnly GetCurrentDate() 
        {
            DateTime date = DateTime.Now;
            DateOnly dateOnly = DateOnly.FromDateTime(date);

            return dateOnly;
        }

        public static DateOnly GetDateOnly(DateOnly? date)
        {
            DateOnly dateOnly = date ?? GetCurrentDate();

            return dateOnly;
        }

        public static customer GetOrCreateCustomer(living_fountainContext context, int block, int lot, int phase)
        {
            // Attempt to find the customer in the database
            var customer = context.customers
                .FirstOrDefault(c => c.block == block && c.lot == lot && c.phase == phase);

            // If the customer does not exist, create a new one
            if (customer == null)
            {
                customer = new customer
                {
                    block = block,
                    lot = lot,
                    phase = phase
                    // Add any other necessary fields here
                };

                context.customers.Add(customer);
                context.SaveChanges();
            }

            return customer;
        }

        public static decimal? ComputeNewPrice(int? quantity, decimal? price)
        {
            decimal? newPrice = quantity * price;

            return newPrice;
        }
    }
}
