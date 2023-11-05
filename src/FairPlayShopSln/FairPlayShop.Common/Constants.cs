namespace FairPlayShop.Common
{
    public static class Constants
    {
        public static class ApiRoutes
        {
            public const string CreateMyProduct = $"/Product/{nameof(CreateMyProduct)}";
            public const string MyProductList = $"/Product/{nameof(MyProductList)}";
            public const string CreateMyStore = $"/Store/{nameof(CreateMyStore)}";
            public const string MyStoreList = $"/Store/{nameof(MyStoreList)}";
            public const string CreateMyStoreCustomer = $"/StoreCustomer/{nameof(CreateMyStoreCustomer)}";
            public const string MyStoreCustomerList = $"/StoreCustomer/{nameof(MyStoreCustomerList)}";
            public const string CreateStoreCustomerOrder = $"/StoreCustomerOrder/{nameof(CreateStoreCustomerOrder)}";
            public const string StoreCustomerOrderList = $"/StoreCustomerOrder/{nameof(StoreCustomerOrderList)}";
        }

        public static class Pagination
        {
            public const int PageSize = 2;
        }
    }
}
