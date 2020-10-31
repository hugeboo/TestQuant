using Newtonsoft.Json;
using NLog;
using QuantaBasket.Core.Contracts;
using QuantaBasket.Core.Extensions;
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
        private readonly IStreamDataStore _streamDataStore;
        private readonly IStreamDataChannel<TimePoint> _slowSMAChannel;
        private readonly IStreamDataChannel<TimePoint> _fastSMAChannel;
        private readonly IStreamDataChannel<TimePoint> _signalsChannel;
        private readonly IStreamDataChannel<OHLCV> _ohlcvChannel;

        private readonly DoubleSMA _doubleSMA;
        private readonly BarGenerator _barGenerator;

        public override string QuantBaseName => "TestQuant";

        protected override Configuration Configuration => Configuration.Instance;

        public TestQuant()
        {
            _streamDataStore = new SQLiteStreamDataStore($"Data Source=d:\\temp\\{Name}.db;Version=3;");

            _slowSMAChannel = _streamDataStore.OpenOrCreateChannel<TimePoint>("SlowSMA");
            _fastSMAChannel = _streamDataStore.OpenOrCreateChannel<TimePoint>("FastSMA");
            _signalsChannel = _streamDataStore.OpenOrCreateChannel<TimePoint>("Signals");
            _ohlcvChannel = _streamDataStore.OpenOrCreateChannel<OHLCV>("OHLCV");

            _barGenerator = new BarGenerator(Configuration.BarInerval);
            _barGenerator.RegisterCallback(b =>
            {
                Logger.Info(b.ToString());
                _ohlcvChannel.Write(b);
                //_doubleSMA.Add(new TimePoint(b.StartTime.AddSeconds((int)b.IntervalSec), b.Close));
            });

            _doubleSMA = new DoubleSMA(Configuration.SlowSMAPeriod, Configuration.FastSMAPeriod);
            _doubleSMA.RegisterCallback(r =>
            {
                _slowSMAChannel.Write(r.SlowSMA);
                _fastSMAChannel.Write(r.FastSMA);
                if (r.Event != DoubleSMA.Event.None)
                {
                    _signalsChannel.Write(new TimePoint(r.SlowSMA.Time, r.Event == DoubleSMA.Event.FastBreakAbove ? -100 : +100));
                }
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            _ohlcvChannel.Dispose();
            _slowSMAChannel.Dispose();
            _fastSMAChannel.Dispose();
            _signalsChannel.Dispose();
            _streamDataStore.Dispose();
        }

        protected override void OnStartMessage(StartMessage m)
        {
            _barGenerator.Reset();
            _doubleSMA.Reset();
        }

        protected override void OnL1QuotationsMessage(L1QuotationsMessage m)
        {
            m.Quotations.ForEach(q => 
            {
                _barGenerator.Add(q);
                if ((q.Changes & L1QuotationChangedFlags.Trade) != 0)
                {
                    _doubleSMA.Add(new TimePoint(q.DateTime, q.Last));
                }
            });
        }
    }
}
