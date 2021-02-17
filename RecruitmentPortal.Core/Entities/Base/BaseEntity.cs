using System;
using System.Collections.Generic;
using System.Text;

namespace RecruitmentPortal.Core.Entities.Base
{
    public abstract class BaseEntity<TID> : IBaseEntity<TID>
    {
        public virtual TID ID { get; set; }
    }
}
