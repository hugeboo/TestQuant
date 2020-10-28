using Newtonsoft.Json;
using NLog;
using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Interfaces;
using QuantaBasket.Core.Mathx;
using QuantaBasket.Core.Messages;
using System;
using System.Collections.Generic;
using System.IO;
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

        private StreamWriter _writer1;
        private StreamWriter _writer2;

        private SMA _sma1;
        private SMA _sma2;

        private QuantStatus _status = Configuration.Instance.Enabled ? QuantStatus.Idle : QuantStatus.Disabled;

        public string Name => _name;

        public HashSet<SecurityId> Securities => _securities;

        public QuantStatus Status => _status;

        private bool IsActive => _status == QuantStatus.Active;

        public TestQuant()
        {
            var dsec = JsonConvert.DeserializeAnonymousType(Configuration.Instance.Securities, new[] { new { c = "", s = "" } });
            _name = "TestQuant: " + string.Join(",", dsec.Select(d => d.s));
            _securities = dsec.Select(d => new SecurityId { ClassCode = d.c, SecurityCode = d.s }).ToHashSet();

        }

        public void Init(IBasketService basketService)
        {
            _logger.Info($"Initialize quant: {_name}");
            _basketService = basketService;
            _basketService.RegisterCallback(MessageProcessor);
        }

        //private bool sent;
        //private string signalId;
        //private bool canceled;

        private void MessageProcessor(AMessage message)
        {
            if (Status == QuantStatus.Disabled) return;
            switch (message)
            {
                case L1QuotationsMessage qm:
                    if (IsActive)
                    {
                        _logger.Debug("qm");
                        foreach(var q in qm.Quotations)
                        {
                            if (q.Security.SecurityCode == "RIZ0" && q.DVolume!=0L)
                            {
                                _sma1?.Add(new TimePoint(q.DateTime, q.Last));
                                _sma2?.Add(new TimePoint(q.DateTime, q.Last));
                            }
                        }
                    }
                    break;
                case SignalChangedMessage scm:
                    _logger.Debug($"scm: {scm}");
                    break;
                case TimerMessage tm:
                    if (IsActive) _logger.Debug("tm");
                    //if (!sent)
                    //{
                    //    sent = true;
                    //    var s = _basketService.CreateSignal();
                    //    signalId = s.Id;
                    //    s.ClassCode = "TQBR";
                    //    s.SecCode = "GAZP";
                    //    s.Side = SignalSide.Buy;
                    //    s.Qtty = 10;
                    //    s.Price = 3999.0m;
                    //    s.PriceType = PriceType.Limit;
                    //    _basketService.SendSignal(s);
                    //}
                    //else if (!canceled)
                    //{
                    //    canceled = true;
                    //    _basketService.CancelSignal(signalId);
                    //}
                    break;
                case StartMessage sm:
                    _logger.Debug("sm");
                    if (_status != QuantStatus.Disabled)
                    {
                        _status = QuantStatus.Active;
                        _writer1 = new StreamWriter("sma1.csv");
                        _writer2 = new StreamWriter("sma2.csv");
                        _sma1 = new SMA(28);
                        _sma1.RegisterCallback((d) => 
                        {
                            _writer1.WriteLine(d.ToString());
                            _writer1.Flush();
                        });
                        _sma2 = new SMA(14);
                        _sma2.RegisterCallback((d) =>
                        {
                            _writer2.WriteLine(d.ToString());
                            _writer2.Flush();
                        });
                    }
                    break;
                case StopMessage stm:
                    _logger.Debug("stm");
                    if (_status != QuantStatus.Disabled)
                    {
                        _status = QuantStatus.Idle;
                        _writer1.Dispose();
                        _writer2.Dispose();
                        _writer1 = null;
                        _writer2 = null;
                    }
                    break;
                case null:
                    throw new NullReferenceException("message");
            }
        }

        public void Dispose()
        {
            _logger.Debug($"{_name} disposing");
        }

        public object GetConfiguration()
        {
            return Configuration.Instance;
        }

        public void SaveConfiguration()
        {
            Configuration.Instance.Save();
        }
    }
}
