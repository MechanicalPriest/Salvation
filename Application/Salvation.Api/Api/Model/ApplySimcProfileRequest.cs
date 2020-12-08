using Salvation.Core.Profile.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Api.Api.Model
{
    public class ApplySimcProfileRequest
    {
        public PlayerProfile Profile { get; set; }
        public string SimcProfileString { get; set; }
    }
}
