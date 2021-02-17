using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Entities.Base
{
    public interface IBaseEntity<TID>
    {
        TID ID { get; set; }
    }
}
