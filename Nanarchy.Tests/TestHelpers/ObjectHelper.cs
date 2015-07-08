﻿using System;

namespace Dell.Hierarchy.Tests.TestHelpers
{
    public static class ObjectHelper
    {
         public static T CastTo<T>(this object obj)
         {
             try
             {
                 return (T) obj;
             }
             catch (Exception)
             {
                 return default(T);
             }
         }
    }
}