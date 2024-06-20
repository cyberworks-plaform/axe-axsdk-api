using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Enums
{
    public static class EnumTaskQueue
    {
        public enum Language
        {
            vi,
        }

        public enum Function
        {
            ocr,
            processTaxInvoice,
            processAutoInsurance
        }
        public enum CharType
        {
            TEXT,
            HANDWRI
        }

    }
}
