using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.AzureOpenAI
{
    public class TranslationRequest
    {
        public string? OriginalText { get; set; }
        public string? SourceLocale { get; set; }
        public string? DestLocale { get; set; }
    }
}
