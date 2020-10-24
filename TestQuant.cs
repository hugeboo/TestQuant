using Newtonsoft.Json;
using NLog;
using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Interfaces;
using QuantaBasket.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantasBasket.Quantas.TestQuant
{
    public sealed class TestQuant : IQuant
    {
        private readonly string _name;
        private readonly HashSet<SecurityId> _securities;

        private IBasketService _basketService;
        private readonly ILogger _logger = LogManager.GetLogger("TestQuant");

        private QuantStatus _status = QuantStatus.Idle;

        public string Name => _name;

        public HashSet<SecurityId> Securities => _securities;

        public IBasketService BasketService { set => _basketService = value ?? throw new ArgumentNullException("BasketService"); }
        public QuantStatus Status => _status;

        public TestQuant()
        {
            try
            {
                var dsec = JsonConvert.DeserializeAnonymousType(Properties.Settings.Default.Securities, new[] { new { c = "", s = "" } });
                _name = "TestQuant: " + string.Join(",", dsec.Select(d => d.s));
                _securities = dsec.Select(d => new SecurityId { ClassCode = d.c, SecurityCode = d.s }).ToHashSet();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Cannot initialize. Securites = {Properties.Settings.Default.Securities}");
                throw;
            }
        }

        public void Init()
        {
            _logger.Info($"Initialize quant: {_name}");
            _basketService.RegisterMessageProcessor(MessageProcessor);
        }

        private void MessageProcessor(AMessage message)
        {
            try
            {
                switch (message)
                {
                    case ErrorMessage em:
                        _logger.Debug("em");
                        break;
                    case L1QuotationsMessage qm:
                        _logger.Debug("qm");
                        break;
                    case TimerMessage tm:
                        _logger.Debug("tm");
                        //var s = _basketService.CreateSignal();
                        //s.ClassCode = "TQBR";
                        //s.SecCode = "LKOH";
                        //s.Side = SignalSide.Buy;
                        //s.Qtty = 10;
                        //s.Price = 3999.0m;
                        //s.PriceType = PriceType.Limit;
                        //_basketService.SendSignal(s);
                        break;
                    case StartMessage sm:
                        _logger.Debug("sm");
                        _status = QuantStatus.Active;
                        break;
                    case StopMessage stm:
                        _logger.Debug("stm");
                        _status = QuantStatus.Idle;
                        break;
                    case null:
                        throw new NullReferenceException("message");
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Cannot process message {message}");
            }
        }

        public void Dispose()
        {
            _logger.Debug($"{_name} disposing");
        }
    }
}
