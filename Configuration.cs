using Newtonsoft.Json;
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
    [Configuration("TestQuant.dll.json")]
    public sealed class Configuration : AQuantConfigurationSingleton<Configuration>
    {
        public override string InstanceType => "TestQuant";
    }
}
