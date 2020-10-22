using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantasBasket.Quantas.TestQuant
{
    public sealed class TestQuant : IQuant
    {
        public string Name => throw new NotImplementedException();

        public HashSet<SecurityId> Securities => throw new NotImplementedException();

        public IBasketService BasketService { set => throw new NotImplementedException(); }
    }
}
