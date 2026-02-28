using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data.Models.ErrorModel
{
    public class ApplicationError
    {
        public string Message { get; set; } = string.Empty;

        public DateTime When { get; set; } = DateTime.Now;
    }
}
