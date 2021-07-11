﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace YS.Knife.Data
{
    public class OrderItem2
    {
        public ValueInfo Value { get; set; }
        public OrderType OrderType { get; set; }

        public static OrderItem2 FromValuePaths(List<ValuePath> paths)
        {
            var last = paths?.LastOrDefault();

            if (last != null && last.IsFunction)
            {
                if (string.Equals(last.Name, "desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderItem2
                    {
                        Value = new ValueInfo
                        {
                            IsConstant = false,
                            NavigatePaths = paths.Where(p => p != last).ToList()
                        },
                        OrderType = OrderType.Desc
                    };
                }
                else if (string.Equals(last.Name, "asc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderItem2
                    {
                        Value = new ValueInfo
                        {
                            IsConstant = false,
                            NavigatePaths = paths.Where(p => p != last).ToList()
                        },
                        OrderType = OrderType.Desc
                    };
                }
            }
            return new OrderItem2
            {
                Value = new ValueInfo
                {
                    IsConstant = false,
                    NavigatePaths = paths
                },
                OrderType = OrderType.Asc
            };
        }

        public static OrderItem2 FromValueInfo(ValueInfo value)
        {
            if (value == null) return null;

            if (!value.IsConstant)
            {
                return FromValuePaths(value.NavigatePaths);
            }

            return new OrderItem2 { Value = value, OrderType = OrderType.Asc };

        }
    }
}
