using Newtonsoft.Json;
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
    public sealed class Configuration : ConfigurationSingleton<Configuration>
    {
        [Category("Basic")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("Basic")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Securities { get; set; } = "[{\"c\":\"TQBR\",\"s\":\"LKOH\"},{\"c\":\"SPBFUT\",\"s\":\"RIZ0\"}]";

        [Category("Instance")]
        [JsonIgnore]
        public string InstanceType => "TestQuant";
    }
}
