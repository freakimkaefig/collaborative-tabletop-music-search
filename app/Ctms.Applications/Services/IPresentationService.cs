﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctms.Applications.Services
{
    public interface IPresentationService
    {
        double VirtualScreenWidth { get; }

        double VirtualScreenHeight { get; }


        void InitializeCultures();
    }
}
