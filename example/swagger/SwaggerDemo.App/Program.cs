﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YS.Knife.Hosting;

namespace SwaggerDemo.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            KnifeWebHost.Start(args);
        }
    }
}
