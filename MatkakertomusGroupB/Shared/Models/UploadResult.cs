using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatkakertomusGroupB.Shared.Models
{
    public class UploadResult
    {
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        // this will be used to randomize the uploaded filename on server
    }
}
