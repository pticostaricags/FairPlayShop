using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Common.CustomExceptions
{
    public class ConfigurationException(string? message) : Exception(message)
    {
    }
}
