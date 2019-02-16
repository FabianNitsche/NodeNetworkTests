﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using UcrPoc.ViewModels.Editors;
using UcrPoc.ViewModels.Ports;

namespace UcrPoc.ViewModels.Nodes
{
    public class DeadzoneNode : NodeViewModel
    {
        private readonly Subject<short?> _output = new Subject<short?>();
        private ValueNodeInputViewModel<float?> _dzAmount;
        private double _scaleFactor;
        private double _deadzoneCutoff;

        static DeadzoneNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<DeadzoneNode>));
        }

        public DeadzoneNode()
        {
            Name = "Deadzone";

            _dzAmount = new ValueNodeInputViewModel<float?>()
            {
                Name = "Input",
                Port = null,
                Editor = new FloatEditorViewModel()
            };
            Inputs.Add(_dzAmount);

            _dzAmount.ValueChanged.Subscribe(newValue =>
            {
                if (newValue == 0)
                {
                    _deadzoneCutoff = 0;
                    _scaleFactor = 1.0;
                }
                else
                {
                    _deadzoneCutoff = (double) (short.MaxValue * (newValue * 0.01));
                    _scaleFactor = short.MaxValue / (short.MaxValue - _deadzoneCutoff);
                }
            });

            var input = new ValueNodeInputViewModel<short?>()
            {
                Name = "Input",
                Port = new AxisPortViewModel(),
            };
            Inputs.Add(input);

            input.ValueChanged.Subscribe(newValue =>
            {
                _output.OnNext(ApplyDeadzone(newValue));
            });

            Outputs.Add(new ValueNodeOutputViewModel<short?>
            {
                Name = "Output",
                Port = new AxisPortViewModel(),
                Value = _output
            });

        }

        private short? ApplyDeadzone(short? inValue)
        {
            if (inValue == null) return null;
            var value = (short) inValue;
            var wideVal = WideAbs(value);
            if (wideVal < Math.Round(_deadzoneCutoff))
            {
                return 0;
            }

            var sign = Math.Sign(value);
            var adjustedValue = (wideVal - _deadzoneCutoff) * _scaleFactor;
            var newValue = (int)Math.Round(adjustedValue * sign);
            if (newValue < -32768) newValue = -32768;   // ToDo: Negative values can go up to -32777 (9 over), can this be improved?
            //Debug.WriteLine($"Pre-DZ: {value}, Post-DZ: {newValue}, Cutoff: {_deadzoneCutoff}");
            return (short)newValue;
        }

        public static int WideAbs(short value)
        {
            return Math.Abs((int)value);
        }
    }
}