﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctms.Domain
{
    //Stores keywords in a hierarchical order below their Types
    public class KeywordContainer
    {
        public Dictionary<KeywordTypes, List<Keyword>> Container;
    }
}