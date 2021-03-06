﻿using Microsoft.Extensions.Options;

namespace YS.Knife.Options
{
    [Options]
    public class Custom8Options
    {
        public int Number { get; set; }
    }

    [OptionsValidateHandler]
    public class Custom8Validator : IValidateOptions<Custom8Options>
    {
        public ValidateOptionsResult Validate(string name, Custom8Options options)
        {
            if (options.Number % 2 == 1)
            {
                return ValidateOptionsResult.Fail("Number should be an even number.");
            }
            else
            {
                return ValidateOptionsResult.Success;
            }
        }
    }
}
