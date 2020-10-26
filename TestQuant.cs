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
    public sealed class TestQuant : IQuant, IHaveConfiguration
    {
        private readonly string _name;
        private readonly HashSet<SecurityId> _securities;

        private IBasketService _basketService;
        private readonly ILogger _logger = LogManager.GetLogger("TestQuant");

        private QuantStatus _status = Configuration.Default.Enabled ? QuantStatus.Idle : QuantStatus.Disabled;

        public string Name => _name;

        public HashSet<SecurityId> Securities => _securities;

        public IBasketService BasketService { set => _basketService = value ?? throw new ArgumentNullException("BasketService"); }

        public QuantStatus Status => _status;

        public bool IsActive => _status == QuantStatus.Active;

        public TestQuant()
        {
            try
            {
                var dsec = JsonConvert.DeserializeAnonymousType(Configuration.Default.Securities, new[] { new { c = "", s = "" } });
                _name = "TestQuant: " + string.Join(",", dsec.Select(d => d.s));
                _securities = dsec.Select(d => new SecurityId { ClassCode = d.c, SecurityCode = d.s }).ToHashSet();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Cannot initialize. Securites = {Configuration.Default.Securities}");
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
            if (Status == QuantStatus.Disabled) return;
            try
            {
                switch (message)
                {
                    case L1QuotationsMessage qm:
                        if (IsActive) _logger.Debug("qm");
                        break;
                    case TimerMessage tm:
                        if (IsActive) _logger.Debug("tm");
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

        public object GetConfiguration()
        {
            return Configuration.Default;
        }

        public void SaveConfiguration()
        {
            Configuration.Default.Save();
        }
    }
}
