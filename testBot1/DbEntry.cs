﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testBot1
{
    public class DbEntry
    {
        public int Id { get; set; }

        protected bool Equals(DbEntry other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DbEntry)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
