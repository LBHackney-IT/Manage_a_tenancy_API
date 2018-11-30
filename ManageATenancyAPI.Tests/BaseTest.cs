﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ManageATenancyAPI.Tests
{
   public abstract class BaseTest
   {
       protected IConfigurationRoot Config;

        public BaseTest()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
       public static T GetConfiguration<T>(IConfiguration config, string key) where T : new()
       {
           var instance = new T();
           config.GetSection(key).Bind(instance);
           return instance;
       }
    }
}
