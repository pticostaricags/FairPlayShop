namespace FairPlayShop.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceKeyAttribute : Attribute
    {
        public string DefaultValue { get; }
        public ResourceKeyAttribute(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
    }
}
