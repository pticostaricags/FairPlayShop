namespace FairPlayShop.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServerSideServiceOfTAttribute<T> : Attribute where T : class
    {

    }
}
