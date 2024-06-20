using AXService.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Dtos
{
    public class TaskQueueRequest
    {
        public string fileId { get; set; }
        public string fileUrl { get; set; }
        public string function { get; set; } = nameof(EnumTaskQueue.Function.processTaxInvoice);
        public string requestId { get; set; }

        //public string sender { get; set; }
        //public string char_type { get; set; } = nameof(EnumTaskQueue.CharType.TEXT);
        //public List<string> languages { get; set; } = new List<string> { nameof(EnumTaskQueue.Language.vi)};
    }
}
