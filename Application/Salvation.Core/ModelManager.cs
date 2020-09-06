using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core
{
    public class ModelManager
    {
        // TODO: This class doesn't really need to be instanced, right?
        public static BaseModel LoadModel(BaseProfile profile, GlobalConstants constants)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            if (constants == null)
                throw new ArgumentNullException("constants");

            BaseModel model;

            switch ((int)profile.SpecId)
            {
                case 257:
                    model = new HolyPriestModel(constants, profile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        "profile.specid", "Invalid SpecId provided for profile");
            }

            return model;
        }
    }
}
