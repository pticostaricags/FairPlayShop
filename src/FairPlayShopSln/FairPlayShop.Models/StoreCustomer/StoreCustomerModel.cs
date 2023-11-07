namespace FairPlayShop.Models.StoreCustomer
{
    public class StoreCustomerModel
    {
        public long StoreCustomerId { get; set; }
        public long StoreId { get; set; }
        public string? Name { get; set; }
        public string? FirstSurname { get; set; }
        public string? SecondSurname { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
