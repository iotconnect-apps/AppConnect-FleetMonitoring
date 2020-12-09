using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class DeviceFiles
    {
        public Guid Guid { get; set; }
        //public Guid GeneratorGuid { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public Guid CreatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public Guid? UpdatedBy { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        //public ICollection<FileDetail> FileDetails { get; set; }
    }

   public class file
    {
        public string path { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
    }
}
