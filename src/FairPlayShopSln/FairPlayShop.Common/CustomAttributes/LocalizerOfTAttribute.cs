namespace FairPlayShop.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LocalizerOfTAttribute<T> : Attribute where T : class
    {
    }
}
