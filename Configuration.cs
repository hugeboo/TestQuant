using Newtonsoft.Json;
using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Quant;
using QuantaBasket.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuantasBasket.Quantas.TestQuant
{
    [Configuration("Config\\TestQuant.dll.json")]
    public sealed class Configuration : AQuantConfigurationSingleton<Configuration>
    {
        public override string InstanceType => "TestQuant";

        [Category("Basic")]
        [DefaultValue(28)]
        public int SlowSMAPeriod { get; set; } = 14;

        [Category("Basic")]
        [DefaultValue(14)]
        public int FastSMAPeriod { get; set; } = 9;

        [Category("Basic")]
        [DefaultValue(BarInterval.Min1)]
        public BarInterval BarInerval { get; set; } = BarInterval.Min1;
    }
}
