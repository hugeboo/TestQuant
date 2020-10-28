using Newtonsoft.Json;
using NLog;
using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Interfaces;
using QuantaBasket.Core.Mathx;
using QuantaBasket.Core.Messages;
using QuantaBasket.Core.Quant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantasBasket.Quantas.TestQuant
{
    public sealed class TestQuant : AQuant<Configuration>
    {
        private readonly ILogger _logger = LogManager.GetLogger("TestQuant");

        public override string QuantBaseName => "TestQuant";

        protected override Configuration Configuration => Configuration.Instance;
    }
}
