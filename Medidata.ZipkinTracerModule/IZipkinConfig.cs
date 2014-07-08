﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medidata.ZipkinTracerModule
{
    public interface IZipkinConfig
    {
        string ZipkinServerName { get; }
        string ZipkinServerPort { get; }
        string ServiceName { get; }
    }
}